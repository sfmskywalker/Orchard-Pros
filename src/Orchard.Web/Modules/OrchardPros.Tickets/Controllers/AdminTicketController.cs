using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Services;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using OrchardPros.Tickets.Helpers;
using OrchardPros.Tickets.Models;
using OrchardPros.Tickets.Services;
using OrchardPros.Tickets.ViewModels;

namespace OrchardPros.Tickets.Controllers {
    [Admin]
    public class AdminTicketController : Controller {
        private readonly IContentManager _contentManager;
        private readonly INotifier _notifier;
        private readonly ITicketService _ticketService;
        private readonly IClock _clock;

        public AdminTicketController(IContentManager contentManager, INotifier notifier, ITicketService ticketService, IClock clock) {
            _contentManager = contentManager;
            _notifier = notifier;
            _ticketService = ticketService;
            _clock = clock;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Create(int id) {
            var user = _contentManager.Get<ExpertPart>(id);
            var model = SetupCreateViewModel(new TicketViewModel(), user);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(int id, TicketViewModel model) {
            var user = _contentManager.Get<ExpertPart>(id);

            if (!ModelState.IsValid) {
                SetupCreateViewModel(model, user);
                return View(model);
            }

            var ticket = _ticketService.Create(user, model.Title, model.Description, model.Type, t => {
                t.Bounty = model.Bounty;
                t.DeadlineUtc = model.DeadlineUtc.Value;
                t.ExperiencePoints = model.ExperiencePoints;
            });
            
            _ticketService.AssignCategories(ticket, model.Categories);
            _ticketService.AssignTags(ticket, model.Tags);
            _notifier.Information(T("Ticket created for user {0}", user.As<IUser>().UserName));
            return RedirectToAction("Edit", "Admin", new { user.Id, Area = "Orchard.Users" });
        }

        public ActionResult Edit(int id) {
            var ticket = _ticketService.GetTicket(id);
            var user = _contentManager.Get<ExpertPart>(ticket.UserId);
            var model = SetupEditViewModel(new TicketViewModel {
                Bounty = ticket.Bounty,
                Categories = ticket.Categories.Select(x => x.CategoryId).ToArray(),
                Tags = _ticketService.GetTagsFor(ticket).ToDelimitedString(),
                CreatedUtc = ticket.CreatedUtc,
                DeadlineUtc = ticket.DeadlineUtc,
                Description = ticket.Description,
                ExperiencePoints = ticket.ExperiencePoints,
                Title = ticket.Title,
                Type = ticket.Type
            }, user);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(int id, TicketViewModel model) {
            var ticket = _ticketService.GetTicket(id);
            var user = _contentManager.Get<ExpertPart>(ticket.UserId);

            if (!ModelState.IsValid) {
                SetupEditViewModel(model, user);
                return View(model);
            }

            ticket.Bounty = model.Bounty;
            ticket.CreatedUtc = model.CreatedUtc.Value;
            ticket.DeadlineUtc = model.DeadlineUtc.Value;
            ticket.Description = model.Description;
            ticket.ExperiencePoints = model.ExperiencePoints;
            ticket.ModifiedUtc = _clock.UtcNow;
            ticket.Title = model.Title;
            ticket.Type = model.Type;
            
            _ticketService.AssignCategories(ticket, model.Categories);
            _ticketService.AssignTags(ticket, model.Tags);

            _notifier.Information(T("Ticket {0} updated for user {1}", id, user.As<IUser>().UserName));
            return RedirectToAction("Edit", "Admin", new { user.Id, Area = "Orchard.Users" });
        }

        public ActionResult Delete(int id) {
            var ticket = _ticketService.GetTicket(id);
            var user = _contentManager.Get<IUser>(ticket.UserId);
            _ticketService.Archive(ticket);

            _notifier.Information(T("Ticket removed for user {0}", user.UserName));
            return RedirectToAction("Edit", "Admin", new { user.Id, Area = "Orchard.Users" });
        }

        private TicketViewModel SetupCreateViewModel(TicketViewModel model, ExpertPart user) {
            model.ExperiencePoints = _ticketService.CalculateExperience(user);
            model.User = user;
            model.CategoryTerms = _ticketService.GetCategories().ToArray();
            model.CreatedUtc = _clock.UtcNow;
            model.DeadlineUtc = _clock.UtcNow.AddDays(7);
            return model;
        }

        private TicketViewModel SetupEditViewModel(TicketViewModel model, ExpertPart user) {
            model.User = user;
            model.CategoryTerms = _ticketService.GetCategories().ToArray();
            return model;
        }
    }
}