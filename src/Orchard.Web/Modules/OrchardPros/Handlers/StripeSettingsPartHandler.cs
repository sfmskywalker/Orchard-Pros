using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using OrchardPros.Models;

namespace OrchardPros.Handlers {
    public class StripeSettingsPartHandler : ContentHandler {
        public StripeSettingsPartHandler() {
            T = NullLocalizer.Instance;
            OnGetContentItemMetadata<StripeSettingsPart>((context, part) => context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Stripe"))));
        }

        public Localizer T { get; set; }
    }
}