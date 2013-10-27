using System;

namespace Orchard.Templates.Services {
    public interface ITemplateParser : IDependency {
        string Type { get; }
        string Parse<TModel>(string template, Action<ITemplateViewBase<TModel>> activator, TModel model = default(TModel));
        string Parse(string template, Action<ITemplateViewBase<dynamic>> activator, dynamic model = default(dynamic));
    }
}