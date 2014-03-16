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
    public class BioController : Controller {
        private readonly INotifier _notifier;
        private readonly IOrchardServices _services;

        public BioController(IOrchardServices services) {
            _notifier = services.Notifier;
            _services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private IUser CurrentUser {
            get { return _services.WorkContext.CurrentUser; }
        }

        public ActionResult Edit() {
            var viewModel = new BioViewModel {
                Bio = CurrentUser.As<UserProfilePart>().Bio,
                User = CurrentUser
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(BioViewModel model) {
            if (!ModelState.IsValid) {
                model.User = CurrentUser;
                return View(model);
            }

            var profile = CurrentUser.As<UserProfilePart>();
            profile.Bio = model.Bio.TrimSafe();
            _notifier.Information(T("Your Bio has been updated."));
            return Redirect(Url.Profile(CurrentUser));
        }
    }
}