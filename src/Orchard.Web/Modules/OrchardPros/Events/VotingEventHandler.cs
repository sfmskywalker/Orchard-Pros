using Contrib.Voting.Events;
using Contrib.Voting.Models;
using Orchard.Security;
using OrchardPros.Services.User;

namespace OrchardPros.Events {
    public class VotingEventHandler : IVotingEventHandler {
        private readonly IMembershipService _membershipService;
        private readonly IUserManager _userManager;

        public VotingEventHandler(IMembershipService membershipService, IUserManager userManager) {
            _membershipService = membershipService;
            _userManager = userManager;
        }

        public virtual void Voted(VoteRecord vote) {
            AddActivityPoints(vote);
        }

        public virtual void VoteChanged(VoteRecord vote, double previousValue) {
            AddActivityPoints(vote);
        }

        public virtual void VoteRemoved(VoteRecord vote) {
            AddActivityPoints(vote);
        }

        public virtual void Calculated(ResultRecord result) { }

        private void AddActivityPoints(VoteRecord vote) {
            var user = _membershipService.GetUser(vote.Username);
            _userManager.AddActivityPoints(user);
        }
    }
}