using System;

namespace Orchard.Templates.Services {
    public abstract class TemplateParserImpl : ITemplateParser {
        public abstract string Type { get; }
        public abstract string Parse<TModel>(string template, Action<ITemplateViewBase<TModel>> activator, TModel model = default(TModel));

        public virtual string Parse(string template, Action<ITemplateViewBase<dynamic>> activator, dynamic model = null) {
            return Parse<dynamic>(template, activator, model);
        }
    }
}