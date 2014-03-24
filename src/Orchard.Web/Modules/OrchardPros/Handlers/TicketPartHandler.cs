using System;
using System.Linq;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using OrchardPros.Models;
using OrchardPros.Services.Content;
using OrchardPros.Services.User;

namespace OrchardPros.Handlers {
    public class TicketPartHandler : ContentHandler {
        private readonly ITicketService _ticketService;
        private readonly IUserManager _userManager;

        public TicketPartHandler(IRepository<TicketPartRecord> repository, ITicketService ticketService, IUserManager userManager) {
            _ticketService = ticketService;
            _userManager = userManager;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<TicketPart>(SetupLazyFields);
            OnCreated<TicketPart>(OnTicketCreated);
            OnUpdated<TicketPart>(OnTicketUpdated);
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
    }
}