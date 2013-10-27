using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Implementation;
using Orchard.Templates.Models;

namespace Orchard.Templates.Services {
    public class DefaultTemplateService : ITemplateService {
        private readonly IShapeFactory _shapeFactory;
        private readonly IDisplayHelperFactory _displayHelperFactory;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<ITemplateParser> _parsers;

        public DefaultTemplateService(
            IShapeFactory shapeFactory, 
            IDisplayHelperFactory displayHelperFactory, 
            IWorkContextAccessor workContextAccessor, 
            IContentManager contentManager, 
            IEnumerable<ITemplateParser> parsers) {
            _shapeFactory = shapeFactory;
            _displayHelperFactory = displayHelperFactory;
            _workContextAccessor = workContextAccessor;
            _contentManager = contentManager;
            _parsers = parsers;
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

        public string Parse<TModel>(string template, string language, Action<ITemplateViewBase<TModel>> activator, TModel model = default(TModel))
        {
            var parser = _parsers.Single(x => String.Equals(x.Type, language, StringComparison.OrdinalIgnoreCase));
            return parser.Parse(template, activator, model);
        }

        public string Parse(string template, string language, Action<ITemplateViewBase<dynamic>> activator, dynamic model = null)
        {
            return Parse<dynamic>(template, language, activator, model);
        }

        public IEnumerable<ShapePart> GetTemplates(VersionOptions versionOptions = null) {
            return _contentManager.Query<ShapePart>(versionOptions ?? VersionOptions.Published).List();
        }

        private class ViewDataContainer : IViewDataContainer {
            public ViewDataDictionary ViewData { get; set; }

            public ViewDataContainer() {
                ViewData = new ViewDataDictionary();
            }
        }
    }
}