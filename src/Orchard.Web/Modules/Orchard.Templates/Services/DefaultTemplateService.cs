using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Implementation;
using Orchard.Templates.Models;

namespace Orchard.Templates.Services {
    public interface ITemplateService : IDependency {
        IEnumerable<IParser> Parsers { get; }
        string ExecuteShape(string shapeType);
        string ExecuteShape(string shapeType, INamedEnumerable<object> parameters);
        string ParseTemplate<TModel>(string template, string language, TModel model = default(TModel));
        string ParseTemplate(string template, string language, dynamic model = default(dynamic));
        IEnumerable<ShapePart> GetTemplates(VersionOptions versionOptions = null);
    }

    public class DefaultTemplateService : ITemplateService {
        private readonly IShapeFactory _shapeFactory;
        private readonly IDisplayHelperFactory _displayHelperFactory;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<IParser> _parsers;

        public DefaultTemplateService(
            IShapeFactory shapeFactory, 
            IDisplayHelperFactory displayHelperFactory, 
            IWorkContextAccessor workContextAccessor, 
            IContentManager contentManager, 
            IEnumerable<IParser> parsers) {
            _shapeFactory = shapeFactory;
            _displayHelperFactory = displayHelperFactory;
            _workContextAccessor = workContextAccessor;
            _contentManager = contentManager;
            _parsers = parsers;
        }

        public IEnumerable<IParser> Parsers {
            get { return _parsers; }
        }

        public string ExecuteShape(string shapeType) {
            return ExecuteShape(shapeType, null);
        }

        public string ExecuteShape(string shapeType, INamedEnumerable<object> parameters) {
            var shape = _shapeFactory.Create(shapeType, parameters);
            var display = _displayHelperFactory.CreateHelper(new ViewContext { HttpContext = _workContextAccessor.GetContext().HttpContext }, new ViewDataContainer());
            var result = ((DisplayHelper)display).ShapeExecute(shape).ToString();
            return result;
        }

        public string ParseTemplate<TModel>(string template, string language, TModel model = default(TModel)) {
            var parser = _parsers.Single(x => String.Equals(x.Language, language, StringComparison.OrdinalIgnoreCase));
            return parser.Parse(template, model);
        }

        public string ParseTemplate(string template, string language, dynamic model = null) {
            return ParseTemplate<dynamic>(template, language, model);
        }

        public IEnumerable<ShapePart> GetTemplates(VersionOptions versionOptions = null) {
            return _contentManager.Query<ShapePart>(versionOptions ?? VersionOptions.Published).List().ToList();
        }

        private class ViewDataContainer : IViewDataContainer {
            public ViewDataDictionary ViewData { get; set; }

            public ViewDataContainer() {
                ViewData = new ViewDataDictionary();
            }
        }
    }
}