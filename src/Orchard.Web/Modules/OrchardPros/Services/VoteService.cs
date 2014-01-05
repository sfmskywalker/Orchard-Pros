using System.Linq;
using System.Web.UI.WebControls;
using Orchard.Data;
using Orchard.Security;
using Orchard.Services;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public class VoteService : IVoteService {
        private readonly IRepository<Vote> _voteRepository;
        private readonly IClock _clock;

        public VoteService(IRepository<Vote> voteRepository, IClock clock) {
            _voteRepository = voteRepository;
            _clock = clock;
        }

        public Vote GetVoteByUser(VotablePart part, IUser user) {
            var vote = _voteRepository.Get(x => x.ContentItemId == part.Id && x.UserId == user.Id);

            if (vote == null) {
                vote = new Vote {
                    ContentItemId = part.Id,
                    CreatedUtc = _clock.UtcNow,
                    UserId = user.Id,
                    Points = 0
                };

                _voteRepository.Create(vote);
            }

            return vote;
        }

        public int CountVotes(int contentId) {
            return _voteRepository.Table.Where(x => x.ContentItemId == contentId).Select(x => x.Points).Sum();
        }

        public VoteCaps GetVoteCapabilitiesByUser(VotablePart part, IUser user) {
            var maxPoints = 1;
            var vote = GetVoteByUser(part, user);
            var caps = new VoteCaps {
                Vote = vote,
                VoteUp = vote.Points < maxPoints,
                VoteDown = vote.Points > -maxPoints
            };
            return caps;
        }

        public void Vote(VotablePart votablePart, IUser user, VoteDirection direction) {
            var caps = GetVoteCapabilitiesByUser(votablePart, user);
            var vote = caps.Vote;

            switch (direction) {
                case VoteDirection.Up:
                    if (caps.VoteUp)
                        vote.Points++;
                    break;
                case VoteDirection.Down:
                    if (caps.VoteDown)
                        vote.Points--;
                    break;
            }
        }
    }
}