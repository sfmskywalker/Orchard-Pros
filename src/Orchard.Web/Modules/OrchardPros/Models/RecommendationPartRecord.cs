using Orchard.ContentManagement.Records;

namespace OrchardPros.Models {
    public class RecommendationPartRecord : ContentPartRecord {
        public virtual int RecommendedUserId { get; set; }
    }
}