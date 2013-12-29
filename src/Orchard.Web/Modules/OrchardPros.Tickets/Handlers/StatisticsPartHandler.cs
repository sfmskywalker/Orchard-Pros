using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Handlers {
    public class StatisticsPartHandler : ContentHandler {
        public StatisticsPartHandler(IRepository<StatisticsPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}