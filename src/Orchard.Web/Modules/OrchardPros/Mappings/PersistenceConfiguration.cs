﻿using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using Orchard.Data;
using Orchard.Utility;
using OrchardPros.Models;

namespace OrchardPros.Mappings {
    public class PersistenceConfiguration : ISessionConfigurationEvents {
        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel) {
            defaultModel.Override<Experience>(mapping => mapping.References(x => x.Position, "PositionId"));
            defaultModel.Override<SubscriptionSourcePartRecord>(mapping => mapping.HasMany(x => x.Subscriptions).KeyColumn("SubscriptionSourceId"));
            defaultModel.Override<UserProfilePartRecord>(mapping => mapping.References(x => x.Country, "CountryId").Fetch.Join().Not.LazyLoad());
        }

        public void Building(Configuration cfg) { }
        public void Prepared(FluentConfiguration cfg) { }
        public void Finished(Configuration cfg) {}

        public void ComputingHash(Hash hash) {
            hash.AddStringInvariant("42FD7BC1-0EA3-41DE-87CE-078DDCE4B4985");
        }
    }
}