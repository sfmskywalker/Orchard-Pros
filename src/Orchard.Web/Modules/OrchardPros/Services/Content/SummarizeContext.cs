namespace OrchardPros.Services.Content {
    public class SummarizeContext {
        public string Text { get; set; }
        public string Flavor { get; set; }
        public string Html { get; set; }
        public int BoundaryCount { get; set; }
        public string Ellipses { get; set; }
        public string[] HtmlWhiteList { get; set; }
        public string GeneratorType { get; set; }

        public SummarizeContext() {
            BoundaryCount = 20;
            Ellipses = "...";
            GeneratorType = Providers.SummaryGenerators.SummaryGenerators.Characters;
            HtmlWhiteList = new[] { "b", "strong", "p", "em", "i", "a" };
        }
    }
}