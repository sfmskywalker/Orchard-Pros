using System;
using System.Text.RegularExpressions;
using OrchardPros.Services.Content;

namespace OrchardPros.Providers.SummaryGenerators {
    public class PlainTextWordsSummaryGenerator : ISummaryGenerator {
        public string Generate(SummarizeContext context) {
            var ingress = Regex.Replace(context.Html, "<(.|\n)+?>", String.Empty, RegexOptions.IgnoreCase);
            ingress = Regex.Match(ingress, @"(((^\s*)*\S+\s+)|(\S+)){0," + context.BoundaryCount + "}").ToString().Trim() + context.Ellipses;

            return ingress;
        }
    }
}