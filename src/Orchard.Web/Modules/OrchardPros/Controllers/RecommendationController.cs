using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Notify;
using OrchardPros.Helpers;
using OrchardPros.Models;
using OrchardPros.Services.User;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Themed, Authorize]
    public class RecommendationController : Controller {
        private readonly IContentManager _contentManager;
        private readonly IRecommendationManager _recommendationManager;
        private readonly INotifier _notifier;
        private readonly IMembershipService _membershipService;
        private readonly IAuthorizer _authorizer;
        private readonly IOrchardServices _services;

        public RecommendationController(IRecommendationManager recommendationManager, IOrchardServices services, IMembershipService membershipService) {
            _contentManager = services.ContentManager;
            _recommendationManager = recommendationManager;
            _services = services;
            _membershipService = membershipService;
            _notifier = services.Notifier;
            _authorizer = services.Authorizer;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private IUser CurrentUser {
            get { return _services.WorkContext.CurrentUser; }
        }

        public ActionResult Create(string id) {
            var user = _membershipService.GetUser(id);

            if(!_authorizer.Authorize(Permissions.WriteRecommendation, user))
                return new HttpUnauthorizedResult(T("You cannot recommend yourself like this. Think of something else").ToString());

            return View(new RecommendationViewModel {
                User = user
            });
        }

        [HttpPost]
        public ActionResult Create(string id, RecommendationViewModel model) {
            var user = _membershipService.GetUser(id);

            if (!_authorizer.Authorize(Permissions.WriteRecommendation, user))
                return new HttpUnauthorizedResult(T("You cannot recommend yourself like this. Think of something else").ToString());

            if (!ModelState.IsValid) {
                model.User = user;
                return View(model);
            }

            _recommendationManager.Create(r => {
                r.Body = model.Text.TrimSafe();
                r.RecommendingUser = CurrentUser;
                r.User = user;
            });

            _notifier.Information(T("Your recommendation has been created and awaiting approval."));
            return Redirect(Url.Profile(id));
        }

        [HttpPost]
        public ActionResult Publish(int id) {
            var recommendation = _contentManager.Get<RecommendationPart>(id, VersionOptions.Latest);

            if(!_authorizer.Authorize(Permissions.PublishRecommendation, recommendation))
                return new HttpUnauthorizedResult(T("You can only publish recommendations created by others assigned to you.").ToString());

            var user = recommendation.User;
            _contentManager.Publish(recommendation.ContentItem);
            _notifier.Information(T("That recommendation has been published."));

            return Redirect(Url.Profile(user));
        }

        [HttpPost]
        public ActionResult Unpublish(int id) {
            var recommendation = _contentManager.Get<RecommendationPart>(id, VersionOptions.Latest);

            if (!_authorizer.Authorize(Permissions.PublishRecommendation, recommendation))
                return new HttpUnauthorizedResult(T("You can only unpublish recommendations created by others assigned to you.").ToString());

            var user = recommendation.User;
            _contentManager.Unpublish(recommendation.ContentItem);
            _notifier.Information(T("That recommendation has been unpublished."));

            return Redirect(Url.Profile(user));
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var recommendation = _contentManager.Get<RecommendationPart>(id);

            if (!_authorizer.Authorize(Permissions.DeleteRecommendation, recommendation))
                return new HttpUnauthorizedResult(T("You can only delete recommendations assigned to you.").ToString());

            var user = recommendation.User;
            _contentManager.Remove(recommendation.ContentItem);
            _notifier.Information(T("That recommendation has been removed."));

            return Redirect(Url.Profile(user));
        }
    }
}