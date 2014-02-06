using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using OrchardPros.Models;

namespace OrchardPros.Drivers {
    public class StripeSettingsPartDriver : ContentPartDriver<StripeSettingsPart> {
        protected override DriverResult Editor(StripeSettingsPart part, dynamic shapeHelper) {
            return Editor(part, null, shapeHelper);
        }

        protected override DriverResult Editor(StripeSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            return ContentShape("Parts_StripeSettings_Edit", () => {
                if (updater != null) {
                    updater.TryUpdateModel(part, Prefix, null, new[] {"SecretKey", "PublishableKey"});
                }

                return shapeHelper.EditorTemplate(TemplateName: "Parts/StripeSettings", Model: part, Prefix: Prefix);
            }).OnGroup("Stripe");
        }
    }
}