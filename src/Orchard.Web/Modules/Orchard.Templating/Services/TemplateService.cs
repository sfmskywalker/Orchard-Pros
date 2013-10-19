using System.Web.Mvc;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Implementation;

namespace Orchard.Templating.Services {
    public interface ITemplateService : IDependency {
        string ExecuteShape(string shapeType);
        string ExecuteShape(string shapeType, INamedEnumerable<object> parameters);
    }

    public class TemplateService : ITemplateService {
        private readonly IShapeFactory _shapeFactory;
        private readonly IDisplayHelperFactory _displayHelperFactory;
        private readonly IWorkContextAccessor _workContextAccessor;

        public TemplateService(IShapeFactory shapeFactory, IDisplayHelperFactory displayHelperFactory, IWorkContextAccessor workContextAccessor) {
            _shapeFactory = shapeFactory;
            _displayHelperFactory = displayHelperFactory;
            _workContextAccessor = workContextAccessor;
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

        private class ViewDataContainer : IViewDataContainer {
            public ViewDataDictionary ViewData { get; set; }

            public ViewDataContainer() {
                ViewData = new ViewDataDictionary();
            }
        }
    }
}