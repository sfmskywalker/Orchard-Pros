using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using OrchardPros.Models;

namespace OrchardPros.Handlers {
    public class StatisticsPartHandler : ContentHandler {
        public StatisticsPartHandler(IRepository<StatisticsPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
            OnGetDisplayShape<StatisticsPart>(UpdateStatistics);
        }

        private void UpdateStatistics(BuildDisplayContext context, StatisticsPart part) {
            part.ViewCount++;
        }
    }
}