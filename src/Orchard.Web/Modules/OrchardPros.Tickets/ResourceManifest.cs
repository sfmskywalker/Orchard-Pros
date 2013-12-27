using Orchard.UI.Resources;

namespace OrchardPros.Tickets {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest
                .DefineScript("OrchardPros.Tickets.CreateTicket")
                .SetUrl("ticket-create.min.js", "ticket-create.js")
                .SetDependencies("jQuery", "jQueryFileUpload");
        }
    }
}