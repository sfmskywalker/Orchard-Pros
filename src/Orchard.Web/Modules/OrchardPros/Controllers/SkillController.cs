using System;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using OrchardPros.Helpers;
using OrchardPros.Models;
using OrchardPros.Services;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Admin]
    public class SkillController : Controller {
        private readonly IContentManager _contentManager;
        private readonly ISkillManager _skillManager;
        private readonly INotifier _notifier;

        public SkillController(IContentManager contentManager, ISkillManager skillManager, INotifier notifier) {
            _contentManager = contentManager;
            _skillManager = skillManager;
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
        public ActionResult Create(int id, SkillViewModel viewModel) {
            var profile = _contentManager.Get<UserProfilePart>(id);
            if (!ModelState.IsValid) {
                return View(InitializeViewModel(viewModel, profile));
            }
            _skillManager.Create(id, x => Update(x, viewModel));
            _notifier.Information(T("Your Skill has been created."));
            return RedirectToAction("Edit", "Admin", new { profile.Id, Area = "Orchard.Users" });
        }

        public ActionResult Edit(int id) {
            var skill = _skillManager.Get(id);
            var profile = _contentManager.Get<UserProfilePart>(skill.UserId);
            var viewModel = CreateViewModel(profile, x => {
                x.Name = skill.Name;
                x.Rating = skill.Rating;
            });
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(int id, SkillViewModel viewModel) {
            var skill = _skillManager.Get(id);
            var profile = _contentManager.Get<UserProfilePart>(skill.UserId);

            if (!ModelState.IsValid) {
                return View(InitializeViewModel(viewModel, profile));
            }
            Update(skill, viewModel);
            _notifier.Information(T("Your Skill has been updated."));
            return RedirectToAction("Edit", "Admin", new { profile.Id, Area = "Orchard.Users" });
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var skill = _skillManager.Get(id);
            var profile = _contentManager.Get<UserProfilePart>(skill.UserId);
            _skillManager.Delete(skill);
            _notifier.Information(T("Your Skill has been deleted."));
            return RedirectToAction("Edit", "Admin", new { profile.Id, Area = "Orchard.Users" });
        }

        private static void Update(Skill skill, SkillViewModel viewModel) {
            skill.Name = viewModel.Name.TrimSafe();
            skill.Rating = viewModel.Rating;
        }

        private static SkillViewModel CreateViewModel(UserProfilePart profile, Action<SkillViewModel> initialize = null) {
            return InitializeViewModel(new SkillViewModel(), profile, initialize);
        }

        private static SkillViewModel InitializeViewModel(SkillViewModel viewModel, UserProfilePart profile, Action<SkillViewModel> initialize = null) {
            viewModel.Profile = profile;
            if (initialize != null)
                initialize(viewModel);
            return viewModel;
        }
    }
}