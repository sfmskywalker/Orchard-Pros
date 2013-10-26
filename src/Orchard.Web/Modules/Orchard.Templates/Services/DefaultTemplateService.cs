using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Implementation;
using Orchard.Templates.Models;

namespace Orchard.Templates.Services {
    public interface ITemplateService : IDependency {
        string ExecuteShape(string shapeType);
        string ExecuteShape(string shapeType, INamedEnumerable<object> parameters);
        IEnumerable<ShapePart> GetTemplates(VersionOptions versionOptions = null);
    }

    public class DefaultTemplateService : ITemplateService {
        private readonly IShapeFactory _shapeFactory;
        private readonly IDisplayHelperFactory _displayHelperFactory;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IContentManager _contentManager;

        public DefaultTemplateService(IShapeFactory shapeFactory, IDisplayHelperFactory displayHelperFactory, IWorkContextAccessor workContextAccessor, IContentManager contentManager) {
            _shapeFactory = shapeFactory;
            _displayHelperFactory = displayHelperFactory;
            _workContextAccessor = workContextAccessor;
            _contentManager = contentManager;
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