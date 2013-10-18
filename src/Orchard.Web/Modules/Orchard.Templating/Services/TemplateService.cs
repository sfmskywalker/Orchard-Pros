using Orchard.DisplayManagement;

namespace Orchard.Templating.Services {
    public interface ITemplateService : IDependency {
        string ExecuteShape(string shapeType);
        string ExecuteShape(string shapeType, INamedEnumerable<object> parameters);
    }

    public class TemplateService : ITemplateService {
        private readonly IShapeFactory _shapeFactory;
        private readonly IDisplayHelperFactory _displayHelperFactory;
        private readonly ViewContextFactory _viewContextFactory;

        public TemplateService(IShapeFactory shapeFactory, IDisplayHelperFactory displayHelperFactory, ViewContextFactory viewContextFactory) {
            _shapeFactory = shapeFactory;
            _displayHelperFactory = displayHelperFactory;
            _viewContextFactory = viewContextFactory;
        }

        public string ExecuteShape(string shapeType) {
            return ExecuteShape(shapeType, null);
        }

        public string ExecuteShape(string shapeType, INamedEnumerable<object> parameters) {
            var shape = _shapeFactory.Create(shapeType, parameters);
            var viewContext = _viewContextFactory.Create();
            var display = _displayHelperFactory.CreateHelper(viewContext, null);
            return display(shape);
        }
    }
}