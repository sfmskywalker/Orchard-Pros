using Orchard;
using Orchard.ContentManagement.Drivers;
using OrchardPros.Models;
using OrchardPros.Services;

namespace OrchardPros.Drivers {
    public class VotablePartDriver : ContentPartDriver<VotablePart> {
        private readonly IOrchardServices _services;
        private readonly IVotingPolicy _votingPolicy;

        public VotablePartDriver(IOrchardServices services, IVotingPolicy votingPolicy) {
            _services = services;
            _votingPolicy = votingPolicy;
        }

        protected override DriverResult Display(VotablePart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_Votable", () => {
                var currentUser = _services.WorkContext.CurrentUser;
                var voteCaps = _votingPolicy.GetCapabilities(part, currentUser);
                return shapeHelper.Parts_Votable(Vote: voteCaps.Vote, VoteCaps: voteCaps);
            });
        }
    }
}