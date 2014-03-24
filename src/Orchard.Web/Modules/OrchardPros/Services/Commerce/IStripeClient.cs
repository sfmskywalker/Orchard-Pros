using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services.Commerce {
    public interface IStripeClient : IDependency {
        StripeCharge CreateCharge(int amount, string currency, string card, string description);
        StripeToken Token(string code, string grantType = "authorization_code");
    }
}