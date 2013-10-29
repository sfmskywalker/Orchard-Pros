using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor;
using System.Web.Routing;
using System.Web.WebPages;
using Microsoft.CSharp;
using Orchard.Caching;
using Orchard.DisplayManagement.Implementation;
using Orchard.Logging;

namespace Orchard.Templates.Services {
    public class RazorTemplateProcessor : TemplateProcessorImpl {
        private readonly ICacheManager _cache;
        private readonly RouteCollection _routeCollection;
        const string DynamicallyGeneratedClassName = "RazorCompiledTemplate";
        const string NamespaceForDynamicClasses = "Orchard.Templates";
        const string DynamicClassFullName = NamespaceForDynamicClasses + "." + DynamicallyGeneratedClassName;

        public override string Type {
            get { return "Razor"; }
        }

        public RazorTemplateProcessor(ICacheManager cache, RouteCollection routeCollection) {
            _cache = cache;
            _routeCollection = routeCollection;
            Logger = NullLogger.Instance;
        }

        ILogger Logger { get; set; }

        public override void Verify(string template) {
            Compile(template);
        }

        public override string Process(string template, DisplayContext context = null, dynamic model = null) {
            var instance = Compile(template);

            if (context != null) {
                Activate(instance, context);
            }

            return instance.GetContent();
        }

        private RazorTemplate Compile(string template) {
            var hash = GetHash(template);
            return _cache.Get(hash, ctx => {
                var language = new CSharpRazorCodeLanguage();
                var host = new RazorEngineHost(language) {
                    DefaultBaseClass = typeof(RazorTemplate).FullName,
                    DefaultClassName = DynamicallyGeneratedClassName,
                    DefaultNamespace = NamespaceForDynamicClasses,
                };

                var namespaces = new List<string> {
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.Collections",
                    "System.Collections.Generic",
                    "System.Dynamic",
                    "System.Text",
                    "System.Web",
                    "System.Web.Mvc",
                    "System.Web.Mvc.Html",
                    "System.Web.Mvc.Ajax",
                    "System.Web.UI",
                    "Orchard.ContentManagement",
                    "Orchard.DisplayManagement",
                    "Orchard.DisplayManagement.Shapes",
                    "Orchard.Security.Permissions",
                    "Orchard.UI.Resources",
                    "Orchard.Security",
                    "Orchard.Mvc.Spooling",
                    "Orchard.Mvc.Html"
                };

                foreach (var n in namespaces) {
                    host.NamespaceImports.Add(n);
                }

                var engine = new RazorTemplateEngine(host);
                var tr = new StringReader(template);

                var razorTemplate = engine.GenerateCode(tr);
                var compiledAssembly = CreateCompiledAssemblyFor(razorTemplate.GeneratedCode);

                if (compiledAssembly == null) {
                    return null;
                }

                return (RazorTemplate)compiledAssembly.CreateInstance(DynamicClassFullName);
            });
        }

        private Assembly CreateCompiledAssemblyFor(CodeCompileUnit unitToCompile) {
            var compilerParameters = new CompilerParameters();
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Dynamic.dll");
            compilerParameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            compilerParameters.ReferencedAssemblies.Add(typeof(WebPage).Assembly.Location);
            compilerParameters.ReferencedAssemblies.Add(typeof(HtmlString).Assembly.Location);
            compilerParameters.ReferencedAssemblies.Add(typeof(ActionResult).Assembly.Location);
            compilerParameters.ReferencedAssemblies.Add(typeof(RazorTemplateProcessor).Assembly.Location);
            compilerParameters.ReferencedAssemblies.Add(typeof(IDependency).Assembly.Location);
            compilerParameters.GenerateInMemory = true;

            var compilerResults = new CSharpCodeProvider().CompileAssemblyFromDom(compilerParameters, unitToCompile);

            if (compilerResults.Errors.HasErrors) {
                var errors = compilerResults.Errors.Cast<CompilerError>().Aggregate(string.Empty, (s, error) => s + "\r\n" + error.ToString());
                throw new Exception(string.Format("Razor template compilation errors:\r\n{0}", errors));
            }
            else {
                var compiledAssembly = compilerResults.CompiledAssembly;
                return compiledAssembly;
            }
        }

        public static string GetHash(string value) {
            var data = Encoding.ASCII.GetBytes(value);
            var hashData = new SHA1Managed().ComputeHash(data);

            return hashData.Aggregate(string.Empty, (current, b) => current + b.ToString("X2"));
        }


        private void Activate(ITemplateBase obj, DisplayContext displayContext) {
            //obj.Writer = new HtmlTextWriter(disp);
            obj.Url = new UrlHelper(displayContext.ViewContext.RequestContext, _routeCollection);
            obj.Html = new HtmlHelper<dynamic>(displayContext.ViewContext, displayContext.ViewDataContainer, _routeCollection);
            obj.Ajax = new AjaxHelper<dynamic>(displayContext.ViewContext, displayContext.ViewDataContainer, _routeCollection);
            obj.ViewContext = displayContext.ViewContext;
            obj.ViewData = displayContext.ViewDataContainer.ViewData;
            obj.ViewData.Model = displayContext.Value;
            obj.InitHelpers();
        }
    }

}