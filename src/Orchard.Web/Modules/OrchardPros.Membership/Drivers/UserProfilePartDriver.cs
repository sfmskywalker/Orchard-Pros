using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using OrchardPros.Membership.Models;

namespace OrchardPros.Membership.Drivers {
    public class UserProfilePartDriver : ContentPartDriver<UserProfilePart> {
        protected override DriverResult Display(UserProfilePart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_UserProfile", () => shapeHelper.Parts_UserProfile());
        }

        protected override DriverResult Editor(UserProfilePart part, dynamic shapeHelper) {
            return ContentShape("Parts_UserProfile_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/UserProfile", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(UserProfilePart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}