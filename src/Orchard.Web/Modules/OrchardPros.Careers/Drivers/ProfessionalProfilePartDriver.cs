using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.Drivers {
    public class ProfessionalProfilePartDriver : ContentPartDriver<ProfessionalProfilePart> {
        protected override DriverResult Display(ProfessionalProfilePart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_ProfessionalProfile", () => shapeHelper.Parts_ProfessionalProfile);
        }

        protected override DriverResult Editor(ProfessionalProfilePart part, dynamic shapeHelper) {
            return ContentShape("Parts_ProfessionalProfile_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/ProfessionalProfile", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(ProfessionalProfilePart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}