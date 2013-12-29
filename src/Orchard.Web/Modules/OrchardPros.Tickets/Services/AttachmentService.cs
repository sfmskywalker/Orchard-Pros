using System.Collections.Generic;
using Orchard.ContentManagement;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Services {
    public class AttachmentService : IAttachmentService {
        private readonly IContentManager _contentManager;

        public AttachmentService(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public IEnumerable<AttachmentPart> GetAttachments(IEnumerable<int> ids) {
            return _contentManager.GetMany<AttachmentPart>(ids, VersionOptions.Published, QueryHints.Empty);
        }
    }
}