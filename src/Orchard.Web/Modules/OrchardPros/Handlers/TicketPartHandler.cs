using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using OrchardPros.Models;
using OrchardPros.Services.Content;
using OrchardPros.Services.User;

namespace OrchardPros.Handlers {
    public class TicketPartHandler : ContentHandler {
        private readonly ITicketService _ticketService;
        private readonly IUserManager _userManager;
        private readonly IReplyService _replyService;
        private readonly IContentManager _contentManager;

        public TicketPartHandler(
            IRepository<TicketPartRecord> repository, 
            ITicketService ticketService, 
            IUserManager userManager, 
            IReplyService replyService, 
            IContentManager contentManager) {

            _ticketService = ticketService;
            _userManager = userManager;
            _replyService = replyService;
            _contentManager = contentManager;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<TicketPart>(SetupLazyFields);
            OnCreated<TicketPart>(OnTicketCreated);
            OnUpdated<TicketPart>(OnTicketUpdated);
            OnRemoved<TicketPart>(OnTicketRemoved);
        }

        private void OnTicketCreated(CreateContentContext context, TicketPart part) {
            _userManager.AddActivityPoints(part.User, 5);
        }

        private void OnTicketUpdated(UpdateContentContext context, TicketPart part) {
            part.Record.Categories = String.Format("|{0}|", String.Join("|", part.Categories.Select(x => x.Id)));
            part.Record.Tags = String.Format("|{0}|", String.Join("|", part.Tags.Select(x => x.Id)));
            _userManager.AddActivityPoints(part.User);
        }

        private void SetupLazyFields(ActivatedContentContext context, TicketPart part) {
            part.CategoriesField.Loader(() => _ticketService.GetCategoriesFor(part.Id).ToArray());
            part.TagsField.Loader(() => _ticketService.GetTagsFor(part.Id).ToArray());
            part.LastModifiedUtcField.Loader(() => _ticketService.GetLastModifiedUtcFor(part));
            part.LastModifierField.Loader(() => _ticketService.GetLastModifierFor(part));
            part.RemainingTimeFunc = () => _ticketService.GetRemainingTimeFor(part);
        }

        private void OnTicketRemoved(RemoveContentContext context, TicketPart part) {
            // Delete replies.
            var replies = _replyService.GetRepliesByContent(part.Id);

            foreach (var reply in replies) {
                _contentManager.Remove(reply.ContentItem);
            }
        }
    }
}