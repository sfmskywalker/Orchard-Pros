using System;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Common.Utilities;
using Orchard.Security;

namespace OrchardPros.Careers.Models {
    public class RecommendationPart : ContentPart<RecommendationPartRecord> {
        internal LazyField<IUser> UserField = new LazyField<IUser>();

        public int UserId {
            get { return Retrieve(x => x.UserId); }
            set { Store(x => x.UserId, value); }
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

        public bool Approved {
            get { return this.Retrieve(x => x.Approved); }
            set { this.Store(x => x.Approved, value); }
        }

        public DateTime? ApprovedUtc {
            get { return this.Retrieve(x => x.ApprovedUtc); }
            set { this.Store(x => x.ApprovedUtc, value); }
        }

        public bool AllowPublication {
            get { return this.Retrieve(x => x.AllowPublication); }
            set { this.Store(x => x.AllowPublication, value); }
        }
    }
}