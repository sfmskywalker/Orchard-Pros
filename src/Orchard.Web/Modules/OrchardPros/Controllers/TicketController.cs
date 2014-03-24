using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Contrib.Voting.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Html;
using Orchard.Security;
using Orchard.Services;
using Orchard.Themes;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using OrchardPros.Models;
using OrchardPros.Helpers;
using OrchardPros.Services.Content;
using OrchardPros.Services.User;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Authorize, Themed]
    public class TicketController : Controller {
        private readonly INotifier _notifier;
        private readonly ITicketService _ticketService;
        private readonly IClock _clock;
        private readonly IOrchardServices _services;
        private readonly IAttachmentService _attachmentService;
        private readonly IRecommendationManager _recommendationManager;
        private readonly IAuthorizer _authorizer;
        private readonly IEnumerable<IContentHandler> _handlers;
        private readonly IVotingService _votingService;

        public TicketController(
            ITicketService ticketService, 
            IClock clock, 
            IOrchardServices services, 
            IAttachmentService attachmentService, 
            IRecommendationManager recommendationManager, 
            IAuthorizer authorizer, 
            IEnumerable<IContentHandler> handlers, 
            IVotingService votingService) {

            _notifier = services.Notifier;
            _ticketService = ticketService;
            _clock = clock;
            _services = services;
            _attachmentService = attachmentService;
            _recommendationManager = recommendationManager;
            _authorizer = authorizer;
            _handlers = handlers;
            _votingService = votingService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        private IUser CurrentUser {
            get { return _services.WorkContext.CurrentUser; }
        }

        [AllowAnonymous]
        public ActionResult Index(PagerParameters pagerParameters, TicketsCriteria criteria = TicketsCriteria.Latest, int? categoryId = null, int? tagId = null, string term = null) {
            var pager = new Pager(_services.WorkContext.CurrentSite, pagerParameters);
            var tickets = _ticketService.GetTickets(pager.GetStartIndex(), pager.PageSize, criteria, categoryId, tagId, term);
            var pagerShape = _services.New.Pager(pager).TotalItemCount(tickets.TotalItemCount);
            var viewModel = _services.New.ViewModel(
                Tickets_List: _services.New.Tickets_List(
                    Tickets: tickets, 
                    Criteria: criteria,
                    CategoryId: categoryId,
                    Pager: pagerShape),
                Tickets_List_Filter: _services.New.Tickets_List_Filter(
                    Categories: _ticketService.GetCategories().ToArray(),
                    Tags: _ticketService.GetPopularTags().ToArray(),
                    CategoryId: categoryId,
                    TagId: tagId,
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
                t.DeadlineUtc = model.DeadlineUtc.Value;
                t.ExperiencePoints = _ticketService.CalculateExperience(CurrentUser);
                t.ExternalUrl = model.ExternalUrl.TrimSafe();
            });

            _ticketService.AssignCategories(ticket, model.Categories);
            _ticketService.AssignTags(ticket, model.Tags);
            UpdateAttachments(ticket, model);

            var context = new UpdateContentContext(ticket.ContentItem);
            _handlers.Invoke(x => x.Updated(context), Logger);

            _notifier.Information(T("Your ticket has been created."));
            return Redirect(Url.ItemDisplayUrl(ticket));
        }
        
        public ActionResult Edit(int id) {
            var ticket = _ticketService.GetTicket(id);
            var model = SetupEditViewModel(new TicketViewModel {
                Id = id,
                Categories = ticket.Categories.Select(x => x.Id).ToArray(),
                Tags = _ticketService.GetTagsFor(ticket.Id).ToDelimitedString(),
                DeadlineUtc = ticket.DeadlineUtc,
                Body = ticket.Body,
                ExperiencePoints = ticket.ExperiencePoints,
                Subject = ticket.Subject,
                Type = ticket.Type,
                ExternalUrl = ticket.ExternalUrl
            }, ticket);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(int id, TicketViewModel model) {
            var ticket = _ticketService.GetTicket(id);

            if (!ModelState.IsValid) {
                SetupEditViewModel(model, ticket);
                return View(model);
            }

            ticket.DeadlineUtc = model.DeadlineUtc.Value;
            ticket.Body = model.Body.TrimSafe();
            ticket.Subject = model.Subject;
            ticket.Type = model.Type;
            ticket.ExternalUrl = model.ExternalUrl.TrimSafe();

            _ticketService.AssignCategories(ticket, model.Categories);
            _ticketService.AssignTags(ticket, model.Tags);
            UpdateAttachments(ticket, model);

            var context = new UpdateContentContext(ticket.ContentItem);

            _handlers.Invoke(x => x.Updated(context), Logger);
            _ticketService.Publish(ticket);

            _notifier.Information(T("Your ticket has been updated."));
            return Redirect(Url.ItemDisplayUrl(ticket));
        }

        [HttpPost]
        public ActionResult Solve(int id, int replyId, int? rating, string recommendation, bool? allowPublication) {
            var ticket = _ticketService.GetTicket(id);

            if(!_authorizer.Authorize(Permissions.SolveOwnTickets, ticket))
                return new HttpUnauthorizedResult();

            var currentUser = CurrentUser;
            var reply = ticket.As<RepliesPart>().Replies.Single(x => x.Id == replyId);
            var isOwnReply = reply.User.Id == currentUser.Id;
            
            _ticketService.Solve(ticket, reply);
            _notifier.Information(T("Your ticket has been solved."));

            if (!isOwnReply) {
                if (rating != null) {
                    _votingService.Vote(reply.User.ContentItem, currentUser.UserName, Request.UserHostAddress, rating.Value);
                }

                if (!String.IsNullOrWhiteSpace(recommendation)) {
                    _recommendationManager.Create(r => {
                        r.AllowPublication = allowPublication == true;
                        r.Body = recommendation.TrimSafe();
                        r.RecommendingUser = ticket.User;
                        r.UserId = reply.User.Id;
                    });
                    _notifier.Information(allowPublication == true
                        ? T("Your recommendation has been created and will be published when approved. Thanks!")
                        : T("Your recommendation has been created. Thanks!"));
                }
            }

            return Redirect(Url.ItemDisplayUrl(ticket));
        }

        private void UpdateAttachments(TicketPart ticket, TicketViewModel model) {
            if (model.Attachments != null)
                _attachmentService.AssociateAttachments(ticket, model.Attachments.UploadedFileNames, model.Attachments.UploadedFileContentTypes, model.Attachments.OriginalFileNames);
            else {
                _attachmentService.DeleteAttachments(ticket);
            }
        }

        private TicketViewModel SetupCreateViewModel(TicketViewModel model) {
            model.ExperiencePoints = _ticketService.CalculateExperience(CurrentUser);
            model.CategoryTerms = _ticketService.GetCategories().ToArray();
            model.DeadlineUtc = _clock.UtcNow.AddDays(7);
            model.User = CurrentUser;

            if (model.Attachments != null) {
                model.Attachments.CurrentFiles = new List<AttachmentViewModel>();
            }
            return model;
        }

        private TicketViewModel SetupEditViewModel(TicketViewModel model, TicketPart ticket) {
            model.User = CurrentUser;
            model.CategoryTerms = _ticketService.GetCategories().ToArray();
            model.Attachments = new AttachmentsViewModel {
                CurrentFiles = ticket.As<AttachmentsHolderPart>().Attachments.Select(x => new AttachmentViewModel {FileName = x.OriginalFileName, FileSize = x.FileSize}).ToList()
            };
            return model;
        }
    }
}