using Orchard.Events;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface ITransactionEventHandler : IEventHandler {
        void Charged(TransactionChargedContext context);
    }
}