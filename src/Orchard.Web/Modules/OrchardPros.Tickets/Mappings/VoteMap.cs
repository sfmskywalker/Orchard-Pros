using FluentNHibernate.Mapping;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Mappings {
    public class VoteMap : ClassMap<Vote> {
        public VoteMap() {
            References(x => x.Reply, "ReplyId");
        }
    }
}