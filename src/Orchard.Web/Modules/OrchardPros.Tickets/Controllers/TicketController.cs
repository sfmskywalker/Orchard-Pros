using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Mvc.Html;
using Orchard.Services;
using Orchard.Themes;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using OrchardPros.Membership.Helpers;
using OrchardPros.Tickets.Helpers;
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

        public TicketController(ITicketService ticketService, IClock clock, IOrchardServices services) {
            _notifier = services.Notifier;
            _ticketService = ticketService;
            _clock = clock;
            _services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private ExpertPart CurrentUser {
            get { return _services.WorkContext.CurrentUser.As<ExpertPart>(); }
        }

        public ActionResult Index(PagerParameters pagerParameters, TicketsCriteria criteria = TicketsCriteria.Latest, int? categoryId = null) {
            var pager = new Pager(_services.WorkContext.CurrentSite, pagerParameters);
            var tickets = _ticketService.GetTickets(skip: pager.GetStartIndex(), take: pager.PageSize, criteria: criteria);
            var pagerShape = _services.New.Pager(pager).TotalItemCount(tickets.TotalItemCount);
            var viewModel = _services.New.ViewModel(
                Tickets_List: _services.New.Tickets_List(
                    Tickets: tickets, 
                    Criteria: criteria,
                    CategoryId: categoryId,
                    Pager: pagerShape),
                Tickets_List_Filter: _services.New.Tickets_List_Filter(
                    Categories: _ticketService.GetCategories().ToArray(),
                    Tags: _ticketService.GetTags().ToArray(),
                    CategoryId: categoryId,
                    Criteria: criteria,
                    Pager: pagerShape));
            return View(viewModel);
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

            var ticket = _ticketService.Create(user, model.Subject, model.Body, model.Type, t => {
                t.Bounty = model.Bounty;
                t.DeadlineUtc = model.DeadlineUtc.Value;
                t.ExperiencePoints = _ticketService.CalculateExperience(CurrentUser);
            });

            _ticketService.AssignCategories(ticket, model.Categories);
            _ticketService.AssignTags(ticket, model.Tags);
            _ticketService.AssociateAttachments(ticket, model.UploadedFileNames, model.OriginalFileNames);

            _notifier.Information(T("Your ticket has been created."));
            return Redirect(Url.ItemDisplayUrl(ticket));
        }

        public ActionResult Edit(int id) {
            var ticket = _ticketService.GetTicket(id);
            var model = SetupEditViewModel(new TicketViewModel {
                Id = id,
                Bounty = ticket.Bounty,
                Categories = ticket.Categories.Select(x => x.Id).ToArray(),
                Tags = _ticketService.GetTagsFor(ticket.Id).ToDelimitedString(),
                DeadlineUtc = ticket.DeadlineUtc,
                Body = ticket.Body,
                ExperiencePoints = ticket.ExperiencePoints,
                Subject = ticket.Subject,
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
            ticket.Body = model.Body.TrimSafe();
            ticket.Subject = model.Subject;
            ticket.Type = model.Type;

            _ticketService.AssignCategories(ticket, model.Categories);
            _ticketService.AssignTags(ticket, model.Tags);
            _ticketService.AssociateAttachments(ticket, model.UploadedFileNames, model.OriginalFileNames);
            _ticketService.Publish(ticket);

            _notifier.Information(T("Your ticket has been updated."));
            return Redirect(Url.ItemDisplayUrl(ticket));
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