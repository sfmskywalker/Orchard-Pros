using System;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Notify;
using OrchardPros.Models;
using OrchardPros.PayoutProviders;
using OrchardPros.Services.Commerce;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Authorize, Themed]
    public class StripeController : Controller {
        private readonly ITransactionService _transactionService;
        private readonly IContentManager _contentManager;
        private readonly IStripeClient _stripeClient;
        private readonly ITransactionEventHandler _transactionEventHandler;
        private readonly INotifier _notifier;
        private readonly IOrchardServices _services;
        private readonly IStripePayoutProvider _stripePayoutProvider;

        public StripeController(
            ITransactionService transactionService, 
            IStripeClient stripeClient, 
            ITransactionEventHandler transactionEventHandler, 
            IOrchardServices services, 
            IStripePayoutProvider stripePayoutProvider) {

            _transactionService = transactionService;
            _contentManager = services.ContentManager;
            _stripeClient = stripeClient;
            _transactionEventHandler = transactionEventHandler;
            _notifier = services.Notifier;
            _services = services;
            _stripePayoutProvider = stripePayoutProvider;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Pay(string id) {
            var transaction = _transactionService.Get(id);

            if (transaction == null)
                return HttpNotFound();

            var user = _contentManager.Get<IUser>(transaction.UserId);
            return View(new StripeViewModel {
                Transaction = transaction,
                User = user
            });
        }

        public ActionResult Charge(string id, string stripeToken) {
            var transaction = _transactionService.Get(id);

            if (transaction == null)
                return HttpNotFound();

            try {
                var charge = _stripeClient.CreateCharge((int)(transaction.Amount * 100), "USD", stripeToken, "Bounty");

                if (!charge.Paid) {
                    _transactionService.Decline(transaction);
                    return RedirectToAction("PaymentFailed", new {id = id});
                }

                _transactionService.Charge(transaction, charge.Id);
                _transactionEventHandler.Charged(new TransactionChargedContext {Transaction = transaction});
                return RedirectToAction("Success", new { id = id });
            }
            catch (Exception) {
                _transactionService.Decline(transaction);
                var viewModel = new StripeErrorViewModel {
                    Transaction = transaction
                };
                return View("Error", viewModel);
            }
        }

        public ActionResult Success(string id) {
            var transaction = _transactionService.Get(id);

            if (transaction == null)
                return HttpNotFound();

            return View(transaction);
        }

        public ActionResult PaymentFailed(string id) {
            var transaction = _transactionService.Get(id);

            if (transaction == null)
                return HttpNotFound();

            return View(transaction);
        }

        // ReSharper disable once InconsistentNaming
        public ActionResult Connect(string code, string error, string error_decription) {

            if (error != null) {
                _notifier.Error(T("We could not connect you with Stripe. The returned message was: {0}", error_decription));
            }
            else {
                var token = _stripeClient.Token(code);
                var profile = _services.WorkContext.CurrentUser.As<UserProfilePart>();
                var providers = profile.PayoutProviders;
                var providerName = _stripePayoutProvider.Name;
                var providerData = (providers.ContainsKey(providerName) ? providers[providerName] : default(UserPayoutProvider)) ?? new UserPayoutProvider {
                    ProviderName = providerName
                };

                providerData.AccessToken = token.AccessToken;
                providerData.RefreshToken = token.RefreshToken;
                providers[providerName] = providerData;
                profile.PayoutProviders = providers;

                _notifier.Information(T("Your account has been succesfully connected with Stripe."));                
            }
            return RedirectToAction("PayoutProviders", "Profile");
        }
    }
}