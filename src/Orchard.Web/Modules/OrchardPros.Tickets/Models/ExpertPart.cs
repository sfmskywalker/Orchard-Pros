using Orchard.ContentManagement;

namespace OrchardPros.Tickets.Models {
    public class ExpertPart : ContentPart {
        public int Level {
            get { return this.Retrieve(x => x.Level); }
            set { this.Store(x => x.Level, value); }
        }

        public int ExperiencePoints {
            get { return this.Retrieve(x => x.ExperiencePoints); }
            set { this.Store(x => x.ExperiencePoints, value); }
        }
    }
}