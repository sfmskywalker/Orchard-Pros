using System;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Security;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Services {
    public interface IReplyService : IDependency {
        IContentQuery<ReplyPart> GetRepliesFor(int contentId);
        ReplyPart Create(IContent container, string body, IUser user, string subject = null, int? parentReplyId = null, Action<ReplyPart> initialize = null);
    }
}