using OrchardPros.Models;

namespace OrchardPros.Services.Commerce {
    public abstract class TransactionEventHandlerBase : ITransactionEventHandler {
        public virtual void Charged(TransactionChargedContext context) { }
    }
}