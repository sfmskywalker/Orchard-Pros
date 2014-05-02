using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Notify;
using OrchardPros.Helpers;
using OrchardPros.Models;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Themed, Authorize]
    public class SocialChannelsController : Controller {
        private readonly INotifier _notifier;
        private readonly IOrchardServices _services;

        public SocialChannelsController(IOrchardServices services) {
            _notifier = services.Notifier;
            _services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private IUser CurrentUser {
            get { return _services.WorkContext.CurrentUser; }
        }

        public ActionResult Edit() {
            var profilePart = CurrentUser.As<UserProfilePart>();
            var viewModel = new SocialChannelsViewModel {
                TwitterAlias = profilePart.TwitterAlias,
                FacebookUrl = profilePart.FacebookUrl,
                LinkedInUrl = profilePart.LinkedInUrl,
                CompanyWebsiteUrl = profilePart.CompanyWebsiteUrl,
                BlogUrl = profilePart.BlogUrl,
                User = CurrentUser
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(SocialChannelsViewModel model) {
            if (!ModelState.IsValid) {
                model.User = CurrentUser;
                return View(model);
            }

            var profile = CurrentUser.As<UserProfilePart>();
            profile.TwitterAlias = model.TwitterAlias.TrimSafe();
            profile.FacebookUrl = model.FacebookUrl.TrimSafe();
            profile.LinkedInUrl = model.LinkedInUrl.TrimSafe();
            profile.CompanyWebsiteUrl = model.CompanyWebsiteUrl.TrimSafe();
            profile.BlogUrl = model.BlogUrl.TrimSafe();
            _notifier.Information(T("Your social channels have been updated."));
            return Redirect(Url.Profile(CurrentUser));
        }
    }
}