//using System;
//using System.Web;
//using Orchard.DisplayManagement.Descriptors;
//using Orchard.DisplayManagement.Implementation;
//using Orchard.Environment;
//using Orchard.Mvc.ViewEngines.ThemeAwareness;

//namespace Orchard.Templates.Services {
//    public class TemplateBindingStrategy : IShapeTableProvider {
//        private readonly Work<ITemplateService> _templateService;
//        private readonly Work<ILayoutAwareViewEngine> _viewEngine;

//        public TemplateBindingStrategy(Work<ITemplateService> templateService, Work<ILayoutAwareViewEngine> viewEngine) {
//            _templateService = templateService;
//            _viewEngine = viewEngine;
//        }

//        public void Discover(ShapeTableBuilder builder) {
//            var templates = _templateService.Value.GetTemplates();

//            foreach (var template in templates) {
//                var shapeType = template.Name;
//                var bindingSource = String.Format("Template Content Item: {0}", shapeType);
//                builder.Describe(shapeType).BoundAs(bindingSource, shapeDescriptor => displayContext => RenderTemplate(displayContext, template.Body));
//            }
//        }

//        private IHtmlString RenderTemplate(DisplayContext displayContext, string template) {
//            return null;
//        }
//    }
//}