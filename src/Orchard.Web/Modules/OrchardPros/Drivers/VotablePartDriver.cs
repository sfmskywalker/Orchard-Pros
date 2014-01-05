using Orchard;
using Orchard.ContentManagement.Drivers;
using OrchardPros.Models;
using OrchardPros.Services;

namespace OrchardPros.Drivers {
    public class VotablePartDriver : ContentPartDriver<VotablePart> {
        private readonly IOrchardServices _services;
        private readonly IVoteService _voteService;

        public VotablePartDriver(IOrchardServices services, IVoteService voteService) {
            _services = services;
            _voteService = voteService;
        }

        protected override DriverResult Display(VotablePart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_Votable", () => {
                var currentUser = _services.WorkContext.CurrentUser;
                var vote = _voteService.GetVoteByUser(part, currentUser);
                var voteCaps = _voteService.GetVoteCapabilitiesByUser(part, currentUser);
                return shapeHelper.Parts_Votable(Vote: vote, VoteCaps: voteCaps);
            });
        }
    }
}