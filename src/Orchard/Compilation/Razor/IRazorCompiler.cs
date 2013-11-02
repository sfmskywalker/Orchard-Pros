using System.Collections.Generic;

namespace Orchard.Compilation.Razor {
    public interface IRazorCompiler : ICompiler {
        RazorTemplateBase CompileRazor(string code, string name, IDictionary<string, object> parameters);
    }
}