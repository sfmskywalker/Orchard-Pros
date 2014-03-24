using System.Linq;
using Orchard.ContentManagement.Handlers;
using OrchardPros.Models;
using OrchardPros.Services.Content;

namespace OrchardPros.Handlers {
    public class RepliesPartHandler : ContentHandler {
        private readonly IReplyService _replyService;

        public RepliesPartHandler(IReplyService replyService) {
            _replyService = replyService;
            OnActivated<RepliesPart>(SetupLazyFields);
        }

        private void SetupLazyFields(ActivatedContentContext context, RepliesPart part) {
            part.RepliesField.Loader(() => _replyService.GetRepliesByContent(part.Id).ToArray());
        }
    }
}