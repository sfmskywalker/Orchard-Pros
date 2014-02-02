using System;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Themes;
using OrchardPros.Models;
using OrchardPros.Services;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Authorize, Themed]
    public class StripeController : Controller {
        private readonly ICommerceService _commerceService;
        private readonly IContentManager _contentManager;
        private readonly IStripeClient _stripeClient;
        private readonly ITransactionEventHandler _transactionEventHandler;

        public StripeController(ICommerceService commerceService, IContentManager contentManager, IStripeClient stripeClient, ITransactionEventHandler transactionEventHandler) {

            _commerceService = commerceService;
            _contentManager = contentManager;
            _stripeClient = stripeClient;
            _transactionEventHandler = transactionEventHandler;
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

            try {
                var charge = _stripeClient.CreateCharge((int)(transaction.Amount * 100), "USD", stripeToken, "Bounty");

                if (!charge.Paid) {
                    _commerceService.DeclineTransaction(transaction);
                    return RedirectToAction("PaymentFailed", new {id = id});
                }

                _commerceService.ChargeTransaction(transaction, charge.Id);
                _transactionEventHandler.Charged(new TransactionChargedContext {Transaction = transaction});
                return RedirectToAction("Success", new { id = id });
            }
            catch (Exception) {
                _commerceService.DeclineTransaction(transaction);
                var viewModel = new StripeErrorViewModel {
                    Transaction = transaction
                };
                return View("Error", viewModel);
            }
        }

        public ActionResult Success(string id) {
            var transaction = _commerceService.GetTransaction(id);

            if (transaction == null)
                return HttpNotFound();

            return View(transaction);
        }

        public ActionResult PaymentFailed(string id) {
            var transaction = _commerceService.GetTransaction(id);

            if (transaction == null)
                return HttpNotFound();

            return View(transaction);
        }
    }
}