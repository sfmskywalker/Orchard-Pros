using Orchard;
using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.Services.Commerce {
    public interface ITransactionService : IDependency {
        Transaction Create(IUser user, string productName, decimal amount, string currency, string context = null);
        Transaction Get(string handle);
        void Charge(Transaction transaction, string reference);
        void Decline(Transaction transaction);
    }
}