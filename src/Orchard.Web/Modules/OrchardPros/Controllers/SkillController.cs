using System;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Notify;
using OrchardPros.Helpers;
using OrchardPros.Models;
using OrchardPros.Services;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Themed, Authorize]
    public class SkillController : Controller {
        private readonly IContentManager _contentManager;
        private readonly ISkillManager _skillManager;
        private readonly INotifier _notifier;
        private readonly IOrchardServices _services;

        public SkillController(ISkillManager skillManager, IOrchardServices services) {
            _contentManager = services.ContentManager;
            _skillManager = skillManager;
            _notifier = services.Notifier;
            _services = services;
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
        public ActionResult Create(SkillViewModel viewModel) {
            if (!ModelState.IsValid) {
                return View(InitializeViewModel(viewModel));
            }
            _skillManager.Create(CurrentUser.Id, x => Update(x, viewModel));
            _notifier.Information(T("Your Skill has been created."));
            return Redirect(Url.Profile(CurrentUser));
        }

        public ActionResult Edit(int id) {
            var skill = _skillManager.Get(id);
            var user = _contentManager.Get<IUser>(skill.UserId);

            if(!_services.Authorizer.Authorize(Permissions.ManageOwnProfile, user))
                return new HttpUnauthorizedResult(T("You don't have permissions to edit that Skill").ToString());

            var viewModel = CreateViewModel(x => {
                x.Id = id;
                x.Name = skill.Name;
                x.Rating = skill.Rating;
            });
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(int id, SkillViewModel viewModel) {
            var skill = _skillManager.Get(id);
            var user = _contentManager.Get<IUser>(skill.UserId);

            if (!_services.Authorizer.Authorize(Permissions.ManageOwnProfile, user))
                return new HttpUnauthorizedResult(T("You don't have permissions to edit that Skill").ToString());

            if (!ModelState.IsValid) {
                return View(InitializeViewModel(viewModel, vm => vm.Id = id));
            }
            Update(skill, viewModel);
            _notifier.Information(T("Your Skill has been updated."));
            return Redirect(Url.Profile(CurrentUser));
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var skill = _skillManager.Get(id);
            var user = _contentManager.Get<IUser>(skill.UserId);

            if (!_services.Authorizer.Authorize(Permissions.ManageOwnProfile, user))
                return new HttpUnauthorizedResult(T("You don't have permissions to delete that Skill").ToString());

            _skillManager.Delete(skill);
            _notifier.Information(T("Your Skill has been deleted."));
            return Redirect(Url.Profile(CurrentUser));
        }

        private static void Update(Skill skill, SkillViewModel viewModel) {
            skill.Name = viewModel.Name.TrimSafe();
            skill.Rating = viewModel.Rating;
        }

        private SkillViewModel CreateViewModel(Action<SkillViewModel> initialize = null) {
            return InitializeViewModel(new SkillViewModel(), initialize);
        }

        private SkillViewModel InitializeViewModel(SkillViewModel viewModel, Action<SkillViewModel> initialize = null) {
            viewModel.User = CurrentUser;
            if (initialize != null)
                initialize(viewModel);
            return viewModel;
        }
    }
}