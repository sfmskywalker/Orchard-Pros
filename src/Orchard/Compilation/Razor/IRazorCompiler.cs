using System;
using System.Collections.Generic;

namespace Orchard.Compilation.Razor {
    public interface IRazorCompiler : ICompiler {
        IRazorTemplateBase<TModel> CompileRazor<TModel>(string code, string name, IDictionary<string, object> parameters);
        IRazorTemplateBase CompileRazor(string code, string name, Type modelType, IDictionary<string, object> parameters);
    }
}