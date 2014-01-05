using Orchard.Environment.Extensions;
using Orchard.UI.Resources;

namespace OrchardPros.ResourceManifests {
    [OrchardFeature("MarkdownDeep")]
    public class MarkdownDeep : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineScript("MarkdownDeepLib").SetUrl("MarkdownDeepLib.min.js").SetDependencies("jQuery");
            manifest.DefineScript("MarkdownDeep").SetUrl("MarkdownDeep.js").SetDependencies("MarkdownDeepLib");
            manifest.DefineStyle("MarkdownDeep").SetUrl("mdd_styles.css");
        }
    }
}