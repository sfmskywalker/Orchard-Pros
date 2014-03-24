using System;
using System.Linq;
using Contrib.Voting.Models;
using Contrib.Voting.Services;
using Orchard.ContentManagement;
using Orchard.Mvc;
using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.Services.User {
    public class VotingPolicy : IVotingPolicy {
        private readonly IVotingService _votingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VotingPolicy(IVotingService votingService, IHttpContextAccessor httpContextAccessor) {
            _votingService = votingService;
            _httpContextAccessor = httpContextAccessor;
        }

        public VotingCapabilities GetCapabilities(IContent content, IUser user) {
            var vote = _votingService.Get(x => x.ContentItemRecord.Id == content.Id && x.Username == user.UserName).FirstOrDefault();
            return GetCapabilities(content, user, vote);
        }

        public VotingCapabilities CastVote(IContent content, IUser user, double value) {
            var caps = GetCapabilities(content, user);

            if (!caps.CanVote)
                throw new InvalidOperationException("The specified user cannot cast a vote with the specified number of points on the specified content.");

            if (caps.Vote == null) {
                var ip = _httpContextAccessor.Current().Request.UserHostAddress;
                caps.Vote = _votingService.Vote(caps.Content.ContentItem, caps.User.UserName, ip, value);
            }
            else {
                var newValue = caps.Vote.Value + value;
                _votingService.ChangeVote(caps.Vote, newValue);
            }

            return GetCapabilities(content, user, caps.Vote);
        }

        private VotingCapabilities GetCapabilities(IContent content, IUser user, VoteRecord vote) {
            var points = vote != null ? vote.Value : 0;
            var maxPoints = 1; // Currently hardcoded, but will eventually vary on certain properties of the user.
            var minPoints = -maxPoints;

            return new VotingCapabilities {
                MaxPoints = maxPoints,
                CanVoteDown = points > minPoints,
                CanVoteUp = points < maxPoints,
                User = user,
                Content = content,
                Vote = vote
            };
        }
    }
}