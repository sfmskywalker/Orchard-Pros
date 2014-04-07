using Orchard;

namespace OrchardPros.Services.Content {
    public interface ISummarizer : IDependency {
        string Summarize(string text, string flavor, int boundaryCount = 20, string ellipses = "...");
        string Summarize(string text, string flavor, string generatorType, int boundaryCount = 20, string ellipses = "...");
        string Summarize(SummarizeContext context);
    }
}