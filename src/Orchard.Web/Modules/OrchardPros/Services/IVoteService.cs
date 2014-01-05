using Orchard;
using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface IVoteService : IDependency {
        Vote GetVoteByUser(VotablePart part, IUser user);
        int CountVotes(int contentId);
        VoteCaps GetVoteCapabilitiesByUser(VotablePart part, IUser user);
        void Vote(VotablePart votablePart, IUser user, VoteDirection direction);
    }
}