using System.Linq;
using HtmlAgilityPack;
using OrchardPros.Helpers;
using OrchardPros.Services.Content;

namespace OrchardPros.Providers.SummaryGenerators {
    public class ParagraphsSummaryGenerator : ISummaryGenerator {
        public string Generate(SummarizeContext context) {
            var document = new HtmlDocument();
            var html = context.Html.SanitizeHtml(context.HtmlWhiteList);
            document.LoadHtml(html);
            var paragraphs = document.DocumentNode.Elements("p").Take(context.BoundaryCount);
            var htmlExcerpt = string.Join("\r\n", paragraphs.Select(p => p.OuterHtml));
            return htmlExcerpt;
        }
    }
}