using System;
using System.Linq;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using OrchardPros.Models;
using OrchardPros.Services;

namespace OrchardPros.Handlers {
    public class TicketPartHandler : ContentHandler {
        private readonly ITicketService _ticketService;

        public TicketPartHandler(IRepository<TicketPartRecord> repository, ITicketService ticketService) {
            _ticketService = ticketService;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<TicketPart>(SetupLazyFields);
            OnUpdated<TicketPart>(OnTicketUpdated);
        }

        private void OnTicketUpdated(UpdateContentContext context, TicketPart part) {
            part.Record.Categories = String.Format("|{0}|", String.Join("|", part.Categories.Select(x => x.Id)));
            part.Record.Tags = String.Format("|{0}|", String.Join("|", part.Tags.Select(x => x.Id)));
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