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
        private readonly IEnumerable<ITemplateProcessor> _processors;

        public DefaultTemplateService(
            IShapeFactory shapeFactory, 
            IDisplayHelperFactory displayHelperFactory, 
            IWorkContextAccessor workContextAccessor, 
            IContentManager contentManager, 
            IEnumerable<ITemplateProcessor> processors) {
            _shapeFactory = shapeFactory;
            _displayHelperFactory = displayHelperFactory;
            _workContextAccessor = workContextAccessor;
            _contentManager = contentManager;
            _processors = processors;
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

        public string Execute<TModel>(string template, string language, TModel model = default(TModel)) {
            return Execute(template, language, null, model);
        }

        public string Execute<TModel>(string template, string language, DisplayContext context, TModel model = default(TModel)) {
            var processor = _processors.FirstOrDefault(x => String.Equals(x.Type, language, StringComparison.OrdinalIgnoreCase));
            return processor != null ? processor.Process(template, context, model) : string.Empty;
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