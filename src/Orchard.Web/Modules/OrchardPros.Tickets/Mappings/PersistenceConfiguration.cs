using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using Orchard.Data;
using Orchard.Utility;

namespace OrchardPros.Tickets.Mappings {
    public class PersistenceConfiguration : ISessionConfigurationEvents {
        public void Building(Configuration cfg) {}

        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel) {
            cfg.Mappings(x => x.FluentMappings
                .Add<VoteMap>()
                .Add<ReplyMap>()
                .Add<AttachmentMap>());
        }

        public void Prepared(FluentConfiguration cfg) {}
        public void Finished(Configuration cfg) {}
        public void ComputingHash(Hash hash) {}
    }
}