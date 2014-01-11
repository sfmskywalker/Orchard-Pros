using Orchard;
using Orchard.ContentManagement;
using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface IVotingPolicy : IDependency {
        VotingCapabilities GetCapabilities(IContent content, IUser user);
        void CastVote(VotingCapabilities caps, double value);
    }
}