using System.Collections.Generic;
using System.Web.Mvc;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Users.Services;
using Orchard.Workflows.Services;
using OrchardPros.Membership.ViewModels;

namespace OrchardPros.Membership.Controllers {
    [Themed]
    public class AccountController : Controller {
        private readonly IWorkflowManager _workFlowManager;
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;

        public AccountController(IShapeFactory shapeFactory, IWorkflowManager workFlowManager, IMembershipService membershipService, IUserService userService) {
            New = shapeFactory;
            T = NullLocalizer.Instance;
            _workFlowManager = workFlowManager;
            _membershipService = membershipService;
            _userService = userService;
        }

        public Localizer T { get; set; }
        private dynamic New { get; set; }

        public ActionResult SignUp() {
            var formViewModel = new SignupFormViewModel();
            var viewModel = New.ViewModel().SignUpForm(New.SignUpForm(Model: formViewModel));
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SignUp(SignupFormViewModel model) {
            if (ModelState.IsValid) {
                if (!_userService.VerifyUserUnicity(model.UserName, model.EmailAddress)) {
                    ModelState.AddModelError("UserName", T("The specified username and/or email address are already in use. Please retry with a different username and/or email address").ToString());
                }
            }
            
            if (!ModelState.IsValid) {
                var viewModel = New.ViewModel().SignUpForm(New.SignUpForm(Model: model));
                return View(viewModel);
            }
            
            var user = _membershipService.CreateUser(new CreateUserParams(model.UserName, model.Password, model.EmailAddress, null, null, false));
            _workFlowManager.TriggerEvent("AccountCreated", user, () => new Dictionary<string, object>());
            
            return new EmptyResult();
        }

        public ActionResult Created() {
            return View();
        }
    }
}