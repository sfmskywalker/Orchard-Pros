using System.Web.Mvc;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Services;
using Orchard.Themes;
using Orchard.UI.Notify;
using OrchardPros.Services;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Authorize, Themed]
    public class BountyController : Controller {
        private readonly INotifier _notifier;
        private readonly ITicketService _ticketService;
        private readonly IClock _clock;
        private readonly IOrchardServices _services;
        private readonly IAuthorizer _authorizer;
        private readonly ICommerceService _commerceService;

        public BountyController(
            ITicketService ticketService, 
            IClock clock, 
            IOrchardServices services, ICommerceService commerceService) {

            _notifier = services.Notifier;
            _ticketService = ticketService;
            _clock = clock;
            _services = services;
            _commerceService = commerceService;
            _authorizer = services.Authorizer;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        private IUser CurrentUser {
            get { return _services.WorkContext.CurrentUser; }
        }

        public ActionResult Create(int id) {
            var ticket = _ticketService.GetTicket(id);
            if(!_authorizer.Authorize(Permissions.StartBounty, ticket))
                return new HttpUnauthorizedResult();

            return View(new BountyViewModel{ Ticket = ticket});
        }

        [HttpPost]
        public ActionResult Create(int id, BountyViewModel model) {
            var ticket = _ticketService.GetTicket(id);
            if (!_authorizer.Authorize(Permissions.StartBounty, ticket))
                return new HttpUnauthorizedResult();

            if (!ModelState.IsValid) {
                model.Ticket = ticket;
                return View(model);
            }

            var transaction = _commerceService.CreateTransaction(CurrentUser, "Bounty", model.Amount.Value);
            return RedirectToAction("Pay", "Stripe", new { id = transaction.Handle });
        }
    }
}