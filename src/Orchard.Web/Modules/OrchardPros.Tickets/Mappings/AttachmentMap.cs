using FluentNHibernate.Mapping;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Mappings {
    public class AttachmentMap : ClassMap<Attachment> {
        public AttachmentMap() {
            References(x => x.Ticket, "TicketId");
        }
    }
}