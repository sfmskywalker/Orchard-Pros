using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface IStripeClient : IDependency {
        StripeCharge CreateCharge(int amount, string currency, string card, string description);
    }
}