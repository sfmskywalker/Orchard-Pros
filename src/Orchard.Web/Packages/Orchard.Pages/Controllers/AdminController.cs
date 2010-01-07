using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Orchard.Pages.Models;
using Orchard.Pages.Services;
using Orchard.Pages.Services.Templates;
using Orchard.Pages.ViewModels;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using Orchard.Security;

namespace Orchard.Pages.Controllers {
    [ValidateInput(false)]
    public class AdminController : Controller {
        private readonly IPageManager _pageManager;
        private readonly IPageScheduler _pageScheduler;
        private readonly IRepository<Page> _repository;
        private readonly ITemplateProvider _templateProvider;
        private readonly IAuthorizer _authorizer;
        private readonly INotifier _notifier;

        public AdminController(IPageManager pageManager,
            IPageScheduler pageScheduler,
            IRepository<Page> repository,
            ITemplateProvider templateProvider,
            INotifier notifier,
            IAuthorizer authorizer) {
            _pageManager = pageManager;
            _authorizer = authorizer;
            _pageScheduler = pageScheduler;
            _repository = repository;
            _templateProvider = templateProvider;
            _notifier = notifier;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }


        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            //TEMP: this is a hack until a cron system is in place
            _pageScheduler.Sweep();
            base.OnActionExecuting(filterContext);
        }

        public ActionResult Index(PageIndexOptions options) {
            // Default options
            if (options == null)
                options = new PageIndexOptions();

            IEnumerable<Page> pages;
            // Filtering
            switch (options.Filter) {
                case PageIndexFilter.All:
                    pages = from page in _repository.Table select page;
                    break;
                case PageIndexFilter.Published:
                    //TODO: We have to force a "ToList()" call. It seems the Linq query is not
                    //      properly interpreted by the Linq to NHib layer: the "page.Published != null"
                    //      condition is interpreted as always false
                    pages = from page in _repository.Table.ToList() where page.Published != null select page;
                    break;
                case PageIndexFilter.Offline:
                    pages = from page in _repository.Table.ToList() where page.Published == null select page;
                    break;
                case PageIndexFilter.Scheduled:
                    pages = from page in _repository.Table.ToList() where page.Scheduled.Any() select page;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var entries = pages.Select(page => CreatePageEntry(page)).ToList();
            var model = new PageIndexViewModel { Options = options, PageEntries = entries };
            return View(model);
        }

        [HttpPost, ActionName("Index")]
        [FormValueRequired("submit.BulkEdit")]
        public ActionResult IndexPOST(PageIndexOptions options, IList<PageEntry> pageEntries) {
            //var viewModel = new PageIndexViewModel();
            //UpdateModel(viewModel);

            try {
                IEnumerable<PageEntry> checkedEntries = pageEntries.Where(p => p.IsChecked);
                switch (options.BulkAction) {
                    case PageIndexBulkAction.None:
                        break;

                    case PageIndexBulkAction.PublishNow:
                        //TODO: Transaction
                        if (!_authorizer.Authorize(Permissions.PublishPages, T("Couldn't publish page")))
                            return new HttpUnauthorizedResult();

                        foreach (PageEntry entry in checkedEntries) {
                            entry.Page = _repository.Get(entry.PageId);
                            _pageScheduler.ClearTasks(entry.Page);
                            var lastRevision = _pageManager.GetLastRevision(entry.PageId);
                            _pageManager.Publish(lastRevision, new PublishOptions());
                        }
                        break;

                    case PageIndexBulkAction.PublishLater:
                        if (!_authorizer.Authorize(Permissions.SchedulePages, T("Couldn't publish page")))
                            return new HttpUnauthorizedResult();

                        if (options.BulkPublishLaterDate != null) {
                            //TODO: Transaction
                            foreach (PageEntry entry in checkedEntries) {
                                var page = _repository.Get(entry.PageId);
                                var revision = _pageManager.AcquireDraft(page.Id);
                                _pageScheduler.ClearTasks(page);
                                _pageScheduler.AddPublishTask(revision, options.BulkPublishLaterDate.Value);
                            }
                        }
                        else {
                            return View("BulkPublishLater", new PageIndexViewModel {
                                Options = options,
                                PageEntries = pageEntries
                            });
                        }
                        break;

                    case PageIndexBulkAction.Unpublish:
                        if (!_authorizer.Authorize(Permissions.UnpublishPages, T("Couldn't unpublish page")))
                            return new HttpUnauthorizedResult();

                        foreach (PageEntry entry in checkedEntries) {
                            var page = _repository.Get(entry.PageId);
                            _pageManager.UnpublishPage(page);
                        }
                        break;

                    case PageIndexBulkAction.Delete:
                        if (!_authorizer.Authorize(Permissions.DeletePages, T("Couldn't delete page")))
                            return new HttpUnauthorizedResult();

                        if (options.BulkDeleteConfirmed) {
                            //TODO: Transaction
                            foreach (PageEntry entry in checkedEntries) {
                                var page = _repository.Get(entry.PageId);
                                _pageScheduler.ClearTasks(page);
                                _pageManager.DeletePage(page);
                            }
                        }
                        else {
                            return View("BulkDeleteConfirm", new PageIndexViewModel {
                                Options = options,
                                PageEntries = pageEntries
                            });
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex) {
                ModelState.AddModelError("", ex);
                //TODO: Is this a good idea to return to the index view in case of error?
                return Index(options);
            }

            return RedirectToAction("Index");
        }

        private PageEntry CreatePageEntry(Page page) {
            var lastRevision = _pageManager.GetLastRevision(page.Id);
            var published = page.Published;
            var draftRevision = (published != null && published.PageRevision == lastRevision) ? null : lastRevision;

            return new PageEntry {
                Page = page,
                Published = published,
                DraftRevision = draftRevision,
                IsChecked = false,
                PageId = page.Id
            };
        }

        public ActionResult Create() {
            var model = new PageCreateViewModel { Templates = _templateProvider.List() };

            // Select first template by default
            if (model.Templates.Count > 0) {
                model.TemplateName = model.Templates[0].Name;
            }


            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post), ActionName("Create")]
        public ActionResult CreatePOST() {

            var viewModel = new PageCreateViewModel { Templates = _templateProvider.List() };
            try {
                UpdateModel(viewModel);
                if (!_authorizer.Authorize(Permissions.CreatePages, T("Couldn't create page")))
                    return new HttpUnauthorizedResult();

                Logger.Information("Creating Page slug:{0} title{1}: template{2}",
                    viewModel.Slug, viewModel.Title, viewModel.TemplateName);
                var revision = _pageManager.CreatePage(new CreatePageParams(viewModel.Title, viewModel.Slug, viewModel.TemplateName));
                return RedirectToAction("Edit", new { revision.Page.Id });
            }
            catch (Exception ex) {
                ModelState.AddModelError("", ex);
                return View(viewModel);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Edit(int id) {
            var lastRevision = _pageManager.GetLastRevision(id);
            if (lastRevision == null) {
                //TODO: Error message
                throw new HttpException(404, "page with id " + id + " was not found");
            }
            var model = new PageEditViewModel { Revision = lastRevision, Command = PageEditCommand.PublishNow };

            if (lastRevision.Page.Scheduled.Any(x => x.Action == ScheduledAction.Publish)) {
                model.Command = PageEditCommand.PublishLater;
                model.PublishLaterDate = lastRevision.Page.Scheduled.First(x => x.Action == ScheduledAction.Publish).ScheduledDate;
            }
            else if (lastRevision.IsPublished()) {
                model.Command = PageEditCommand.PublishNow;
            }
            else {
                model.Command = PageEditCommand.SaveDraft;
            }

            FillViewModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPOST(int id) {
            var model = new PageEditViewModel();
            try {
                //TODO: need a transaction to surround this entire lot, really

                model.Revision = _pageManager.AcquireDraft(id);
                model.Template = _templateProvider.Get(model.Revision.TemplateName);

                UpdateModel(model);
                RemoveUnusedContentItems(model.Revision, model.Template);

                _pageScheduler.ClearTasks(model.Revision.Page);
                if (!_authorizer.Authorize(Permissions.ModifyPages, T("Couldn't edit page")))
                    return new HttpUnauthorizedResult();

                switch (model.Command) {
                    case PageEditCommand.PublishNow:
                        if (!_authorizer.Authorize(Permissions.PublishPages, T("Couldn't publish page")))
                            return new HttpUnauthorizedResult();

                        _pageManager.Publish(model.Revision, new PublishOptions());
                        break;
                    case PageEditCommand.PublishLater:
                        if (!_authorizer.Authorize(Permissions.SchedulePages, T("Couldn't publish page")))
                            return new HttpUnauthorizedResult();

                        if (model.PublishLaterDate == null)
                            throw new ArgumentNullException("No pub later value");
                        _pageScheduler.AddPublishTask(model.Revision, model.PublishLaterDate.Value);
                        break;
                    case PageEditCommand.SaveDraft:
                        break;
                    default:
                        throw new NotImplementedException("Unknown edit command");
                }

                //TODO: ur... edit's "Save" continues to show itself?
                return RedirectToAction("Index");
            }
            catch (Exception ex) {
                FillViewModel(model);
                ModelState.AddModelError("Page", ex);
                return View(model);
            }
        }

        [HttpPost]
        [ActionName("Edit")]
        [FormValueRequired("submit.DeleteDraft")]
        public ActionResult DeleteDraft(int id) {
#warning UNIT TEST!!!!
            if (!_authorizer.Authorize(Permissions.DeleteDraftPages, T("Couldn't delete draft page")))
                return new HttpUnauthorizedResult();

            var lastRevision = _pageManager.GetLastRevision(id);
            if (!lastRevision.IsPublished())
                lastRevision.Page.Revisions.Remove(lastRevision);

            return RedirectToAction("Edit", new { id });
        }


        void FillViewModel(PageEditViewModel model) {
            if (model.Template == null)
                model.Template = _templateProvider.Get(model.Revision.TemplateName);

            if (!model.Revision.IsPublished())
                model.CanDeleteDraft = true;
        }


        // This feels wrong... Should it move to the page manager? Or should ApplyTemplateName move to the controller?

        private static void RemoveUnusedContentItems(PageRevision revision, TemplateDescriptor descriptor) {
            // toarray used in iteration because contents are modified
            foreach (var contentItem in revision.Contents.ToArray()) {
                var contentItemEmpty = string.IsNullOrEmpty(contentItem.Content);
                var zoneNamePresent = descriptor.Zones.Contains(contentItem.ZoneName);
                if (contentItemEmpty && !zoneNamePresent) {
                    revision.Contents.Remove(contentItem);
                }
            }
        }

        private static bool HasOrphanContentItems(PageRevision revision, TemplateDescriptor descriptor) {
            foreach (var contentItem in revision.Contents) {
                var contentItemEmpty = string.IsNullOrEmpty(contentItem.Content);
                var zoneNamePresent = descriptor.Zones.Contains(contentItem.ZoneName);
                if (!contentItemEmpty && !zoneNamePresent) {
                    // zone name not present, but content not empty
                    return true;
                }
            }
            return false;
        }

        public ActionResult ChooseTemplate(int id) {
            var revision = _pageManager.GetLastRevision(id);
            var viewModel = new ChooseTemplateViewModel {
                TemplateName = revision.TemplateName,
                Templates = _templateProvider.List()
            };
            return View(viewModel);
        }

        [HttpPost, ActionName("ChooseTemplate")]
        public ActionResult ChooseTemplatePOST(int id) {
            var viewModel = new ChooseTemplateViewModel();
            try {
                UpdateModel(viewModel);

                //todo: needs transaction
                var draft = _pageManager.AcquireDraft(id);
                var template = _templateProvider.Get(viewModel.TemplateName);

                if (draft.TemplateName == viewModel.TemplateName) {
                    //todo: rollback transaction
                    return RedirectToAction("Edit", new { id });
                }


                _pageManager.ApplyTemplateName(draft, viewModel.TemplateName);
                RemoveUnusedContentItems(draft, template);

                if (HasOrphanContentItems(draft, template)) {
                    _notifier.Error("You have switched to a template that does not have the same content zones as the previous one, resulting in some of your contents not showing up on your site. You can either delete that content or copy it into another zone.");
                }

                return RedirectToAction("Edit", new { id });
            }
            catch {
                return View(viewModel);
            }
        }

        public ActionResult Export(int? id) {
            if (id == null)
                return View(_repository.Table.ToArray());

            var revision = _pageManager.GetLastRevision((int)id);
            return View(new[] { revision.Page });
        }
    }

    public class FormValueRequiredAttribute : ActionMethodSelectorAttribute {
        private readonly string _submitButtonName;

        public FormValueRequiredAttribute(string submitButtonName) {
            _submitButtonName = submitButtonName;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo) {
            var value = controllerContext.HttpContext.Request.Form[_submitButtonName];
            return !string.IsNullOrEmpty(value);
        }
    }
}