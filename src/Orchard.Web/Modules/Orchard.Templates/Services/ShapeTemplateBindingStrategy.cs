using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Orchard.Caching;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using Orchard.DisplayManagement.Implementation;
using Orchard.Environment;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Helpers;
using Orchard.Environment.Extensions.Models;
using Orchard.FileSystems.VirtualPath;
using Orchard.Logging;
using Orchard.Mvc.Spooling;
using Orchard.Mvc.ViewEngines.ThemeAwareness;
using Orchard.Utility.Extensions;

namespace Orchard.Templates.Services
{
    [OrchardSuppressDependency("Orchard.DisplayManagement.Descriptors.ShapeTemplateStrategy.ShapeTemplateBindingStrategy")]
    public class ShapeTemplateBindingStrategy : IShapeTableProvider
    {
        private readonly ShellDescriptor _shellDescriptor;
        private readonly IExtensionManager _extensionManager;
        private readonly ICacheManager _cacheManager;
        private readonly IVirtualPathMonitor _virtualPathMonitor;
        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly IEnumerable<IShapeTemplateHarvester> _harvesters;
        private readonly IEnumerable<IShapeTemplateViewEngine> _shapeTemplateViewEngines;
        private readonly IParallelCacheContext _parallelCacheContext;
        private readonly Work<ILayoutAwareViewEngine> _viewEngine;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ShapeTemplateBindingStrategy(
            IEnumerable<IShapeTemplateHarvester> harvesters,
            ShellDescriptor shellDescriptor,
            IExtensionManager extensionManager,
            ICacheManager cacheManager,
            IVirtualPathMonitor virtualPathMonitor,
            IVirtualPathProvider virtualPathProvider,
            IEnumerable<IShapeTemplateViewEngine> shapeTemplateViewEngines,
            IParallelCacheContext parallelCacheContext,
            Work<ILayoutAwareViewEngine> viewEngine,
            IWorkContextAccessor workContextAccessor)
        {

            _harvesters = harvesters;
            _shellDescriptor = shellDescriptor;
            _extensionManager = extensionManager;
            _cacheManager = cacheManager;
            _virtualPathMonitor = virtualPathMonitor;
            _virtualPathProvider = virtualPathProvider;
            _shapeTemplateViewEngines = shapeTemplateViewEngines;
            _parallelCacheContext = parallelCacheContext;
            _viewEngine = viewEngine;
            _workContextAccessor = workContextAccessor;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
        public bool DisableMonitoring { get; set; }

        private static IEnumerable<ExtensionDescriptor> Once(IEnumerable<FeatureDescriptor> featureDescriptors)
        {
            var once = new ConcurrentDictionary<string, object>();
            return featureDescriptors.Select(fd => fd.Extension).Where(ed => once.TryAdd(ed.Id, null)).ToList();
        }

        public void Discover(ShapeTableBuilder builder)
        {
            EnsureWorkContext(() => DiscoverInternal(builder));
        }

        private void DiscoverInternal(ShapeTableBuilder builder)
        {
            Logger.Information("Start discovering shapes");

            var harvesterInfos = _harvesters.Select(harvester => new { harvester, subPaths = harvester.SubPaths() });

            var availableFeatures = _extensionManager.AvailableFeatures();
            var activeFeatures = availableFeatures.Where(FeatureIsEnabled);
            var activeExtensions = Once(activeFeatures);

            var hits = _parallelCacheContext.RunInParallel(activeExtensions, extensionDescriptor =>
            {
                Logger.Information("Start discovering candidate views filenames");
                var pathContexts = harvesterInfos.SelectMany(harvesterInfo => harvesterInfo.subPaths.Select(subPath =>
                {
                    var basePath = Path.Combine(extensionDescriptor.Location, extensionDescriptor.Id).Replace(Path.DirectorySeparatorChar, '/');
                    var virtualPath = Path.Combine(basePath, subPath).Replace(Path.DirectorySeparatorChar, '/');
                    var fileNames = _cacheManager.Get(virtualPath, ctx =>
                    {
                        if (!_virtualPathProvider.DirectoryExists(virtualPath))
                            return new List<string>();

                        if (!DisableMonitoring)
                        {
                            Logger.Debug("Monitoring virtual path \"{0}\"", virtualPath);
                            ctx.Monitor(_virtualPathMonitor.WhenPathChanges(virtualPath));
                        }

                        return _virtualPathProvider.ListFiles(virtualPath).Select(Path.GetFileName).ToReadOnlyCollection();
                    });
                    return new { harvesterInfo.harvester, basePath, subPath, virtualPath, fileNames };
                })).ToList();
                Logger.Information("Done discovering candidate views filenames");

                var fileContexts = pathContexts.SelectMany(pathContext => _shapeTemplateViewEngines.SelectMany(ve =>
                {
                    var fileNames = ve.DetectTemplateFileNames(pathContext.fileNames);
                    return fileNames.Select(
                        fileName => new
                        {
                            fileName = Path.GetFileNameWithoutExtension(fileName),
                            fileVirtualPath = Path.Combine(pathContext.virtualPath, fileName).Replace(Path.DirectorySeparatorChar, '/'),
                            pathContext
                        });
                }));

                var shapeContexts = fileContexts.SelectMany(fileContext =>
                {
                    var harvestShapeInfo = new HarvestShapeInfo
                    {
                        SubPath = fileContext.pathContext.subPath,
                        FileName = fileContext.fileName,
                        TemplateVirtualPath = fileContext.fileVirtualPath
                    };
                    var harvestShapeHits = fileContext.pathContext.harvester.HarvestShape(harvestShapeInfo);
                    return harvestShapeHits.Select(harvestShapeHit => new { harvestShapeInfo, harvestShapeHit, fileContext });
                });

                return shapeContexts.Select(shapeContext => new { extensionDescriptor, shapeContext }).ToList();
            }).SelectMany(hits2 => hits2);

            var templateCache = _workContextAccessor.GetContext().Resolve<ITemplateCache>();

            foreach (var iter in hits)
            {
                // templates are always associated with the namesake feature of module or theme
                var hit = iter;
                var featureDescriptors = iter.extensionDescriptor.Features.Where(fd => fd.Id == hit.extensionDescriptor.Id);
                foreach (var featureDescriptor in featureDescriptors)
                {
                    Logger.Debug("Binding {0} as shape [{1}] for feature {2}",
                        hit.shapeContext.harvestShapeInfo.TemplateVirtualPath,
                        iter.shapeContext.harvestShapeHit.ShapeType,
                        featureDescriptor.Id);

                    // reading contents of each .cshtml file and putting them into the cache
                    var contents = File.ReadAllText(PathHelpers.GetPhysicalPath(hit.shapeContext.harvestShapeInfo.TemplateVirtualPath));
                    templateCache.Set(hit.shapeContext.harvestShapeInfo.TemplateVirtualPath, contents);

                    builder.Describe(iter.shapeContext.harvestShapeHit.ShapeType)
                        .From(new Feature { Descriptor = featureDescriptor })
                        .BoundAs(
                            hit.shapeContext.harvestShapeInfo.TemplateVirtualPath,
                            shapeDescriptor => displayContext => Render(displayContext, hit.shapeContext.harvestShapeInfo));
                }
            }

            Logger.Information("Done discovering shapes");
        }

        private bool FeatureIsEnabled(FeatureDescriptor fd)
        {
            return (DefaultExtensionTypes.IsTheme(fd.Extension.ExtensionType) && (fd.Id == "TheAdmin" || fd.Id == "SafeMode")) ||
                _shellDescriptor.Features.Any(sf => sf.Name == fd.Id);
        }

        private void EnsureWorkContext(Action action)
        {
            var workContext = _workContextAccessor.GetContext();
            if (workContext != null)
            {
                action();
            }
            else
            {
                using (_workContextAccessor.CreateWorkContextScope())
                {
                    action();
                }
            }
        }

        private IHtmlString Render(DisplayContext displayContext, HarvestShapeInfo harvestShapeInfo)
        {
            Logger.Information("Rendering template file '{0}'", harvestShapeInfo.TemplateVirtualPath);

            var templateCache = _workContextAccessor.GetContext().Resolve<ITemplateCache>();
            var template = templateCache.Get(harvestShapeInfo.TemplateVirtualPath);
            var result = template != null ? PerformInvoke(displayContext, "Razor", template, harvestShapeInfo.TemplateVirtualPath) : new HtmlString("");

            Logger.Information("Done rendering template file '{0}'", harvestShapeInfo.TemplateVirtualPath);
            return result ?? new HtmlString("");
        }

        private IHtmlString PerformInvoke(DisplayContext displayContext, string type, string template, string virtualPath)
        {
            var service = _workContextAccessor.GetContext().Resolve<ITemplateService>();
            var output = new HtmlStringWriter();

            if (String.IsNullOrEmpty(template))
                return null;

            output.Write(CoerceHtmlString(service.Execute(template, type, displayContext, virtualPath)));

            return output;
        }

        private static IHtmlString CoerceHtmlString(object invoke)
        {
            return invoke as IHtmlString ?? (invoke != null ? new HtmlString(invoke.ToString()) : null);
        }
    }
}
