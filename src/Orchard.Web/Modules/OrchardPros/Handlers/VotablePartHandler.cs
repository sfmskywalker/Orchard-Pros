using Contrib.Voting.Services;
using Orchard.ContentManagement.Handlers;
using OrchardPros.Models;

namespace OrchardPros.Handlers {
    public class VotablePartHandler : ContentHandler {
        private readonly IVotingService _votingService;

        public VotablePartHandler(IVotingService votingService) {
            _votingService = votingService;
            OnActivated<VotablePart>(SetupLazyFields);
        }

        private void SetupLazyFields(ActivatedContentContext context, VotablePart part) {
            part.VoteCountField.Loader(() => _votingService.GetResult(part.Id, "sum").Count);
        }
    }
}