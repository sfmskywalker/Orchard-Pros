using Orchard.Data;
using Orchard.Security;
using Orchard.Services;
using OrchardPros.Models;
using OrchardPros.Services.System;

namespace OrchardPros.Services.Commerce {
    public class TransactionService : ITransactionService {
        private readonly IClock _clock;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IHandleGenerator _handleGenerator;

        public TransactionService(IClock clock, IRepository<Transaction> transactionRepository, IHandleGenerator handleGenerator) {
            _clock = clock;
            _transactionRepository = transactionRepository;
            _handleGenerator = handleGenerator;
        }

        public Transaction Create(IUser user, string productName, decimal amount, string currency, string context = null) {
            var transaction = new Transaction {
                Handle = _handleGenerator.Generate(),
                Amount = amount,
                Currency = currency,
                Context = context,
                CreatedUtc = _clock.UtcNow,
                ProductName = productName,
                Status = TransactionStatus.Pending,
                UserId = user.Id
            };

            _transactionRepository.Create(transaction);
            return transaction;
        }

        public Transaction Get(string handle) {
            return _transactionRepository.Get(x => x.Handle == handle);
        }

        public void Charge(Transaction transaction, string reference) {
            transaction.ChargedUtc = _clock.UtcNow;
            transaction.Reference = reference;
            transaction.Status = TransactionStatus.Charged;
        }

        public void Decline(Transaction transaction) {
            transaction.DeclinedUtc = _clock.UtcNow;
            transaction.Status = TransactionStatus.Declined;
        }
    }
}