using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor;
using System.Web.UI;
using Microsoft.CSharp;

namespace Orchard.Templates.Services {
    public class RazorTemplateParser : TemplateParserImpl {
        const string DynamicallyGeneratedClassName = "RazorView";
        const string NamespaceForDynamicClasses = "Orchard.Templates";
        const string DynamicClassFullName = NamespaceForDynamicClasses + "." + DynamicallyGeneratedClassName;

        public override string Type {
            get { return "Razor"; }
        }

        public override string Parse<TModel>(string razor, Action<ITemplateViewBase<TModel>> activator, TModel model = default(TModel)) {
            var language = new CSharpRazorCodeLanguage();
            var host = new RazorEngineHost(language) {
                DefaultBaseClass = typeof(RazorViewBase<TModel>).FullName,
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
            var tr = new StringReader(razor);
            var razorTemplate = engine.GenerateCode(tr);
            var compiledAssembly = CreateCompiledAssemblyFor(razorTemplate.GeneratedCode);
            var templateInstance = (RazorViewBase<TModel>)compiledAssembly.CreateInstance(DynamicClassFullName);

            if (activator != null) {
                activator(templateInstance);
            }

            return templateInstance.GetContent();
        }

        private static Assembly CreateCompiledAssemblyFor(CodeCompileUnit unitToCompile) {
            var compilerParameters = new CompilerParameters();
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            compilerParameters.ReferencedAssemblies.Add(typeof(HtmlString).Assembly.Location);
            compilerParameters.ReferencedAssemblies.Add(typeof(ActionResult).Assembly.Location);
            compilerParameters.ReferencedAssemblies.Add(typeof(RazorTemplateParser).Assembly.Location);
            compilerParameters.ReferencedAssemblies.Add(typeof(IDependency).Assembly.Location);
            compilerParameters.GenerateInMemory = true;

            var compilerResults = new CSharpCodeProvider().CompileAssemblyFromDom(compilerParameters, unitToCompile);
            var compiledAssembly = compilerResults.CompiledAssembly;
            return compiledAssembly;
        }
    }

    public abstract class RazorViewBase<TModel> : Mvc.ViewPage<TModel>, ITemplateViewBase<TModel> {
        private StringBuilder _buffer;

        public abstract void Execute();

        protected void Write(object value) {
            WriteLiteral(value);
        }

        protected void WriteLiteral(object value) {
            _buffer.Append(value);
        }

        public string GetContent() {
            _buffer = new StringBuilder(1024);
            Execute();
            return _buffer.ToString();
        }

        HtmlTextWriter ITemplateViewBase<TModel>.Writer { get { return Writer; } set { base.Render(value);} }
    }

    public interface ITemplateViewBase<TModel>
    {
        ViewContext ViewContext { get; set; }
        AjaxHelper<TModel> Ajax { get; set; }
        HtmlHelper<TModel> Html { get; set; }
        UrlHelper Url { get; set; }
        HtmlTextWriter Writer { get; set; }
        void InitHelpers();
    }
}