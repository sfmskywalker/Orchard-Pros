using System;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Notify;
using Orchard.Users.Services;
using OrchardPros.Membership.Helpers;
using OrchardPros.Membership.Models;
using OrchardPros.Membership.Services;
using OrchardPros.Membership.ViewModels;

namespace OrchardPros.Membership.Controllers {
    [Themed, Authorize]
    public class ProfileController : Controller {
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;
        private readonly IOrchardServices _services;
        private readonly IAccountServices _accountServices;
        private readonly IAuthenticationService _authenticationService;
        private readonly INotificationSettingsManager _notificationSettingsManager;

        public ProfileController(
            IShapeFactory shapeFactory, 
            IMembershipService membershipService, 
            IUserService userService, 
            IOrchardServices services, 
            IAccountServices accountServices,
            IAuthenticationService authenticationService, INotificationSettingsManager notificationSettingsManager) {

            New = shapeFactory;
            T = NullLocalizer.Instance;
            _membershipService = membershipService;
            _userService = userService;
            _services = services;
            _accountServices = accountServices;
            _authenticationService = authenticationService;
            _notificationSettingsManager = notificationSettingsManager;
        }

        public Localizer T { get; set; }
        private dynamic New { get; set; }

        public ActionResult Index(string userName) {
            var user = GetUser(userName);
            var profileShape = Wrap(New.Profile(User: user, IsCurrentUser: _services.WorkContext.CurrentUser.Id == user.Id), user);
            return new ShapeResult(this, profileShape);
        }

        public ActionResult TicketsCreated(string userName) {
            var user = GetUser(userName);
            var ticketsCreated = Wrap(New.Profile_TicketsCreated(), user);
            return new ShapeResult(this, ticketsCreated);
        }

        public ActionResult TicketsFollowed() {
            var ticketsFollowed = Wrap(New.Profile_TicketsFollowed());
            return new ShapeResult(this, ticketsFollowed);
        }

        public ActionResult Settings() {
            var currentUser = _services.WorkContext.CurrentUser;
            var profilePart = currentUser.As<UserProfilePart>();
            var notificationSettings = profilePart.NotificationSettings.ToDictionary(x => x.Name);
            var settingsModel = new AccountSettingsViewModel {
                Email = currentUser.Email,
                UserName = currentUser.UserName,
                Notifications = _notificationSettingsManager.GetNotificationSettings().Select(x => new NotificationSettingViewModel {
                    Name = x.Name,
                    Description = x.Description,
                    Checked = notificationSettings.ContainsKey(x.Name)
                }).ToList()
            };
            var shape = Wrap(New.Profile_Settings(AccountSettings: settingsModel));
            return new ShapeResult(this, shape);
        }

        [HttpPost]
        public ActionResult Settings(AccountSettingsViewModel accountSettings) {
            var currentUser = _services.WorkContext.CurrentUser;
            var profilePart = currentUser.As<UserProfilePart>();

            if (ModelState.IsValid) {
                var postedUserName = accountSettings.UserName.TrimSafe();
                var postedEmail = accountSettings.Email.TrimSafe();

                if (currentUser.UserName != accountSettings.UserName || currentUser.Email != postedEmail) {
                    if (!_userService.VerifyUserUnicity(currentUser.Id, postedUserName, postedEmail)) {
                        ModelState.AddModelError("_FORM", T("The specified username and or email are already taken by another account.").ToString());
                    }
                    else {
                        if (currentUser.UserName != accountSettings.UserName) {
                            _accountServices.ChangeUserName(currentUser, postedUserName);
                        }
                        if (currentUser.Email != postedEmail) {
                            _accountServices.ChangeUserEmail(currentUser, postedEmail);
                        }
                        _authenticationService.SignOut();
                        _authenticationService.SignIn(currentUser, true);
                    }
                }

                if (!String.IsNullOrWhiteSpace(accountSettings.Password)) {
                    _membershipService.SetPassword(currentUser, accountSettings.Password);
                }

            }
            if (!ModelState.IsValid) {
                var shape = Wrap(New.Profile_Settings());
                return new ShapeResult(this, shape);    
            }

            profilePart.NotificationSettings = accountSettings.Notifications.Where(x => x.Checked).Select(x => new NotificationSetting {Name = x.Name}).ToArray();
            
            _services.Notifier.Information(T("Your settings have been updated."));
            return RedirectToAction("Settings");
        }

        private dynamic Wrap(dynamic shape, IUser user = null) {
            if (Request.IsAjaxRequest())
                return shape;

            var wrapper = New.Profile_Wrapper();
            wrapper.Add(shape);
            wrapper.User = user ?? _services.WorkContext.CurrentUser;
            return wrapper;
        }

        private IUser GetUser(string userName = null) {
            return String.IsNullOrWhiteSpace(userName) ? _services.WorkContext.CurrentUser : _membershipService.GetUser(userName);
        }
    }
}