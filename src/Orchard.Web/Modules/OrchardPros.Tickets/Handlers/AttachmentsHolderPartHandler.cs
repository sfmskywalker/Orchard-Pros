using System.Linq;
using Orchard.ContentManagement.Handlers;
using OrchardPros.Tickets.Models;
using OrchardPros.Tickets.Services;

namespace OrchardPros.Tickets.Handlers {
    public class AttachmentsHolderPartHandler : ContentHandler {
        private readonly IAttachmentService _attachmentService;

        public AttachmentsHolderPartHandler(IAttachmentService attachmentService) {
            _attachmentService = attachmentService;
            OnActivated<AttachmentsHolderPart>(SetupLazyFields);
        }

        private void SetupLazyFields(ActivatedContentContext context, AttachmentsHolderPart part) {
            part.AttachmentsField.Loader(() => _attachmentService.GetAttachments(part.AttachmentIds).ToArray());
        }
    }
}