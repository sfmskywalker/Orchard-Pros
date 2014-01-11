using Contrib.Voting.Models;
using Orchard.ContentManagement;
using Orchard.Security;

namespace OrchardPros.Models {
    public class VotingCapabilities {
        public int MaxPoints { get; set; }
        public bool CanVoteUp { get; set; }
        public bool CanVoteDown { get; set; }
        public VoteRecord Vote { get; set; }
        public IUser User { get; set; }
        public IContent Content { get; set; }

        public bool CanVote {
            get { return CanVoteUp || CanVoteDown; }
        }
    }
}