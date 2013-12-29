using Orchard;
using Orchard.ContentManagement;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Services {
    public interface IReplyService : IDependency {
        IContentQuery<ReplyPart> GetRepliesFor(int contentId);
    }
}