using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services.Commerce {
    public interface ITransferService : IDependency {
        Transfer Create(int recipientUserId, decimal amount, string currency, string context = null);
        IPagedList<Transfer> GetTransfersByUser(int userId, int? skip, int? take);
        TransferReport GetTranferReportByUser(int userId);
    }
}