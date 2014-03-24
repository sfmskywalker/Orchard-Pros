using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Notify;
using OrchardPros.Models;
using OrchardPros.Services.Content;

namespace OrchardPros.Controllers {
    [Authorize, Themed]
    public class SubscriptionSourceController : Controller {
        private readonly INotifier _notifier;
        private readonly IOrchardServices _services;
        private readonly IContentManager _contentManager;
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionSourceController(IOrchardServices services, IContentManager contentManager, ISubscriptionService subscriptionService) {

            _notifier = services.Notifier;
            _services = services;
            _contentManager = contentManager;
            _subscriptionService = subscriptionService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private IUser CurrentUser {
            get { return _services.WorkContext.CurrentUser; }
        }

        [HttpPost]
        public ActionResult Subscribe(int id, string returnUrl) {
            var subscriptionSource = _contentManager.Get<SubscriptionSourcePart>(id);
            var subscription = subscriptionSource.Subscribers.FirstOrDefault(x => x.Id == CurrentUser.Id);

            if (subscription != null) {
                _notifier.Warning(T("You are already subscribed to this {0}", subscriptionSource.TypeDefinition.DisplayName));
            }
            else {
                _subscriptionService.Subscribe(subscriptionSource, CurrentUser);
                _notifier.Information(T("You are now following <strong>{0}</strong>.", _contentManager.GetItemMetadata(subscriptionSource).DisplayText));
            }

            return Return(returnUrl);
        }

        [HttpPost]
        public ActionResult Unsubscribe(int id, string returnUrl) {
            var subscriptionSource = _contentManager.Get<SubscriptionSourcePart>(id);
            var subscription = subscriptionSource.Subscribers.FirstOrDefault(x => x.Id == CurrentUser.Id);

            if (subscription == null) {
                _notifier.Warning(T("You are already unsubscribed from this {0}", subscriptionSource.TypeDefinition.DisplayName));
            }
            else {
                _subscriptionService.Unsubscribe(subscriptionSource, CurrentUser);
                _notifier.Information(T("You are no longer following <strong>{0}</strong>.", _contentManager.GetItemMetadata(subscriptionSource).DisplayText));
            }

            return Return(returnUrl);
        }

        private ActionResult Return(string returnUrl) {
            var urlReferrer = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : default(string);
            return Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl)
                : Url.IsLocalUrl(urlReferrer)
                    ? Redirect(urlReferrer)
                    : Redirect("~/");
        }
    }
}