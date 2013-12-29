using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Security;

namespace OrchardPros.Tickets.Models {
    public class ReplyPart : ContentPart {
        public string Subject{
            get { return this.As<TitlePart>().Title; }
            set { this.As<TitlePart>().Title = value; }
        }

        public string Body {
            get { return this.As<BodyPart>().Text; }
            set { this.As<BodyPart>().Text = value; }
        }

        public IUser User {
            get { return this.As<CommonPart>().Record.OwnerId; }
            set { this.As<TitlePart>().Title = value; }
        }
    }
}