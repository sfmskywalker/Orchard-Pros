using Orchard.ContentManagement.Records;

namespace OrchardPros.Models {
    public class UserProfilePartRecord : ContentPartRecord {
        public virtual Country Country { get; set; }
    }
}