using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Security;
using OrchardPros.Helpers;
using OrchardPros.Models;

namespace OrchardPros.Services.Content {
    public class ReplyService : IReplyService {
        private readonly IContentManager _contentManager;

        public ReplyService(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public IEnumerable<ReplyPart> GetRepliesByContent(int contentId) {
            return _contentManager.Query<ReplyPart>().Where<CommonPartRecord>(x => x.Container.Id == contentId).List<ReplyPart>();
        }

        public IEnumerable<ReplyPart> GetRepliesByUser(int userId) {
            return _contentManager.Query<ReplyPart>().Where<CommonPartRecord>(x => x.OwnerId == userId).List<ReplyPart>();
        }

        public ReplyPart Create(IContent container, string body, IUser user, string subject = null, int? parentReplyId = null, Action<ReplyPart> initialize = null) {
            Guard.ArgumentNull(container, "container");
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