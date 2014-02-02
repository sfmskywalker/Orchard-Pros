using OrchardPros.Models;

namespace OrchardPros.Services {
    public abstract class TransactionEventHandlerBase : ITransactionEventHandler {
        public virtual void Charged(TransactionChargedContext context) { }
    }
}