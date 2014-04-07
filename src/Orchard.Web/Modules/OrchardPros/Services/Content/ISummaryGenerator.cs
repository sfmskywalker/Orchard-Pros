using Orchard;

namespace OrchardPros.Services.Content {
    public interface ISummaryGenerator : IDependency {
        string Generate(SummarizeContext context);
    }
}