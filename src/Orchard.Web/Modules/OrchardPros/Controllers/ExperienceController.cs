using System;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Services;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using OrchardPros.Helpers;
using OrchardPros.Models;
using OrchardPros.Services;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Admin]
    public class ExperienceController : Controller {
        private readonly IContentManager _contentManager;
        private readonly IExperienceManager _experienceManager;
        private readonly INotifier _notifier;
        private readonly IClock _clock;
        private readonly IPositionManager _positionManager;

        public ExperienceController(
            IExperienceManager experienceManager,
            IOrchardServices services, 
            IClock clock, 
            IPositionManager positionManager) {

            _contentManager = services.ContentManager;
            _experienceManager = experienceManager;
            _notifier = services.Notifier;
            _clock = clock;
            _positionManager = positionManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Create(int id) {
            var profile = _contentManager.Get<UserProfilePart>(id);
            var viewModel = CreateViewModel(profile, x => {
                x.CreatedUtc = _clock.UtcNow;
            });
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(int id, ExperienceViewModel viewModel) {
            var profile = _contentManager.Get<UserProfilePart>(id);
            if (!ModelState.IsValid) {
                return View(InitializeViewModel(viewModel, profile));
            }
            _experienceManager.Create(id, x => Update(x, viewModel));
            _notifier.Information(T("That Experience has been created."));
            return RedirectToAction("Edit", "Admin", new { profile.Id, Area = "Orchard.Users" });
        }

        public ActionResult Edit(int id) {
            var experience = _experienceManager.Get(id);
            var profile = _contentManager.Get<UserProfilePart>(experience.UserId);
            var viewModel = CreateViewModel(profile, x => {
                x.CreatedUtc = experience.CreatedUtc;
                x.Description = experience.Description;
                x.PositionId = experience.Position != null ? experience.Position.Id : default(int?);
            });
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(int id, ExperienceViewModel viewModel) {
            var experience = _experienceManager.Get(id);
            var profile = _contentManager.Get<UserProfilePart>(experience.UserId);

            if (!ModelState.IsValid) {
                return View(InitializeViewModel(viewModel, profile));
            }
            Update(experience, viewModel);
            _notifier.Information(T("That Experience has been updated."));
            return RedirectToAction("Edit", "Admin", new { profile.Id, Area = "Orchard.Users" });
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var experience = _experienceManager.Get(id);
            var profile = _contentManager.Get<UserProfilePart>(experience.UserId);
            _experienceManager.Delete(experience);
            _notifier.Information(T("That Experience has been deleted."));
            return RedirectToAction("Edit", "Admin", new { profile.Id, Area = "Orchard.Users" });
        }

        private void Update(Experience experience, ExperienceViewModel viewModel) {
            experience.Position = viewModel.PositionId != null ? _positionManager.Get(viewModel.PositionId.Value) : default(Position);
            experience.Description = viewModel.Description.TrimSafe();
            experience.CreatedUtc = viewModel.CreatedUtc;
        }

        private ExperienceViewModel CreateViewModel(UserProfilePart profile, Action<ExperienceViewModel> initialize = null) {
            return InitializeViewModel(new ExperienceViewModel(), profile, initialize);
        }

        private ExperienceViewModel InitializeViewModel(ExperienceViewModel viewModel, UserProfilePart profile, Action<ExperienceViewModel> initialize = null) {
            viewModel.Profile = profile;
            viewModel.AvailablePositions = _positionManager.Fetch(profile.Id).ToList();
            if (initialize != null)
                initialize(viewModel);
            return viewModel;
        }
    }
}