using Orchard.ContentManagement.Records;

namespace OrchardPros.Models {
    public class RecommendationPartRecord : ContentPartRecord {
        public virtual int UserId { get; set; }
    }
}