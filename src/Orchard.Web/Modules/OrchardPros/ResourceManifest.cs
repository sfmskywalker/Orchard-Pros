using Orchard.UI.Resources;

namespace OrchardPros {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest
                .DefineScript("OrchardPros.AttachmentsUploader")
                .SetUrl("attachments-uploader.min.js", "attachments-uploader.js")
                .SetDependencies("jQuery", "jQueryFileUpload");
        }
    }
}