using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using Orchard.Data;
using Orchard.Utility;

namespace OrchardPros.Tickets.Mappings {
    public class PersistenceConfiguration : ISessionConfigurationEvents {
        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel) {}
        public void Building(Configuration cfg) { }
        public void Prepared(FluentConfiguration cfg) { }
        public void Finished(Configuration cfg) {}

        public void ComputingHash(Hash hash) {
            hash.AddStringInvariant("52FD7BC1-0EA3-40DE-87CE-078DDCE4B4985");
        }
    }
}