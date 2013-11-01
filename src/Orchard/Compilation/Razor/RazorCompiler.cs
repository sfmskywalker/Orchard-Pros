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
        private readonly ISignals _signals;
        const string DynamicallyGeneratedClassName = "RazorTemplate";
        const string NamespaceForDynamicClasses = "Orchard.Framework.Compilation.Razor";
        const string DynamicClassFullName = NamespaceForDynamicClasses + "." + DynamicallyGeneratedClassName;
        private const string ForceRecompile = "Razor.ForceRecompile";

        public RazorCompiler(ICacheManager cache, ISignals signals) {
            _cache = cache;
            _signals = signals;
            Logger = NullLogger.Instance;
        }

        ILogger Logger { get; set; }

        public RazorTemplateBase CompileRazor(string code, string name, IDictionary<string, object> parameters) {
            return Compile<RazorTemplateBase>(code, name, parameters);
        }

        public object Compile(string code, IDictionary<string, object> parameters) {
            return CompileRazor(code, null, parameters);
        }

        public T Compile<T>(string code, IDictionary<string, object> parameters) {
            return Compile<T>(code, null, parameters);
        }

        private T Compile<T>(string code, string name, IDictionary<string, object> parameters) {
            var cacheKey = name ?? GetHash(code);
            var assembly = _cache.Get(cacheKey, ctx => {
                ctx.Monitor(_signals.When(ForceRecompile));

                var language = new CSharpRazorCodeLanguage();
                var host = new RazorEngineHost(language) {
                    DefaultBaseClass = typeof(T).FullName,
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
                var reader = new StringReader(code);
                var builder = new StringBuilder();

                // A hack to remove any @model directive as it's MVC-specific and compiler does not recognize it.
                // We should use this information to compile a strongly-typed template in the future
                string line;
                while ((line = reader.ReadLine()) != null) {
                    if (!line.TrimStart(' ', '\t', '\n', '\r').StartsWith("@model "))
                        builder.AppendLine(line);
                }

                var razorTemplate = engine.GenerateCode(new StringReader(builder.ToString()));
                var compiledAssembly = CreateCompiledAssemblyFor(razorTemplate.GeneratedCode, name);
                return compiledAssembly;
            });

            return (T)assembly.CreateInstance(DynamicClassFullName);
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