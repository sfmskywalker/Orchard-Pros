using System.Linq;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using OrchardPros.Tickets.Models;
using OrchardPros.Tickets.Services;

namespace OrchardPros.Tickets.Handlers {
    public class TicketPartHandler : ContentHandler {
        private readonly IReplyService _replyService;
        private readonly ITicketService _ticketService;

        public TicketPartHandler(IRepository<TicketPartRecord> repository, IReplyService replyService, ITicketService ticketService) {
            _replyService = replyService;
            _ticketService = ticketService;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<TicketPart>(SetupLazyFields);
        }

        private void SetupLazyFields(ActivatedContentContext context, TicketPart part) {
            part.RepliesField.Loader(() => _replyService.GetRepliesFor(part.Id).List().ToArray());
            part.CategoriesField.Loader(() => _ticketService.GetCategoriesFor(part.Id).ToArray());
            part.TagsField.Loader(() => _ticketService.GetTagsFor(part.Id).ToArray());
            part.LastModifiedUtcField.Loader(() => _ticketService.GetLastModifiedUtcFor(part));
            part.LastModifierField.Loader(() => _ticketService.GetLastModifierFor(part));
            part.RemainingTimeFunc = () => _ticketService.GetRemainingTimeFor(part);
        }
    }
}