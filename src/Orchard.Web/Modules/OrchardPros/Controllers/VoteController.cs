using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using OrchardPros.Models;
using OrchardPros.Services.User;

namespace OrchardPros.Controllers {
    [Authorize]
    public class VoteController : Controller {
        private readonly IOrchardServices _services;
        private readonly IVotingPolicy _votingPolicy;

        public VoteController(IOrchardServices services, IVotingPolicy votingPolicy) {
            _services = services;
            _votingPolicy = votingPolicy;
        }

        [HttpPost]
        public ActionResult Up(int id) {
            return Vote(id, 1);
        }

        [HttpPost]
        public ActionResult Down(int id) {
            return Vote(id, -1);
        }

        private ActionResult Vote(int contentId, double value) {
            var user = _services.WorkContext.CurrentUser;
            var votablePart = _services.ContentManager.Get<VotablePart>(contentId);
            var caps = _votingPolicy.CastVote(votablePart, user, value);
            
            return Json(new {
                Points = votablePart.VoteCount,
                Caps = new {
                    VoteUp = caps.CanVoteUp,
                    VoteDown = caps.CanVoteDown
                }
            }, JsonRequestBehavior.DenyGet);
        }
    }
}