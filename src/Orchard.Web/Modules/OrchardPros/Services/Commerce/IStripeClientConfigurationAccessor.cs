using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services.Commerce {
    public interface IStripeClientConfigurationAccessor : IDependency {
        StripeClientConfiguration GetConfiguration();
    }
}