using Microsoft.ApplicationServer.Caching;
using NHibernate.Cache;
using Orchard.Azure.Services.Environment.Configuration;
using System;
using System.Collections.Generic;

namespace Orchard.Azure.Services.Caching.Database {

    public class AzureCacheProvider : ICacheProvider {

        private DataCache _dataCache;
        private readonly IPlatformConfigurationAccessor _pca;

        public AzureCacheProvider(IPlatformConfigurationAccessor pca) {
            _pca = pca;
        }

        public ICache BuildCache(string regionName, IDictionary<string, string> properties) {
            
            if (_dataCache == null) {
                throw new InvalidOperationException("Can't call this method when provider is in stopped state.");
            }
            
            TimeSpan? expiration = null;
            string expirationString;
            if (properties.TryGetValue(NHibernate.Cfg.Environment.CacheDefaultExpiration, out expirationString) || properties.TryGetValue("cache.default_expiration", out expirationString)) {
                expiration = TimeSpan.FromSeconds(Int32.Parse(expirationString));
            }

            return new AzureCacheClient(_dataCache, regionName, expiration);
        }

        public long NextTimestamp() {
            return Timestamper.Next();
        }

        public void Start(IDictionary<string, string> properties) {
            CacheClientConfiguration configuration;

            try {
                var tenantName = properties["cache.region_prefix"];

                bool enableCompression = false;
                string enableCompressionString;
                if (properties.TryGetValue("compression_enabled", out enableCompressionString))
                    enableCompression = Boolean.Parse(enableCompressionString);

                configuration = CacheClientConfiguration.FromPlatformConfiguration(tenantName, Constants.DatabaseCacheSettingNamePrefix, _pca);
                configuration.CompressionIsEnabled = enableCompression;
                configuration.Validate();
            }
            catch (Exception ex) {
                throw new Exception(String.Format("The {0} configuration settings are missing or invalid.", Constants.DatabaseCacheFeatureName), ex);
            }

            _dataCache = configuration.CreateCache();
        }

        public void Stop() {
            _dataCache = null;
        }
    }
}