using System;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public class ReplyService : IReplyService {
        private readonly IContentManager _contentManager;

        public ReplyService(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public IContentQuery<ReplyPart> GetRepliesFor(int contentId) {
            return _contentManager.Query<ReplyPart>().Where<CommonPartRecord>(x => x.Container.Id == contentId);
        }

        public ReplyPart Create(IContent container, string body, IUser user, string subject = null, int? parentReplyId = null, Action<ReplyPart> initialize = null) {
            return _contentManager.Create<ReplyPart>("Reply", r => {
                r.ContainingContent = container;
                r.Body = body;
                r.User = user;
                r.Subject = subject;
                r.ParentReplyId = parentReplyId;
                
                if (initialize != null)
                    initialize(r);
            });
        }
    }
}