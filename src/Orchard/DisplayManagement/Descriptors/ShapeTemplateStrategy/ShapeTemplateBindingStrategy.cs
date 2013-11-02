using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using System.Web.WebPages;
using Orchard.Caching;
using Orchard.Compilation.Razor;
using Orchard.DisplayManagement.Implementation;
using Orchard.DisplayManagement.Shapes;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Helpers;
using Orchard.Environment.Extensions.Models;
using Orchard.FileSystems.VirtualPath;
using Orchard.Logging;
using Orchard.Mvc.Spooling;
using Orchard.Utility.Extensions;

namespace Orchard.DisplayManagement.Descriptors.ShapeTemplateStrategy {
    public class ShapeTemplateBindingStrategy : IShapeTableProvider {
        private readonly ShellDescriptor _shellDescriptor;
        private readonly IExtensionManager _extensionManager;
        private readonly ICacheManager _cacheManager;
        private readonly IVirtualPathMonitor _virtualPathMonitor;
        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly RouteCollection _routeCollection;
        private readonly IEnumerable<IShapeTemplateHarvester> _harvesters;
        private readonly IEnumerable<IShapeTemplateViewEngine> _shapeTemplateViewEngines;
        private readonly IParallelCacheContext _parallelCacheContext;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ShapeTemplateBindingStrategy(
            RouteCollection routeCollection,
            IEnumerable<IShapeTemplateHarvester> harvesters,
            ShellDescriptor shellDescriptor,
            IExtensionManager extensionManager,
            ICacheManager cacheManager,
            IVirtualPathMonitor virtualPathMonitor,
            IVirtualPathProvider virtualPathProvider,
            IEnumerable<IShapeTemplateViewEngine> shapeTemplateViewEngines,
            IParallelCacheContext parallelCacheContext,
            IWorkContextAccessor workContextAccessor) {
            _routeCollection = routeCollection;
            _harvesters = harvesters;
            _shellDescriptor = shellDescriptor;
            _extensionManager = extensionManager;
            _cacheManager = cacheManager;
            _virtualPathMonitor = virtualPathMonitor;
            _virtualPathProvider = virtualPathProvider;
            _shapeTemplateViewEngines = shapeTemplateViewEngines;
            _parallelCacheContext = parallelCacheContext;
            _workContextAccessor = workContextAccessor;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
        public bool DisableMonitoring { get; set; }

        private static IEnumerable<ExtensionDescriptor> Once(IEnumerable<FeatureDescriptor> featureDescriptors) {
            var once = new ConcurrentDictionary<string, object>();
            return featureDescriptors.Select(fd => fd.Extension).Where(ed => once.TryAdd(ed.Id, null)).ToList();
        }

        public void Discover(ShapeTableBuilder builder) {
            EnsureWorkContext(() => DiscoverInternal(builder));
        }

        private void DiscoverInternal(ShapeTableBuilder builder) {
            Logger.Information("Start discovering shapes");

            var harvesterInfos = _harvesters.Select(harvester => new { harvester, subPaths = harvester.SubPaths() });

            var availableFeatures = _extensionManager.AvailableFeatures();
            var activeFeatures = availableFeatures.Where(FeatureIsEnabled);
            var activeExtensions = Once(activeFeatures);

            var hits = _parallelCacheContext.RunInParallel(activeExtensions, extensionDescriptor => {
                Logger.Information("Start discovering candidate views filenames");
                var pathContexts = harvesterInfos.SelectMany(harvesterInfo => harvesterInfo.subPaths.Select(subPath => {
                    var basePath = Path.Combine(extensionDescriptor.Location, extensionDescriptor.Id).Replace(Path.DirectorySeparatorChar, '/');
                    var virtualPath = Path.Combine(basePath, subPath).Replace(Path.DirectorySeparatorChar, '/');
                    var fileNames = _cacheManager.Get(virtualPath, ctx => {
                        if (!_virtualPathProvider.DirectoryExists(virtualPath))
                            return new List<string>();

                        if (!DisableMonitoring) {
                            Logger.Debug("Monitoring virtual path \"{0}\"", virtualPath);
                            ctx.Monitor(_virtualPathMonitor.WhenPathChanges(virtualPath));
                        }

                        return _virtualPathProvider.ListFiles(virtualPath).Select(Path.GetFileName).ToReadOnlyCollection();
                    });
                    return new { harvesterInfo.harvester, basePath, subPath, virtualPath, fileNames };
                })).ToList();
                Logger.Information("Done discovering candidate views filenames");

                var fileContexts = pathContexts.SelectMany(pathContext => _shapeTemplateViewEngines.SelectMany(ve => {
                    var fileNames = ve.DetectTemplateFileNames(pathContext.fileNames);
                    return fileNames.Select(
                        fileName => new {
                            fileName = Path.GetFileNameWithoutExtension(fileName),
                            fileVirtualPath = Path.Combine(pathContext.virtualPath, fileName).Replace(Path.DirectorySeparatorChar, '/'),
                            pathContext
                        });
                }));

                var shapeContexts = fileContexts.SelectMany(fileContext => {
                    var harvestShapeInfo = new HarvestShapeInfo {
                        SubPath = fileContext.pathContext.subPath,
                        FileName = fileContext.fileName,
                        TemplateVirtualPath = fileContext.fileVirtualPath
                    };
                    var harvestShapeHits = fileContext.pathContext.harvester.HarvestShape(harvestShapeInfo);
                    return harvestShapeHits.Select(harvestShapeHit => new { harvestShapeInfo, harvestShapeHit, fileContext });
                });

                return shapeContexts.Select(shapeContext => new { extensionDescriptor, shapeContext }).ToList();
            }).SelectMany(hits2 => hits2);

            var templateCache = _workContextAccessor.GetContext().Resolve<IRazorTemplateCache>();
            foreach (var iter in hits) {
                // templates are always associated with the namesake feature of module or theme
                var hit = iter;
                var featureDescriptors = iter.extensionDescriptor.Features.Where(fd => fd.Id == hit.extensionDescriptor.Id);
                foreach (var featureDescriptor in featureDescriptors) {
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

        private bool FeatureIsEnabled(FeatureDescriptor fd) {
            return (DefaultExtensionTypes.IsTheme(fd.Extension.ExtensionType) && (fd.Id == "TheAdmin" || fd.Id == "SafeMode")) ||
                _shellDescriptor.Features.Any(sf => sf.Name == fd.Id);
        }

        private void EnsureWorkContext(Action action) {
            var workContext = _workContextAccessor.GetContext();
            if (workContext != null) {
                action();
            }
            else {
                using (_workContextAccessor.CreateWorkContextScope()) {
                    action();
                }
            }
        }

        private IHtmlString Render(DisplayContext displayContext, HarvestShapeInfo harvestShapeInfo) {
            Logger.Information("Rendering template file '{0}'", harvestShapeInfo.TemplateVirtualPath);

            var output = new HtmlStringWriter();
            var compiler = _workContextAccessor.GetContext().Resolve<IRazorCompiler>();
            var templateCache = _workContextAccessor.GetContext().Resolve<IRazorTemplateCache>();
            var template = templateCache.Get(harvestShapeInfo.TemplateVirtualPath);

            if (String.IsNullOrEmpty(template))
                return new HtmlString("");

            var compiledTemplate = compiler.CompileRazor(template, harvestShapeInfo.TemplateVirtualPath, new Dictionary<string, object>());
            var result = ActivateAndRenderTemplate(compiledTemplate, displayContext, harvestShapeInfo.TemplateVirtualPath, _routeCollection);
            output.Write(CoerceHtmlString(result));

            Logger.Information("Done rendering template file '{0}'", harvestShapeInfo.TemplateVirtualPath);
            return output;
        }


        private static IHtmlString CoerceHtmlString(object invoke) {
            return invoke as IHtmlString ?? (invoke != null ? new HtmlString(invoke.ToString()) : null);
        }

        private static string ActivateAndRenderTemplate(IRazorTemplateBase obj, DisplayContext displayContext, string templateVirtualPath, RouteCollection routes)
        {
            var buffer = new StringBuilder(1024);
            using (var writer = new StringWriter(buffer)) {
                var htmlWriter = new HtmlTextWriter(writer);

                var shapeViewContext = new ViewContext(displayContext.ViewContext.Controller.ControllerContext, displayContext.ViewContext.View, displayContext.ViewContext.ViewData, displayContext.ViewContext.TempData, htmlWriter);
                obj.WebPageContext = new WebPageContext(displayContext.ViewContext.HttpContext, obj as WebPageRenderingBase, displayContext.Value);
                obj.ViewContext = shapeViewContext;
                obj.ViewData = new ViewDataDictionary(displayContext.ViewDataContainer.ViewData) { Model = displayContext.Value };
                obj.VirtualPath = templateVirtualPath;
                obj.InitHelpers();
                obj.Render(htmlWriter);
            }

            return buffer.ToString();
        }
    }
}
