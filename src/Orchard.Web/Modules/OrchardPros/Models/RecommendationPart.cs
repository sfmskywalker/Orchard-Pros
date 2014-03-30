using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Common.Utilities;
using Orchard.Security;

namespace OrchardPros.Models {
    public class RecommendationPart : ContentPart<RecommendationPartRecord> {
        internal LazyField<IUser> UserField = new LazyField<IUser>();

        public int RecommendedUserId {
            get { return Retrieve(x => x.RecommendedUserId); }
            set { Store(x => x.RecommendedUserId, value); }
        }

        public IUser User {
            get { return UserField.Value; }
            set { UserField.Value = value; }
        }

        public IUser RecommendingUser {
            get { return this.As<CommonPart>().Owner; }
            set { this.As<CommonPart>().Owner = value; }
        }

        public string Body {
            get { return this.As<BodyPart>().Text; }
            set { this.As<BodyPart>().Text = value; }
        }
    }
}