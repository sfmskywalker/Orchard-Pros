using Orchard.ContentManagement.Handlers;
using OrchardPros.Models;
using OrchardPros.Services;

namespace OrchardPros.Handlers {
    public class VotablePartHandler : ContentHandler {
        private readonly IVoteService _voteService;

        public VotablePartHandler(IVoteService voteService) {
            _voteService = voteService;
            OnActivated<VotablePart>(SetupLazyFields);
        }

        private void SetupLazyFields(ActivatedContentContext context, VotablePart part) {
            part.VoteCountField.Loader(() => _voteService.CountVotes(part.Id));
        }
    }
}