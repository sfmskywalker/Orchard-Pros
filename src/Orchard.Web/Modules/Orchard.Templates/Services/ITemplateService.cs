using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Templates.Models;

namespace Orchard.Templates.Services {
    public interface ITemplateService : IDependency {
        string ExecuteShape(string shapeType);
        string ExecuteShape(string shapeType, INamedEnumerable<object> parameters);
        string Parse<TModel>(string template, string language, Action<ITemplateViewBase<TModel>> activator, TModel model = default(TModel));
        string Parse(string template, string language, Action<ITemplateViewBase<dynamic>> activator, dynamic model = default(dynamic));
        IEnumerable<ShapePart> GetTemplates(VersionOptions versionOptions = null);
    }
}