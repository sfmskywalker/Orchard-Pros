using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Drivers {
    public class ExpertPartDriver : ContentPartDriver<ExpertPart> {
        protected override DriverResult Editor(ExpertPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Expert_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Expert", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(ExpertPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}