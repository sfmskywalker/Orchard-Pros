using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.FileSystems.Media;
using Orchard.Localization;
using Orchard.Services;
using Orchard.Themes;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using OrchardPros.Tickets.Models;
using OrchardPros.Tickets.Services;
using OrchardPros.Tickets.ViewModels;

namespace OrchardPros.Tickets.Controllers {
    [Authorize, Themed]
    public class TicketController : Controller {
        private readonly INotifier _notifier;
        private readonly ITicketService _ticketService;
        private readonly IClock _clock;
        private readonly IOrchardServices _services;
        private readonly IStorageProvider _storageProvider;

        public TicketController(ITicketService ticketService, IClock clock, IOrchardServices services, IStorageProvider storageProvider) {
            _notifier = services.Notifier;
            _ticketService = ticketService;
            _clock = clock;
            _services = services;
            _storageProvider = storageProvider;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private ExpertPart CurrentUser {
            get { return _services.WorkContext.CurrentUser.As<ExpertPart>(); }
        }

        public ActionResult Create() {
            var model = SetupCreateViewModel(new TicketViewModel());
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(TicketViewModel model) {
            var user = CurrentUser;

            if (!ModelState.IsValid) {
                SetupCreateViewModel(model);
                return View(model);
            }

            var ticket = _ticketService.Create(user, model.Title, model.Description, model.Type, t => {
                t.Bounty = model.Bounty;
                t.DeadlineUtc = model.DeadlineUtc.Value;
                t.ExperiencePoints = _ticketService.CalculateExperience(CurrentUser);
                t.Tags = model.Tags;
            });

            _ticketService.AssociateAttachments(ticket, model.UploadedFileNames, model.OriginalFileNames);

            _notifier.Information(T("Your ticket has been created."));
            return RedirectToAction("Details", new { ticket.Id });
        }

        public ActionResult Edit(int id) {
            var ticket = _ticketService.GetTicket(id);
            var model = SetupEditViewModel(new TicketViewModel {
                Id = id,
                Bounty = ticket.Bounty,
                Categories = ticket.Categories.Select(x => x.CategoryId).ToArray(),
                CreatedUtc = ticket.CreatedUtc,
                DeadlineUtc = ticket.DeadlineUtc,
                Description = ticket.Description,
                ExperiencePoints = ticket.ExperiencePoints,
                Tags = ticket.Tags,
                Title = ticket.Title,
                Type = ticket.Type
            });
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(int id, TicketViewModel model) {
            var ticket = _ticketService.GetTicket(id);

            if (!ModelState.IsValid) {
                SetupEditViewModel(model);
                return View(model);
            }

            ticket.Bounty = model.Bounty;
            ticket.DeadlineUtc = model.DeadlineUtc.Value;
            ticket.Description = model.Description;
            ticket.LastModifiedUtc = _clock.UtcNow;
            ticket.Tags = model.Tags;
            ticket.Title = model.Title;
            ticket.Type = model.Type;

            _ticketService.AssignCategories(ticket, model.Categories);
            _ticketService.AssociateAttachments(ticket, model.UploadedFileNames, model.OriginalFileNames);

            _notifier.Information(T("Your ticket has been updated."));
            return RedirectToAction("Details", new { ticket.Id });
        }

        public ActionResult Details(int id) {
            var ticket = _ticketService.GetTicket(id);

            if(ticket == null)
                return new HttpNotFoundResult();

            var shape = _services.New.ViewModel(Ticket: ticket);
            return View(shape);
        }

        [HttpPost]
        public JsonResult Upload() {
            var file = Request.Files[0];
            var temporaryFileName = _ticketService.UploadAttachment(file);

            return Json(new {
                uploadedFileName = temporaryFileName
            });
        }

        private TicketViewModel SetupCreateViewModel(TicketViewModel model) {
            model.ExperiencePoints = _ticketService.CalculateExperience(CurrentUser);
            model.CategoryTerms = _ticketService.GetCategories().ToArray();
            model.CreatedUtc = _clock.UtcNow;
            model.DeadlineUtc = _clock.UtcNow.AddDays(7);
            model.User = CurrentUser;
            return model;
        }

        private TicketViewModel SetupEditViewModel(TicketViewModel model) {
            model.User = CurrentUser;
            model.CategoryTerms = _ticketService.GetCategories().ToArray();
            return model;
        }
    }
}