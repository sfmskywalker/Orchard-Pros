using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Templates.Helpers;
using Orchard.Templates.Models;
using Orchard.Templates.Services;
using Orchard.Templates.ViewModels;

namespace Orchard.Templates.Drivers {
    public class ShapePartDriver : ContentPartDriver<ShapePart> {
        private readonly ITemplateService _templateService;

        public ShapePartDriver(ITemplateService templateService) {
            _templateService = templateService;
        }

        protected override DriverResult Editor(ShapePart part, dynamic shapeHelper) {
            return Editor(part, null, shapeHelper);
        }

        protected override DriverResult Editor(ShapePart part, IUpdateModel updater, dynamic shapeHelper) {
            var viewModel = new ShapePartViewModel {
                Name = part.Name,
                Template = part.Template,
                Language = part.Language,
                AvailableLanguages = _templateService.Parsers.Select(x => x.Language).ToArray()
            };
            if (updater != null) {
                if (updater.TryUpdateModel(viewModel, Prefix, null, new[] { "AvailableLanguages" })) {
                    part.Name = viewModel.Name.TrimSafe();
                    part.Language = viewModel.Language;
                    part.Template = viewModel.Template;
                }
            }
            return ContentShape("Parts_Shape_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts.Shape", Model: part, Prefix: Prefix));
        }
    }
}