using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
    public class PositionController : Controller {
        private readonly IContentManager _contentManager;
        private readonly IClock _clock;
        private readonly IPositionManager _positionManager;
        private readonly INotifier _notifier;

        public PositionController(IContentManager contentManager, IClock clock, IPositionManager positionManager, INotifier notifier) {
            _contentManager = contentManager;
            _clock = clock;
            _positionManager = positionManager;
            _notifier = notifier;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Create(int id) {
            var profile = _contentManager.Get<UserProfilePart>(id);
            var viewModel = CreateViewModel(profile);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(int id, PositionViewModel viewModel) {
            var profile = _contentManager.Get<UserProfilePart>(id);
            if (!ModelState.IsValid) {
                return View(InitializeViewModel(viewModel, profile));
            }
            _positionManager.Create(id, x => Update(x, viewModel));
            _notifier.Information(T("Your Position has been created."));
            return RedirectToAction("Edit", "Admin", new { profile.Id, Area = "Orchard.Users" });
        }

        public ActionResult Edit(int id) {
            var position = _positionManager.Get(id);
            var profile = _contentManager.Get<UserProfilePart>(position.UserId);
            var viewModel = CreateViewModel(profile, x => {
                x.CompanyName = position.CompanyName;
                x.Description = position.Description;
                x.IsCurrentPosition = position.IsCurrentPosition;
                x.Location = position.Location;
                x.PeriodEndMonth = position.PeriodEndMonth;
                x.PeriodEndYear = position.PeriodEndYear;
                x.PeriodStartMonth = position.PeriodStartMonth;
                x.PeriodStartYear = position.PeriodStartYear;
                x.Title = position.Title;
            });
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(int id, PositionViewModel viewModel) {
            var position = _positionManager.Get(id);
            var profile = _contentManager.Get<UserProfilePart>(position.UserId);

            if (!ModelState.IsValid) {
                return View(InitializeViewModel(viewModel, profile));
            }
            Update(position, viewModel);
            _notifier.Information(T("Your Position has been updated."));
            return RedirectToAction("Edit", "Admin", new { profile.Id, Area = "Orchard.Users" });
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var position = _positionManager.Get(id);
            var profile = _contentManager.Get<UserProfilePart>(position.UserId);
            _positionManager.Archive(position);
            _notifier.Information(T("Your Position has been deleted."));
            return RedirectToAction("Edit", "Admin", new { profile.Id, Area = "Orchard.Users" });
        }

        private static void Update(Position position, PositionViewModel viewModel) {
            position.CompanyName = viewModel.CompanyName.TrimSafe();
            position.Description = viewModel.Description.TrimSafe();
            position.IsCurrentPosition = viewModel.IsCurrentPosition;
            position.Location = viewModel.Location.TrimSafe();
            position.PeriodEndMonth = viewModel.PeriodEndMonth;
            position.PeriodEndYear = viewModel.PeriodEndYear;
            position.PeriodStartMonth = viewModel.PeriodStartMonth;
            position.PeriodStartYear = viewModel.PeriodStartYear;
            position.Title = viewModel.Title.TrimSafe();
        }

        private PositionViewModel CreateViewModel(UserProfilePart profile, Action<PositionViewModel> initialize = null) {
            return InitializeViewModel(new PositionViewModel(), profile, initialize);
        }

        private PositionViewModel InitializeViewModel(PositionViewModel viewModel, UserProfilePart profile, Action<PositionViewModel> initialize = null) {
            viewModel.Profile = profile;
            viewModel.Years = GetYears();
            viewModel.Months = GetMonths();
            if (initialize != null)
                initialize(viewModel);
            return viewModel;
        }

        private IList<int> GetYears() {
            var currentYear = _clock.UtcNow.Year;
            const int range = 100;
            return Enumerable.Range(currentYear - range, range + 1).Reverse().ToList();
        }

        private static IList<int> GetMonths() {
            return Enumerable.Range(1, 12).ToList();
        }
    }
}