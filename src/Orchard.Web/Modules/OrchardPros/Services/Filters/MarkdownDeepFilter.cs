using System;
using Orchard.Services;

namespace OrchardPros.Services.Filters {
    public class MarkdownDeepFilter : IHtmlFilter {
        public string ProcessContent(string text, string flavor) {
            return String.Equals(flavor, "markdowndeep", StringComparison.OrdinalIgnoreCase) ? MarkdownReplace(text) : text;
        }

        private static string MarkdownReplace(string text) {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var markdown = new MarkdownDeep.Markdown();
            return markdown.Transform(text);
        }
    }
}