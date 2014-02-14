using Orchard.ContentManagement;

namespace OrchardPros.Models {
    public class StripeSettingsPart : ContentPart {
        public string EndpointUrl {
            get { return this.Retrieve(x => x.EndpointUrl, "https://api.stripe.com/"); }
            set { this.Store(x => x.EndpointUrl, value); }
        }

        public string LiveSecretKey {
            get { return this.Retrieve(x => x.LiveSecretKey); }
            set { this.Store(x => x.LiveSecretKey, value); }
        }

        public string TestSecretKey {
            get { return this.Retrieve(x => x.TestSecretKey); }
            set { this.Store(x => x.TestSecretKey, value); }
        }
        public string LivePublishableKey {
            get { return this.Retrieve(x => x.LivePublishableKey); }
            set { this.Store(x => x.LivePublishableKey, value); }
        }

        public string TestPublishableKey {
            get { return this.Retrieve(x => x.TestPublishableKey); }
            set { this.Store(x => x.TestPublishableKey, value); }
        }

        public string TestClientId {
            get { return this.Retrieve(x => x.TestClientId); }
            set { this.Store(x => x.TestClientId, value); }
        }

        public string LiveClientId {
            get { return this.Retrieve(x => x.LiveClientId); }
            set { this.Store(x => x.LiveClientId, value); }
        }

        public bool Live {
            get { return this.Retrieve(x => x.Live); }
            set { this.Store(x => x.Live, value); }
        }

        public string SecretKey {
            get { return Live ? LiveSecretKey : TestSecretKey; }
        }

        public string PublishableKey {
            get { return Live ? LivePublishableKey : TestPublishableKey; }
        }

        public string ClientId {
            get { return Live ? LiveClientId : TestClientId; }
        }
    }
}