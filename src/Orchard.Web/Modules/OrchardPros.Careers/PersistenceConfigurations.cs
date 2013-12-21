using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using Orchard.Data;
using Orchard.Utility;
using OrchardPros.Careers.Models;
using Position = OrchardPros.Careers.Models.Position;

namespace OrchardPros.Careers {
    public class PersistenceConfigurations : ISessionConfigurationEvents {
        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel) {
            defaultModel.Override<Experience>(mapping => mapping
                .References(x => x.Position, "PositionId"));
        }

        public void Prepared(FluentConfiguration cfg) {}
        public void Building(Configuration cfg) {}
        public void Finished(Configuration cfg) {}

        public void ComputingHash(Hash hash) {
            hash.AddTypeReference(typeof(Experience));
            hash.AddTypeReference(typeof(Position));
        }
    }
}