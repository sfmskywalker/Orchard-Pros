using Orchard;
using Orchard.ContentManagement;
using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.Services.User {
    public interface IVotingPolicy : IDependency {
        VotingCapabilities GetCapabilities(IContent content, IUser user);
        VotingCapabilities CastVote(IContent content, IUser user, double value);
    }
}