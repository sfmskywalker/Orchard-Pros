using Orchard.Events;
using OrchardPros.Models;

namespace OrchardPros.Services.Commerce {
    public interface ITransactionEventHandler : IEventHandler {
        void Charged(TransactionChargedContext context);
    }
}