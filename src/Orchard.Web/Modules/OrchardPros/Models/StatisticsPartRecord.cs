using Orchard.ContentManagement.Records;

namespace OrchardPros.Models {
    public class StatisticsPartRecord : ContentPartRecord {
        public virtual int ViewCount { get; set; }
    }
}