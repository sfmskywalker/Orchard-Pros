using System.Linq;
using Orchard.ContentManagement.Handlers;
using OrchardPros.Models;
using OrchardPros.Services.Content;

namespace OrchardPros.Handlers {
    public class AttachmentsHolderPartHandler : ContentHandler {
        private readonly IAttachmentService _attachmentService;

        public AttachmentsHolderPartHandler(IAttachmentService attachmentService) {
            _attachmentService = attachmentService;
            OnActivated<AttachmentsHolderPart>(SetupLazyFields);
        }

        private void SetupLazyFields(ActivatedContentContext context, AttachmentsHolderPart part) {
            part.AttachmentsField.Loader(() => _attachmentService.GetAttachments(part.AttachmentIds).ToArray());
            part.AttachmentsField.Setter(value => {
                var attachments = value != null ? value.ToArray() : new AttachmentPart[0];
                part.AttachmentIds = attachments.Select(x => x.Id).ToArray();
                return attachments;
            });
        }
    }
}