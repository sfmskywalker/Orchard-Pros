﻿using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using Orchard.Data;
using Orchard.Utility;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Mappings {
    public class PersistenceConfiguration : ISessionConfigurationEvents {

        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel) {
            defaultModel.Override<Ticket>(mapping => {
                mapping.HasMany(x => x.Categories).KeyColumn("TicketId");
                mapping.HasMany(x => x.Tags).KeyColumn("TicketId");
                mapping.HasMany(x => x.Attachments).KeyColumn("TicketId");
                mapping.HasMany(x => x.Replies).KeyColumn("TicketId");
            });
            defaultModel.Override<TicketCategory>(mapping => mapping.References(x => x.Ticket, "TicketId"));
            defaultModel.Override<TicketTag>(mapping => mapping.References(x => x.Ticket, "TicketId"));
            defaultModel.Override<Vote>(mapping => mapping.References(x => x.Reply, "VoteId"));
            defaultModel.Override<Reply>(mapping => {
                mapping.References(x => x.Ticket, "TicketId");
                mapping.References(x => x.ParentReply, "ParentReplyId");
            });
            defaultModel.Override<Attachment>(mapping => mapping.References(x => x.Ticket, "TicketId"));
        }

        public void Building(Configuration cfg) { }
        public void Prepared(FluentConfiguration cfg) { }
        public void Finished(Configuration cfg) {}

        public void ComputingHash(Hash hash) {
            hash.AddStringInvariant("52FD7BC1-0EA3-40DE-87CE-078DDCE4B4984");
        }
    }
}