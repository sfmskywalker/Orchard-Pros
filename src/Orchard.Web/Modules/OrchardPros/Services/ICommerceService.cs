using Orchard;
using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface ICommerceService : IDependency {
        Transaction CreateTransaction(IUser user, string productName, decimal amount);
        Transaction GetTransaction(string handle);
    }
}