using Orchard.ContentManagement.Records;

namespace OrchardPros.Careers.Models {
    public class RecommendationPartRecord : ContentPartRecord {
        public virtual int UserId { get; set; }
    }
}