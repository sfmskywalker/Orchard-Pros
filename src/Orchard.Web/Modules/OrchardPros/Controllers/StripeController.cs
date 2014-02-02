using System;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Themes;
using OrchardPros.Services;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Authorize, Themed]
    public class StripeController : Controller {
        private readonly ICommerceService _commerceService;
        private readonly IContentManager _contentManager;
        private readonly IStripeClient _stripeClient;

        public StripeController(ICommerceService commerceService, IContentManager contentManager, IStripeClient stripeClient) {

            _commerceService = commerceService;
            _contentManager = contentManager;
            _stripeClient = stripeClient;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Pay(string id) {
            var transaction = _commerceService.GetTransaction(id);

            if (transaction == null)
                return HttpNotFound();

            var user = _contentManager.Get<IUser>(transaction.UserId);
            return View(new StripeViewModel {
                Transaction = transaction,
                User = user
            });
        }

        public ActionResult Charge(string id, string stripeToken) {
            var transaction = _commerceService.GetTransaction(id);

            if (transaction == null)
                return HttpNotFound();

            var charge = _stripeClient.CreateCharge("sk_test_PNCH1IHnneafL8NvQWZnVK2w", (int) (transaction.Amount*100), "USD", stripeToken, "Bounty");
            return RedirectToAction("Success", new { id = id });
        }

        public ActionResult Success(string id) {
            var transaction = _commerceService.GetTransaction(id);

            if (transaction == null)
                return HttpNotFound();

            return View(transaction);
        }
    }
}