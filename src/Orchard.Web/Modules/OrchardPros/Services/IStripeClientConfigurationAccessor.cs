using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface IStripeClientConfigurationAccessor : IDependency {
        StripeClientConfiguration GetConfiguration();
    }
}