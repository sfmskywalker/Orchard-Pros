using FluentNHibernate.Mapping;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Mappings {
    public class ReplyMap : ClassMap<Reply> {
        public ReplyMap() {
            References(x => x.ParentReply, "ParentReplyId");
            References(x => x.Ticket, "TicketId");
        }
    }
}