using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Templates.Models;

namespace Orchard.Templates.Drivers {
    public class ShapePartDriver : ContentPartDriver<ShapePart> {
        protected override DriverResult Editor(ShapePart part, dynamic shapeHelper) {
            return Editor(part, null, shapeHelper);
        }

        protected override DriverResult Editor(ShapePart part, IUpdateModel updater, dynamic shapeHelper) {
            if (updater != null) {
                updater.TryUpdateModel(part, Prefix, null, null);
            }
            return ContentShape("Parts_Shape_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts.Shape", Model: part, Prefix: Prefix));
        }
    }
}