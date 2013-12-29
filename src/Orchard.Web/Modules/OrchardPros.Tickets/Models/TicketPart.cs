using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Common.Utilities;
using Orchard.Core.Title.Models;
using Orchard.Security;

namespace OrchardPros.Tickets.Models {
    public class TicketPart: ContentPart<TicketPartRecord> {
        internal LazyField<IEnumerable<ReplyPart>> RepliesField = new LazyField<IEnumerable<ReplyPart>>();

        public TicketType Type {
            get { return Retrieve(x => x.Type); }
            set { Store(x => x.Type, value); }
        }

        public decimal? Bounty {
            get { return Retrieve(x => x.Bounty); }
            set { Store(x => x.Bounty, value); }
        }

        public DateTime DeadlineUtc {
            get { return Retrieve(x => x.DeadlineUtc); }
            set { Store(x => x.DeadlineUtc, value); }
        }

        public int ExperiencePoints {
            get { return Retrieve(x => x.ExperiencePoints); }
            set { Store(x => x.ExperiencePoints, value); }
        }

        public DateTime? SolvedUtc {
            get { return Retrieve(x => x.SolvedUtc); }
            set { Store(x => x.SolvedUtc, value); }
        }

        public int? AnswerId {
            get { return Retrieve(x => x.AnswerId); }
            set { Store(x => x.AnswerId, value); }
        }

        public string Subject {
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

        public IEnumerable<ReplyPart> Replies {
            get { return RepliesField.Value; }
        }
    }
}