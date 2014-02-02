using Orchard.Data;
using Orchard.Security;
using Orchard.Services;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public class CommerceService : ICommerceService {
        private readonly IClock _clock;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IHandleGenerator _handleGenerator;

        public CommerceService(IClock clock, IRepository<Transaction> transactionRepository, IHandleGenerator handleGenerator) {
            _clock = clock;
            _transactionRepository = transactionRepository;
            _handleGenerator = handleGenerator;
        }

        public Transaction CreateTransaction(IUser user, string productName, decimal amount) {
            var transaction = new Transaction {
                Handle = _handleGenerator.Generate(),
                Amount = amount,
                CreatedUtc = _clock.UtcNow,
                ProductName = productName,
                Status = TransactionStatus.Pending,
                UserId = user.Id
            };

            _transactionRepository.Create(transaction);
            return transaction;
        }

        public Transaction GetTransaction(string handle) {
            return _transactionRepository.Get(x => x.Handle == handle);
        }

        public void ChargeTransaction(Transaction transaction, string reference) {
            transaction.ChargedUtc = _clock.UtcNow;
            transaction.Reference = reference;
            transaction.Status = TransactionStatus.Charged;
        }

        public void DeclineTransaction(Transaction transaction) {
            transaction.DeclinedUtc = _clock.UtcNow;
            transaction.Status = TransactionStatus.Declined;
        }
    }
}