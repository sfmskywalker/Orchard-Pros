using Orchard;
using Orchard.ContentManagement.Drivers;
using OrchardPros.Models;
using OrchardPros.Services.User;

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
                var voteCaps = currentUser != null ? _votingPolicy.GetCapabilities(part, currentUser) : default(VotingCapabilities);
                return voteCaps != null ? shapeHelper.Parts_Votable(Vote: voteCaps.Vote, VoteCaps: voteCaps) : shapeHelper.Parts_Votable_Anonymous();
            });
        }
    }
}