namespace Orchard.Compilation.Razor {
    public interface IRazorTemplateCache : ISingletonDependency {
        string Get(string name);
        void Set(string name, string template);
    }
}