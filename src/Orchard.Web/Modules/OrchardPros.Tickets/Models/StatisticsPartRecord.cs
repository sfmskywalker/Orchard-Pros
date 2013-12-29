using Orchard.ContentManagement.Records;

namespace OrchardPros.Tickets.Models {
    public class StatisticsPartRecord : ContentPartRecord {
        public virtual int ViewCount { get; set; }
    }
}