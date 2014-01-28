using Orchard.Data;
using Orchard.Security;
using Orchard.Services;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public class CommerceService : ICommerceService {
        private readonly IClock _clock;
        private readonly IRepository<Transaction> _transactionRepository;

        public CommerceService(IClock clock, IRepository<Transaction> transactionRepository) {
            _clock = clock;
            _transactionRepository = transactionRepository;
        }

        public Transaction CreateTransaction(IUser user, string productName, decimal amount) {
            var transaction = new Transaction {
                Amount = amount,
                CreatedUtc = _clock.UtcNow,
                ProductName = productName,
                Status = TransactionStatus.PaymentPending,
                UserId = user.Id
            };

            _transactionRepository.Create(transaction);
            return transaction;
        }
    }
}