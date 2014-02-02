using OrchardPros.Models;

namespace OrchardPros.Services {
    public class StripeClientConfigurationAccessor : IStripeClientConfigurationAccessor {
        public StripeClientConfiguration GetConfiguration() {
            return new StripeClientConfiguration {
                LivePublishableKey = "pk_live_4xLAX4EhUuS7VpSIRJo8qdmd",
                LiveSecretKey = "sk_live_dxs0V8R7uGYeVMOcAJHm9WcC",
                TestPublishableKey = "pk_test_p1ZgvvsTvnbUNWdO7CBSMeJ6",
                TestSecretKey = "sk_test_PNCH1IHnneafL8NvQWZnVK2w",
                Live = false
            };
        }
    }
}