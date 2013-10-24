using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Templates.Models;

namespace Orchard.Templates.Drivers {
    public class TemplatePartDriver : ContentPartDriver<TemplatePart> {
        protected override DriverResult Editor(TemplatePart part, dynamic shapeHelper) {
            return Editor(part, null, shapeHelper);
        }

        protected override DriverResult Editor(TemplatePart part, IUpdateModel updater, dynamic shapeHelper) {
            if (updater != null) {
                updater.TryUpdateModel(part, Prefix, null, null);
            }
            return ContentShape("Parts_Template_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts.Template", Model: part, Prefix: Prefix));
        }
    }
}