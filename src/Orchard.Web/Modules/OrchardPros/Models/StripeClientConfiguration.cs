namespace OrchardPros.Models {
    public class StripeClientConfiguration {
        public string EndpointUrl {
            get { return "https://api.stripe.com/"; }
        }

        public string LiveSecretKey { get; set; }
        public string TestSecretKey { get; set; }
        public string LivePublishableKey { get; set; }
        public string TestPublishableKey { get; set; }
        public bool Live { get; set; }

        public string SecretKey {
            get { return Live ? LiveSecretKey : TestSecretKey; }
        }

        public string PublishableKey {
            get { return Live ? LivePublishableKey : TestPublishableKey; }
        }
    }
}