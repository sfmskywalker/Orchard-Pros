using Orchard.Data;
using Orchard.Services;
using OrchardPros.Models;

namespace OrchardPros.Services.Commerce {
    public class TransferService : ITransferService {
        private readonly IClock _clock;
        private readonly IRepository<Transfer> _transferRepository;

        public TransferService(IClock clock, IRepository<Transfer> transferRepository) {
            _clock = clock;
            _transferRepository = transferRepository;
        }

        public Transfer Create(int recipientUserId, decimal amount, string currency, string context = null) {
            var transfer = new Transfer() {
                RecipientUserId = recipientUserId,
                Amount = amount,
                Currency = currency,
                Context = context,
                CreatedUtc = _clock.UtcNow,
                Status = TransferStatus.Pending
            };

            _transferRepository.Create(transfer);
            return transfer;
        }
    }
}