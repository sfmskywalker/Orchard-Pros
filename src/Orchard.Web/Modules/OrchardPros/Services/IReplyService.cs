using System;
using System.Collections.Generic;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface IReplyService : IDependency {
        IEnumerable<ReplyPart> GetRepliesByContent(int contentId);
        IEnumerable<ReplyPart> GetRepliesByUser(int userId);
        ReplyPart Create(IContent container, string body, IUser user, string subject = null, int? parentReplyId = null, Action<ReplyPart> initialize = null);
    }
}