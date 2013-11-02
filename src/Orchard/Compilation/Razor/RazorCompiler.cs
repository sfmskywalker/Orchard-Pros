using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Razor;
using System.Web.Razor.Generator;
using Microsoft.CSharp;
using Orchard.Caching;
using Orchard.Logging;

namespace Orchard.Compilation.Razor {
    public class RazorCompiler : IRazorCompiler {
        private readonly ICacheManager _cache;
        private readonly IWorkContextAccessor _wca;
        const string DynamicallyGeneratedClassName = "RazorTemplate";
        const string NamespaceForDynamicClasses = "Orchard.Framework.Compilation.Razor";
        const string DynamicClassFullName = NamespaceForDynamicClasses + "." + DynamicallyGeneratedClassName;
        private const string ForceRecompile = "Razor.ForceRecompile";

        public RazorCompiler(ICacheManager cache, IWorkContextAccessor wca) {
            _cache = cache;
            _wca = wca;
            Logger = NullLogger.Instance;
        }

        ILogger Logger { get; set; }

        public IRazorTemplateBase<TModel> CompileRazor<TModel>(string code, string name, IDictionary<string, object> parameters) {
            return (RazorTemplateBase<TModel>)Compile(code, name, typeof(TModel), parameters);
        }

        public IRazorTemplateBase CompileRazor(string code, string name, Type modelType, IDictionary<string, object> parameters) {
            return (IRazorTemplateBase)Compile(code, name, modelType, parameters);
        }

        public object Compile(string code, IDictionary<string, object> parameters) {
            return CompileRazor<RazorTemplateBase<dynamic>>(code, null, parameters);
        }

        public T Compile<T>(string code, IDictionary<string, object> parameters) {
            return (T) Compile(code, null, null, parameters);
        }

        private object Compile(string code, string name, Type modelType, IDictionary<string, object> parameters) {
            ISignals signals = _wca.GetContext().TryResolve(out signals) ? signals : null;

            var cacheKey = name ?? GetHash(code);
            var assembly = _cache.Get(cacheKey, ctx => {
                if (signals != null) {
                    ctx.Monitor(signals.When(ForceRecompile));
                }

                bool useDynamic = true;
                string modelTypeName = "";
                var reader = new StringReader(code);
                var builder = new StringBuilder();

                // A hack to remove any @model directive as it's MVC-specific and compiler does not recognize it.
                // We should use this information to compile a strongly-typed template in the future
                string line;
                while ((line = reader.ReadLine()) != null) {
                    var internalLine = line.TrimStart(' ', '\t', '\n', '\r');
                    if (internalLine.StartsWith("@model ")) {
                        modelTypeName = internalLine.Substring("@model ".Length).Trim();
                        if(modelTypeName != "dynamic") useDynamic = false;

                        continue;
                    }

                    builder.AppendLine(line);
                }

                var baseType = useDynamic ? typeof (RazorTemplateBase) : typeof (RazorTemplateBase<>).MakeGenericType(modelType);

                var language = new CSharpRazorCodeLanguage();
                var host = new RazorEngineHost(language) {
                    DefaultBaseClass = baseType.FullName,
                    DefaultClassName = DynamicallyGeneratedClassName,
                    DefaultNamespace = NamespaceForDynamicClasses
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
                    "System.Web.Routing",
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
                var razorTemplate = engine.GenerateCode(new StringReader(builder.ToString()));
                var compiledAssembly = CreateCompiledAssemblyFor(razorTemplate.GeneratedCode, name);
                return compiledAssembly;
            });

            return assembly.CreateInstance(DynamicClassFullName);
        }

        public static string GetHash(string value)
        {
            var data = Encoding.ASCII.GetBytes(value);
            var hashData = new SHA1Managed().ComputeHash(data);

            return hashData.Aggregate(string.Empty, (current, b) => current + b.ToString("X2"));
        }

        private static Assembly CreateCompiledAssemblyFor(CodeCompileUnit unitToCompile, string templateName)
        {
            var compilerParameters = new CompilerParameters();
            compilerParameters.ReferencedAssemblies.AddRange(AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Select(a => a.Location)
                .ToArray());

            compilerParameters.GenerateInMemory = true;

            var compilerResults = new CSharpCodeProvider().CompileAssemblyFromDom(compilerParameters, unitToCompile);
            if (compilerResults.Errors.HasErrors) {
                var errors = compilerResults.Errors.Cast<CompilerError>().Aggregate(string.Empty, (s, error) => s + "\r\nTemplate '" + templateName + "': " + error.ToString());
                throw new Exception(string.Format("Razor template compilation errors:\r\n{0}", errors));
            }
            else {
                var compiledAssembly = compilerResults.CompiledAssembly;
                return compiledAssembly;
            }
        }
    }
}