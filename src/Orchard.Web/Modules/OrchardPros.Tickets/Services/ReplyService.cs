using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Services {
    public class ReplyService : IReplyService {
        private readonly IContentManager _contentManager;

        public ReplyService(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public IContentQuery<ReplyPart> GetRepliesFor(int contentId) {
            return _contentManager.Query<ReplyPart>().Where<CommonPartRecord>(x => x.Container.Id == contentId);
        }
    }
}