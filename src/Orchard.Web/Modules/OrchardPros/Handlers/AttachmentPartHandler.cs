using Orchard.ContentManagement.Handlers;
using OrchardPros.Models;
using OrchardPros.Services;

namespace OrchardPros.Handlers {
    public class AttachmentPartHandler : ContentHandler {
        private readonly IAttachmentService _attachmentService;

        public AttachmentPartHandler(IAttachmentService attachmentService) {
            _attachmentService = attachmentService;
            OnRemoved<AttachmentPart>(DeleteFile);
        }

        private void DeleteFile(RemoveContentContext context, AttachmentPart part) {
            _attachmentService.DeleteFileFor(part);
        }
    }
}