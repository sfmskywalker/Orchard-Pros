using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NGM.OpenAuthentication.Models;
using NGM.OpenAuthentication.Services;
using NGM.OpenAuthentication.Services.Clients;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Orchard.Workflows.Services;
using OrchardPros.Membership.ViewModels;

namespace OrchardPros.Membership.Controllers {
    [Themed, Authorize]
    public class AccountController : Controller {
        private readonly IWorkflowManager _workFlowManager;
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserEventHandler _userEventHandler;
        private readonly IEnumerable<IExternalAuthenticationClient> _openAuthClients;
        private readonly IOrchardOpenAuthClientProvider _orchardOpenAuthClientProvider;

        public AccountController(
            IShapeFactory shapeFactory, 
            IWorkflowManager workFlowManager, 
            IMembershipService membershipService, 
            IUserService userService, 
            IAuthenticationService authenticationService,
            IUserEventHandler userEventHandler, 
            IEnumerable<IExternalAuthenticationClient> openAuthClients,
            IOrchardOpenAuthClientProvider orchardOpenAuthClientProvider) {

            New = shapeFactory;
            T = NullLocalizer.Instance;
            _workFlowManager = workFlowManager;
            _membershipService = membershipService;
            _userService = userService;
            _authenticationService = authenticationService;
            _userEventHandler = userEventHandler;
            _openAuthClients = openAuthClients;
            _orchardOpenAuthClientProvider = orchardOpenAuthClientProvider;
        }

        public Localizer T { get; set; }
        private dynamic New { get; set; }

        [AllowAnonymous]
        public ActionResult SignUp() {
            var formViewModel = new SignUpViewModel();
            var viewModel = New.ViewModel(
                SignUp: New.SignUp(Model: formViewModel),
                OAuthLogin: New.OAuthLogin(Providers: GetOAuthProviders().ToList()));
            return View(viewModel);
        }

        [HttpPost, AllowAnonymous]
        public ActionResult SignUp(SignUpViewModel model) {
            if (ModelState.IsValid) {
                if (!_userService.VerifyUserUnicity(model.UserName, model.EmailAddress)) {
                    ModelState.AddModelError("UserName", T("The specified username and/or email address are already in use. Please retry with a different username and/or email address"));
                }
            }
            
            if (!ModelState.IsValid) {
                var viewModel = New.ViewModel(
                    SignUp: New.SignUp(Model: model),
                    OAuthLogin: New.OAuthLogin(Providers: GetOAuthProviders().ToList()));
                return View(viewModel);
            }
            
            _membershipService.CreateUser(new CreateUserParams(model.UserName, model.Password, model.EmailAddress, null, null, false));
            return Response.IsRequestBeingRedirected ? (ActionResult) new EmptyResult() : RedirectToAction("Created");
        }

        public ActionResult Created() {
            return View();
        }

        [AllowAnonymous]
        public ActionResult SignIn() {
            var formViewModel = new SignInViewModel();
            var viewModel = New.ViewModel(
                SignIn: New.SignIn(ModelState: formViewModel),
                OAuthLogin: New.OAuthLogin(Providers: GetOAuthProviders().ToList()));
            return View(viewModel);
        }

        [HttpPost, AllowAnonymous]
        public ActionResult SignIn(SignInViewModel model) {
            IUser user = null;

            if (ModelState.IsValid) {
                user = _membershipService.ValidateUser(model.UserNameOrEmailAddress, model.Password);

                if (user == null) {
                    ModelState.AddModelError("_FORM", T("The username or e-mail or password provided is incorrect."));
                }
            }
            if (!ModelState.IsValid) {
                var viewModel = New.ViewModel(
                    SignIn: New.SignIn(ModelState: model),
                    OAuthLogin: New.OAuthLogin(Providers: GetOAuthProviders().ToList()));
                return View(viewModel);
            }

            _authenticationService.SignIn(user, true);
            _userEventHandler.LoggedIn(user);
            return Response.IsRequestBeingRedirected ? (ActionResult)new EmptyResult() : RedirectToAction("Dashboard");
        }

        public ActionResult SignOut() {
            var wasLoggedInUser = _authenticationService.GetAuthenticatedUser();
            _authenticationService.SignOut();
            if (wasLoggedInUser != null)
                _userEventHandler.LoggedOut(wasLoggedInUser);
            return Response.IsRequestBeingRedirected ? (ActionResult)new EmptyResult() : Redirect("~/");
        }

        public ActionResult Dashboard() {
            return View();
        }

        private IEnumerable<OrchardAuthenticationClientData> GetOAuthProviders() {
            return _openAuthClients.Select(x => _orchardOpenAuthClientProvider.GetClientData(x.ProviderName)).Where(x => x != null);
        }
    }
}