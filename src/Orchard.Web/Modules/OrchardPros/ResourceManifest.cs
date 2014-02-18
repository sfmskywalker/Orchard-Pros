using Orchard.UI.Resources;

namespace OrchardPros {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineStyle("Stripe").SetUrl("stripe.css");
        }
    }
}
