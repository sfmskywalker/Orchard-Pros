using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface ITransferService : IDependency {
        Transfer Create(int recipientUserId, decimal amount, string currency, string context = null);
    }
}