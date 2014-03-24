using System.Linq;
using System.Web.Mvc;
using OrchardPros.Services.Content;

namespace OrchardPros.Controllers {
    public class TagController : Controller {
        private readonly ITicketService _ticketService;

        public TagController(ITicketService ticketService) {
            _ticketService = ticketService;
        }

        public ActionResult Search(string term) {
            var terms = _ticketService.GetTags().Select(x => x.Name).Where(x => x.Contains(term));
            return Json(terms, JsonRequestBehavior.AllowGet);
        }
    }
}