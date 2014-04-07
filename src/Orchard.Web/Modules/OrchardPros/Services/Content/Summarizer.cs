using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Services;

namespace OrchardPros.Services.Content {
    public class Summarizer : ISummarizer {
        public Summarizer(IEnumerable<ISummaryGenerator> generators, IEnumerable<IHtmlFilter> filters) {
            _generators = generators;
            _filters = filters;
        }

        private readonly IEnumerable<ISummaryGenerator> _generators;
        private readonly IEnumerable<IHtmlFilter> _filters;

        public string Summarize(string text, string flavor, int boundaryCount = 20, string ellipses = "...") {
            return Summarize(text, flavor, Providers.SummaryGenerators.SummaryGenerators.Characters, boundaryCount, ellipses);
        }

        public string Summarize(string text, string flavor, string generatorType, int boundaryCount = 20, string ellipses = "...") {
            return Summarize(new SummarizeContext {
                Text = text,
                Flavor = flavor,
                BoundaryCount = boundaryCount,
                Ellipses = ellipses,
                GeneratorType = generatorType
            });
        }

        public string Summarize(SummarizeContext context) {
            var flavor = context.Flavor;
            var text = context.Text;
            var generatorType = !String.IsNullOrWhiteSpace(context.GeneratorType) ? context.GeneratorType : Providers.SummaryGenerators.SummaryGenerators.Characters;
            var html = _filters.Where(x => x.GetType().Name.Equals(flavor + "filter", StringComparison.OrdinalIgnoreCase)).Aggregate(text, (t, filter) => filter.ProcessContent(t, flavor));

            context.Html = html;
            var generator = _generators.FirstOrDefault(p => p.GetType().Name == generatorType);

            if (generator == null)
                throw new InvalidOperationException(String.Format("No summary generator of type {0} was found.", generatorType));

            return generator.Generate(context);
        }
    }
}