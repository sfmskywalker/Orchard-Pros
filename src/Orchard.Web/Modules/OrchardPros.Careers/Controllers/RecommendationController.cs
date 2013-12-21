using System;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Services;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using OrchardPros.Careers.Helpers;
using OrchardPros.Careers.Models;
using OrchardPros.Careers.Services;
using OrchardPros.Careers.ViewModels;

namespace OrchardPros.Careers.Controllers {
    [Admin]
    public class RecommendationController : Controller {
        private readonly IContentManager _contentManager;
        private readonly IRecommendationManager _recommendationManager;
        private readonly INotifier _notifier;
        private readonly IMembershipService _membershipService;
        private readonly IOrchardServices _services;
        private readonly IClock _clock;

        public RecommendationController(IRecommendationManager recommendationManager, IMembershipService membershipService, IOrchardServices services, IClock clock) {
            _contentManager = services.ContentManager;
            _recommendationManager = recommendationManager;
            _notifier = services.Notifier;
            _membershipService = membershipService;
            _services = services;
            _clock = clock;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Create(int id) {
            var profile = _contentManager.Get<ProfessionalProfilePart>(id);
            var viewModel = CreateViewModel(profile, x => {
                x.RecommendingUserName = _services.WorkContext.CurrentUser.UserName;
                x.CreatedUtc = _clock.UtcNow;
            });
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(int id, RecommendationViewModel viewModel) {
            var recommendingUser = ValidateRecommendingUser(viewModel);
            var profile = _contentManager.Get<ProfessionalProfilePart>(id);
            if (!ModelState.IsValid) {
                return View(InitializeViewModel(viewModel, profile));
            }
            _recommendationManager.Create(id, x => Update(x, viewModel, recommendingUser.Id));
            _notifier.Information(T("That Recommendation has been created."));
            return RedirectToAction("Edit", "Admin", new { profile.Id, Area = "Orchard.Users" });
        }

        public ActionResult Edit(int id) {
            var recommendation = _recommendationManager.Get(id);
            var profile = _contentManager.Get<ProfessionalProfilePart>(recommendation.UserId);
            var viewModel = CreateViewModel(profile, x => {
                var recommendingUser = _contentManager.Get<IUser>(recommendation.RecommendingUserId);
                x.Approved = recommendation.Approved;
                x.CreatedUtc = recommendation.CreatedUtc;
                x.RecommendingUserName = recommendingUser != null ? recommendingUser.UserName : null;
                x.Text = recommendation.Text;
            });
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(int id, RecommendationViewModel viewModel) {
            var recommendingUser = ValidateRecommendingUser(viewModel);
            var recommendation = _recommendationManager.Get(id);
            var profile = _contentManager.Get<ProfessionalProfilePart>(recommendation.UserId);

            if (!ModelState.IsValid) {
                return View(InitializeViewModel(viewModel, profile));
            }
            Update(recommendation, viewModel, recommendingUser.Id);
            _notifier.Information(T("That Recommendation has been updated."));
            return RedirectToAction("Edit", "Admin", new { profile.Id, Area = "Orchard.Users" });
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var recommendation = _recommendationManager.Get(id);
            var profile = _contentManager.Get<ProfessionalProfilePart>(recommendation.UserId);
            _recommendationManager.Delete(recommendation);
            _notifier.Information(T("That Recommendation has been deleted."));
            return RedirectToAction("Edit", "Admin", new { profile.Id, Area = "Orchard.Users" });
        }

        [HttpPost]
        public ActionResult Approve(int id) {
            var recommendation = _recommendationManager.Get(id);
            var profile = _contentManager.Get<ProfessionalProfilePart>(recommendation.UserId);
            _recommendationManager.Approve(recommendation);
            _notifier.Information(T("That Recommendation has been approved."));
            return RedirectToAction("Edit", "Admin", new { profile.Id, Area = "Orchard.Users" });
        }

        private IUser ValidateRecommendingUser(RecommendationViewModel viewModel) {
            var recommendingUser = _membershipService.GetUser(viewModel.RecommendingUserName.TrimSafe());

            if (recommendingUser == null) {
                ModelState.AddModelError("RecommendingUserName", T("The specified username does not exist").ToString());
            }
            return recommendingUser;
        }

        private void Update(Recommendation recommendation, RecommendationViewModel viewModel, int recommendingProfileId) {
            recommendation.Text = viewModel.Text.TrimSafe();
            recommendation.CreatedUtc = viewModel.CreatedUtc;
            recommendation.Approved = viewModel.Approved;
            recommendation.RecommendingUserId = recommendingProfileId;
        }

        private static RecommendationViewModel CreateViewModel(ProfessionalProfilePart profile, Action<RecommendationViewModel> initialize = null) {
            return InitializeViewModel(new RecommendationViewModel(), profile, initialize);
        }

        private static RecommendationViewModel InitializeViewModel(RecommendationViewModel viewModel, ProfessionalProfilePart profile, Action<RecommendationViewModel> initialize = null) {
            viewModel.Profile = profile;
            if (initialize != null)
                initialize(viewModel);
            return viewModel;
        }
    }
}