using System.Linq;
using System.Web.Mvc;
using OrchardPros.Tickets.Services;

namespace OrchardPros.Tickets.Controllers {
    public class TagController : Controller {
        private readonly ITicketService _ticketService;

        public TagController(ITicketService ticketService) {
            _ticketService = ticketService;
        }

        public ActionResult Search(string term) {
            var terms = _ticketService.GetTagDictionary().Values.Where(x => x.Contains(term));
            return Json(terms, JsonRequestBehavior.AllowGet);
        }
    }
}