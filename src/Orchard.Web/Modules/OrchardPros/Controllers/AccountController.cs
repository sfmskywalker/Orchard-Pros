using System;
using System.Web.Mvc;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Users.Events;
using Orchard.Users.Services;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Themed, Authorize]
    public class AccountController : Controller {
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserEventHandler _userEventHandler;

        public AccountController(
            IShapeFactory shapeFactory,
            IMembershipService membershipService, 
            IUserService userService, 
            IAuthenticationService authenticationService,
            IUserEventHandler userEventHandler) {

            New = shapeFactory;
            T = NullLocalizer.Instance;
            _membershipService = membershipService;
            _userService = userService;
            _authenticationService = authenticationService;
            _userEventHandler = userEventHandler;
        }

        public Localizer T { get; set; }
        private dynamic New { get; set; }

        [AllowAnonymous]
        public ActionResult SignUp() {
            var formViewModel = new SignUpViewModel();
            var viewModel = New.ViewModel(SignUp: New.SignUp(Model: formViewModel));
            return View(viewModel);
        }

        [HttpPost, AllowAnonymous]
        public ActionResult SignUp(SignUpViewModel model) {
            if (ModelState.IsValid) {
                if (!_userService.VerifyUserUnicity(model.UserName, model.Email)) {
                    ModelState.AddModelError("UserName", T("The specified username and/or email address are already in use. Please retry with a different username and/or email address"));
                }
            }
            
            if (!ModelState.IsValid) {
                var viewModel = New.SignUp(Model: model);
                return View(viewModel);
            }
            
            _membershipService.CreateUser(new CreateUserParams(model.UserName, model.Password, model.Email, null, null, false));
            return Response.IsRequestBeingRedirected ? (ActionResult) new EmptyResult() : RedirectToAction("Created");
        }

        public ActionResult Created() {
            return View();
        }

        [AllowAnonymous]
        public ActionResult SignIn(string returnUrl) {
            return View(New.SignIn(ReturnUrl: returnUrl));
        }

        [HttpPost, AllowAnonymous]
        public ActionResult SignIn(string userNameOrEmail, string password, string returnUrl)
        {
            IUser user = null;

            if (ModelState.IsValid) {
                user = _membershipService.ValidateUser(userNameOrEmail, password);

                if (user == null) {
                    ModelState.AddModelError("_FORM", T("The username or e-mail or password provided is incorrect."));
                }
            }
            if (!ModelState.IsValid) {
                var viewModel = New.ViewModel(SignIn: New.SignIn());
                return View(viewModel);
            }

            _authenticationService.SignIn(user, true);
            _userEventHandler.LoggedIn(user);
            return Response.IsRequestBeingRedirected ? new EmptyResult() : Url.IsLocalUrl(returnUrl) ? (ActionResult) Redirect(returnUrl) : RedirectToAction("Index", "Profile");
        }

        public ActionResult SignOut() {
            var wasLoggedInUser = _authenticationService.GetAuthenticatedUser();
            _authenticationService.SignOut();
            if (wasLoggedInUser != null)
                _userEventHandler.LoggedOut(wasLoggedInUser);
            return Response.IsRequestBeingRedirected ? (ActionResult)new EmptyResult() : Redirect("~/");
        }
    }
}