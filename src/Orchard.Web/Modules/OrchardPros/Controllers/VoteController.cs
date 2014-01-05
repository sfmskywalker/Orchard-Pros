using System;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Mvc;
using OrchardPros.Models;
using OrchardPros.Services;

namespace OrchardPros.Controllers {
    [Authorize]
    public class VoteController : Controller {
        private readonly IVoteService _voteService;
        private readonly IOrchardServices _services;

        public VoteController(IOrchardServices services, IVoteService voteService) {
            _services = services;
            _voteService = voteService;
        }

        [HttpPost]
        public ActionResult Up(int id) {
            return Vote(id, VoteDirection.Up);
        }

        [HttpPost]
        public ActionResult Down(int id) {
            return Vote(id, VoteDirection.Down);
        }

        private ActionResult Vote(int contentId, VoteDirection direction) {
            var user = _services.WorkContext.CurrentUser;
            var votablePart = _services.ContentManager.Get<VotablePart>(contentId);
            
            _voteService.Vote(votablePart, user, direction);
            var caps = _voteService.GetVoteCapabilitiesByUser(votablePart, user);

            return Json(new {
                Points = votablePart.VoteCount,
                Caps = new {
                    VoteUp = caps.VoteUp,
                    VoteDown = caps.VoteDown
                }
            }, JsonRequestBehavior.DenyGet);
        }
    }
}