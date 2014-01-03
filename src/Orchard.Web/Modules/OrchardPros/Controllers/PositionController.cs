using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Services;
using Orchard.Themes;
using Orchard.UI.Notify;
using OrchardPros.Helpers;
using OrchardPros.Models;
using OrchardPros.Services;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Themed, Authorize]
    public class PositionController : Controller {
        private readonly IClock _clock;
        private readonly IPositionManager _positionManager;
        private readonly INotifier _notifier;
        private readonly IOrchardServices _services;
        private readonly IContentManager _contentManager;

        public PositionController(IClock clock, IPositionManager positionManager, IOrchardServices services) {
            _clock = clock;
            _positionManager = positionManager;
            _notifier = services.Notifier;
            _services = services;
            _contentManager = services.ContentManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private IUser CurrentUser {
            get { return _services.WorkContext.CurrentUser; }
        }

        public ActionResult Create() {
            var viewModel = CreateViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(PositionViewModel viewModel) {
            if (!ModelState.IsValid) {
                return View(InitializeViewModel(viewModel));
            }
            _positionManager.Create(CurrentUser.Id, x => Update(x, viewModel));
            _notifier.Information(T("Your Position has been created."));
            return Redirect(Url.Profile(CurrentUser));
        }

        public ActionResult Edit(int id) {
            var position = _positionManager.Get(id);
            var user = _contentManager.Get<IUser>(position.UserId);

            if(!_services.Authorizer.Authorize(Permissions.ManageOwnProfile, user))
                return new HttpUnauthorizedResult(T("You do not have permissions to edit this Position.").ToString());

            var viewModel = CreateViewModel(x => {
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
            var user = _contentManager.Get<IUser>(position.UserId);

            if (!_services.Authorizer.Authorize(Permissions.ManageOwnProfile, user))
                return new HttpUnauthorizedResult(T("You do not have permissions to edit this Position.").ToString());

            if (!ModelState.IsValid) {
                return View(InitializeViewModel(viewModel));
            }
            Update(position, viewModel);
            _notifier.Information(T("Your Position has been updated."));
            return Redirect(Url.Profile(CurrentUser));
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var position = _positionManager.Get(id);
            var user = _contentManager.Get<IUser>(position.UserId);

            if (!_services.Authorizer.Authorize(Permissions.ManageOwnProfile, user))
                return new HttpUnauthorizedResult(T("You do not have permissions to delete this Position.").ToString());

            _positionManager.Archive(position);
            _notifier.Information(T("Your Position has been deleted."));
            return Redirect(Url.Profile(CurrentUser));
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

        private PositionViewModel CreateViewModel(Action<PositionViewModel> initialize = null) {
            return InitializeViewModel(new PositionViewModel(), initialize);
        }

        private PositionViewModel InitializeViewModel(PositionViewModel viewModel, Action<PositionViewModel> initialize = null) {
            viewModel.User = CurrentUser;
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