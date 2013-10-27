using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Razor;
using Microsoft.CSharp;
using Orchard.Mvc;

namespace Orchard.Templates.Services {
    public interface IRazorParser : IDependency {
        string Parse<TModel>(string razor, TModel model = default(TModel));
    }

    public class RazorParser : IRazorParser {
        const string DynamicallyGeneratedClassName = "RazorView";
        const string NamespaceForDynamicClasses = "Orchard.Templates";
        const string DynamicClassFullName = NamespaceForDynamicClasses + "." + DynamicallyGeneratedClassName;

        public string Parse<TModel>(string razor, TModel model = default(TModel)) {
            var language = new CSharpRazorCodeLanguage();
            var host = new RazorEngineHost(language) {
                DefaultBaseClass = typeof(RazorViewBase<TModel>).FullName,
                DefaultClassName = DynamicallyGeneratedClassName,
                DefaultNamespace = NamespaceForDynamicClasses,
            };
            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.Dynamic");
            host.NamespaceImports.Add("System.Text");
            var engine = new RazorTemplateEngine(host);
            var tr = new StringReader(razor);
            var razorTemplate = engine.GenerateCode(tr);
            var compiledAssembly = CreateCompiledAssemblyFor(razorTemplate.GeneratedCode);
            var templateInstance = (RazorViewBase<TModel>)compiledAssembly.CreateInstance(DynamicClassFullName);

            return templateInstance.GetContent();
        }

        private Assembly CreateCompiledAssemblyFor(CodeCompileUnit unitToCompile) {
            var compilerParameters = new CompilerParameters();
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            compilerParameters.ReferencedAssemblies.Add(typeof(RazorParser).Assembly.Location);
            compilerParameters.GenerateInMemory = true;

            var compilerResults = new CSharpCodeProvider().CompileAssemblyFromDom(compilerParameters, unitToCompile);
            var compiledAssembly = compilerResults.CompiledAssembly;
            return compiledAssembly;
        }
    }

    public abstract class RazorViewBase<TModel> /*: ViewPage<TModel>*/ {
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
    }
}