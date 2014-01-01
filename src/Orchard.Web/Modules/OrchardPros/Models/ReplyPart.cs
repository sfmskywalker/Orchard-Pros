using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Security;

namespace OrchardPros.Models {
    public class ReplyPart : ContentPart {
        public IContent ContainingContent {
            get { return this.As<CommonPart>().Container; }
            set { this.As<CommonPart>().Container = value; }
        }

        public string Subject{
            get { return this.As<TitlePart>().Title; }
            set { this.As<TitlePart>().Title = value; }
        }

        public string Body {
            get { return this.As<BodyPart>().Text; }
            set { this.As<BodyPart>().Text = value; }
        }

        public IUser User {
            get { return this.As<CommonPart>().Owner; }
            set { this.As<CommonPart>().Owner = value; }
        }

        public int? ParentReplyId {
            get { return this.Retrieve(x => x.ParentReplyId); }
            set { this.Store(x => x.ParentReplyId, value); }
        }
    }
}