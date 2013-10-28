using System.Collections.Concurrent;

namespace Orchard.Templates.Services {
    public interface ITemplateCache : ISingletonDependency {
        string Get(string name);
        void Set(string name, string template);
    }

    public class DefaultTemplateCache : ITemplateCache {
        private readonly ConcurrentDictionary<string, string> _cache; 

        public DefaultTemplateCache() {
            _cache = new ConcurrentDictionary<string, string>();
        }
        public string Get(string name) {
            string template;
            return _cache.TryGetValue(name, out template) ? template : null;
        }

        public void Set(string name, string template) {
            _cache.AddOrUpdate(name, template, (s, s1) => template);
        }
    }
}