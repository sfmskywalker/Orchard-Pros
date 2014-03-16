using System;
using Orchard.ContentManagement.Records;

namespace OrchardPros.Models {
    public class UserProfilePartRecord : ContentPartRecord {
        public virtual Country Country { get; set; }
        public virtual int ExperiencePoints { get; set; }
        public virtual int ActivityPoints { get; set; }
        public virtual DateTime? CreatedUtc { get; set; }
    }
}