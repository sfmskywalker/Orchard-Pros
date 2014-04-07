using System.Web.Mvc;
using Orchard;
using OrchardPros.Services.Content;

namespace OrchardPros.Helpers {
    public static class SummaryHtmlHelper {
        public static string Summarize(this HtmlHelper htmlHelper, string text, string flavor, int boundaryCount = 20, string ellipses = "...") {
            var summarizer = htmlHelper.ViewContext.GetWorkContext().Resolve<ISummarizer>();
            return summarizer.Summarize(text, flavor, boundaryCount, ellipses);
        }

        public static string Summarize(this HtmlHelper htmlHelper, string text, string flavor, string generatorType, int boundaryCount = 20, string ellipses = "...") {
            var summarizer = htmlHelper.ViewContext.GetWorkContext().Resolve<ISummarizer>();
            return summarizer.Summarize(text, flavor, generatorType, boundaryCount, ellipses);
        }

        public static string Summarize(this HtmlHelper htmlHelper, SummarizeContext context) {
            var summarizer = htmlHelper.ViewContext.GetWorkContext().Resolve<ISummarizer>();
            return summarizer.Summarize(context);
        }
    }
}