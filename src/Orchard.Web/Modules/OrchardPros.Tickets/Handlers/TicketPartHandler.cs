using System.Linq;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using OrchardPros.Tickets.Models;
using OrchardPros.Tickets.Services;

namespace OrchardPros.Tickets.Handlers {
    public class TicketPartHandler : ContentHandler {
        private readonly IReplyService _replyService;

        public TicketPartHandler(IRepository<TicketPartRecord> repository, IReplyService replyService) {
            _replyService = replyService;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<TicketPart>(SetupLazyFields);
        }

        private void SetupLazyFields(ActivatedContentContext context, TicketPart part) {
            part.RepliesField.Loader(() => _replyService.GetRepliesFor(part.Id).List().ToArray());
        }
    }
}