namespace Orchard.Templates.Services {
    public interface IParser : IDependency {
        string Language { get; }
        string Parse<TModel>(string template, TModel model = default(TModel));
        string Parse(string template, dynamic model = default(dynamic));
    }

    public abstract class ParserBase : IParser {
        public abstract string Language { get; }
        public abstract string Parse<TModel>(string template, TModel model = default(TModel));

        public virtual string Parse(string template, dynamic model = null) {
            return Parse<dynamic>(template, model);
        }
    }
}