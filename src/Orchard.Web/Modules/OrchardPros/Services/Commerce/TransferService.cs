using System.Linq;
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

        public IPagedList<Transfer> GetTransfersByUser(int userId, int? skip, int? take) {
            var query = _transferRepository.Fetch(x => x.RecipientUserId == userId, x => x.Desc(o => o.CreatedUtc));
            var totalItemCount = query.LongCount();

            return skip != null 
                ? new PagedList<Transfer>(_transferRepository.Fetch(x => x.RecipientUserId == userId, x => x.Desc(o => o.CreatedUtc), skip.Value, take.Value), totalItemCount) 
                : new PagedList<Transfer>(query, totalItemCount);
        }

        public TransferReport GetTranferReportByUser(int userId) {
            var baseQuery = _transferRepository.Fetch(x => x.RecipientUserId == userId);
            var totalPaid = baseQuery.Where(x => x.Status == TransferStatus.Completed).Sum(x => x.Amount);
            var totalPending = baseQuery.Where(x => x.Status == TransferStatus.Pending).Sum(x => x.Amount);

            return new TransferReport {
                UserId = userId,
                TotalPaid = totalPaid,
                TotalPending = totalPending
            };
        }
    }
}