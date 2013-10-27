using System;
using System.Web;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Implementation;
using Orchard.Environment;
using Orchard.Templates.Models;

namespace Orchard.Templates.Services {
    public class TemplateBindingStrategy : IShapeTableProvider {
        private readonly Work<ITemplateService> _templateService;

        public TemplateBindingStrategy(Work<ITemplateService> templateService) {
            _templateService = templateService;
        }

        public void Discover(ShapeTableBuilder builder) {
            var templates = _templateService.Value.GetTemplates();

            foreach (var template in templates) {
                var shapeType = template.Name;
                var bindingSource = String.Format("Template Content Item: {0}", shapeType);
                builder.Describe(shapeType).BoundAs(bindingSource, shapeDescriptor => displayContext => RenderTemplate(displayContext, template));
            }
        }

        private IHtmlString RenderTemplate(DisplayContext displayContext, ShapePart template) {
            return new HtmlString(_templateService.Value.ParseTemplate(template.Template, template.Language));
        }
    }
}