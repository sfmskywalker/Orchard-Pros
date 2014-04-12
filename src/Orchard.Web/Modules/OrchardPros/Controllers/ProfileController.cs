using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NGM.OpenAuthentication.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Users.Services;
using OrchardPros.Helpers;
using OrchardPros.Models;
using OrchardPros.Services.Commerce;
using OrchardPros.Services.Content;
using OrchardPros.Services.Notifications;
using OrchardPros.Services.Security;
using OrchardPros.Services.User;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Themed, Authorize]
    public class ProfileController : Controller {
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;
        private readonly IOrchardServices _services;
        private readonly IAccountServices _accountServices;
        private readonly IAuthenticationService _authenticationService;
        private readonly INotificationSettingsManager _notificationSettingsManager;
        private readonly ITicketService _ticketService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IRepository<Country> _countryRepository;
        private readonly IOpenAuthServices _openAuthServices;
        private readonly Lazy<IEnumerable<IPayoutProvider>> _payoutProviders;
        private readonly ITransferService _transferService;

        public ProfileController(
            IShapeFactory shapeFactory, 
            IMembershipService membershipService, 
            IUserService userService, 
            IOrchardServices services, 
            IAccountServices accountServices,
            IAuthenticationService authenticationService, 
            INotificationSettingsManager notificationSettingsManager, 
            ITicketService ticketService, 
            ISubscriptionService subscriptionService, 
            IRepository<Country> countryRepository, 
            IOpenAuthServices openAuthServices, 
            Lazy<IEnumerable<IPayoutProvider>> payoutProviders, 
            ITransferService transferService) {

            New = shapeFactory;
            T = NullLocalizer.Instance;
            _membershipService = membershipService;
            _userService = userService;
            _services = services;
            _accountServices = accountServices;
            _authenticationService = authenticationService;
            _notificationSettingsManager = notificationSettingsManager;
            _ticketService = ticketService;
            _subscriptionService = subscriptionService;
            _countryRepository = countryRepository;
            _openAuthServices = openAuthServices;
            _payoutProviders = payoutProviders;
            _transferService = transferService;
        }

        public Localizer T { get; set; }
        private dynamic New { get; set; }

        [AllowAnonymous]
        public ActionResult Index(string userName) {
            var user = GetUser(userName);
            var currentUserId = _services.WorkContext.CurrentUser != null ? _services.WorkContext.CurrentUser.Id : default(int?);
            var profileShape = Wrap(New.Profile(User: user, IsCurrentUser: currentUserId == user.Id), user);
            return new ShapeResult(this, profileShape);
        }

        [AllowAnonymous]
        public ActionResult TicketsCreated(string userName, PagerParameters pagerParameters) {
            var pager = new Pager(_services.WorkContext.CurrentSite, pagerParameters);
            var user = GetUser(userName);
            var tickets = _ticketService.GetTicketsCreatedBy(user.Id, pager.GetStartIndex(), pager.PageSize);
            var pagerShape = New.Pager(pager).TotalItemCount(tickets.TotalItemCount);
            var ticketsCreated = Wrap(New.Profile_TicketsCreated(Tickets: tickets, Pager: pagerShape), user);
            return new ShapeResult(this, ticketsCreated);
        }

        public ActionResult TicketsFollowed(PagerParameters pagerParameters) {
            var pager = new Pager(_services.WorkContext.CurrentSite, pagerParameters);
            var user = GetUser();
            var tickets = _subscriptionService.GetSubscriptionSourcesByUser(user.Id).As<TicketPart>();
            var pagerShape = New.Pager(pager).TotalItemCount(tickets.TotalItemCount);
            var ticketsFollowed = Wrap(New.Profile_TicketsFollowed(Tickets: tickets, Pager: pagerShape));
            return new ShapeResult(this, ticketsFollowed);
        }

        public ActionResult TicketsSolved(PagerParameters pagerParameters) {
            var pager = new Pager(_services.WorkContext.CurrentSite, pagerParameters);
            var user = GetUser();
            var tickets = _ticketService.GetTicketsSolvedBy(user.Id, pager.GetStartIndex(), pager.PageSize);
            var pagerShape = New.Pager(pager).TotalItemCount(tickets.TotalItemCount);
            var ticketsFollowed = Wrap(New.Profile_TicketsFollowed(Tickets: tickets, Pager: pagerShape));
            return new ShapeResult(this, ticketsFollowed);
        }

        public ActionResult Transfers(PagerParameters pagerParameters) {
            var pager = new Pager(_services.WorkContext.CurrentSite, pagerParameters);
            var user = GetUser();
            var transfers = _transferService.GetTransfersByUser(user.Id, pager.GetStartIndex(), pager.PageSize);
            var transferReport = _transferService.GetTranferReportByUser(user.Id);
            var ticketIds = transfers.Select(x => Int32.Parse(x.Context));
            var tickets = _services.ContentManager.GetMany<TicketPart>(ticketIds, VersionOptions.Published, QueryHints.Empty).ToDictionary(x => x.Id);
            var pagerShape = New.Pager(pager).TotalItemCount(transfers.TotalItemCount);
            var transfersShape = Wrap(New.Profile_Transfers(Transfers: transfers, Pager: pagerShape, TicketsDictionary: tickets, TransferReport: transferReport));
            return new ShapeResult(this, transfersShape);
        }

        public ActionResult PayoutProviders() {
            var user = GetUser();
            var userProfilePart = user.As<UserProfilePart>();
            var availableProviders = _payoutProviders.Value.Select(x => x.BuildDisplay(New)).ToArray();
            var connectedProviders = userProfilePart.PayoutProviders;
            var shape = Wrap(New.Profile_PayoutProviders(AvailableProviders: availableProviders, ConnectedProviders: connectedProviders));
            return new ShapeResult(this, shape);
        }

        public ActionResult ConnectedApps() {
            var user = GetUser();
            var userProvidersPart = user.As<UserProvidersPart>();
            var availableApps = _openAuthServices.GetProviders().ToArray();
            var connectedApps = userProvidersPart.Providers;
            var shape = Wrap(New.Profile_ConnectedApps(AvailableApps: availableApps, ConnectedApps: connectedApps));
            return new ShapeResult(this, shape);
        }

        public ActionResult Settings() {
            var currentUser = _services.WorkContext.CurrentUser;
            var profilePart = currentUser.As<UserProfilePart>();
            var notificationSettings = profilePart.NotificationSettings.ToDictionary(x => x.Name);
            var settingsModel = new AccountSettingsViewModel {
                Email = currentUser.Email,
                UserName = currentUser.UserName,
                AvatarType = profilePart.AvatarType,
                FirstName = profilePart.FirstName,
                MiddleName = profilePart.MiddleName,
                LastName = profilePart.LastName,
                CountryId = profilePart.Country != null ? profilePart.Country.Id : default(int?),
                Notifications = _notificationSettingsManager.GetNotificationSettings().Select(x => new NotificationSettingViewModel {
                    Name = x.Name,
                    Description = x.Description,
                    Checked = notificationSettings.ContainsKey(x.Name)
                }).ToList()
            };
            var shape = Wrap(New.Profile_Settings(
                AccountSettings: settingsModel,
                User: currentUser,
                Countries: _countryRepository.Table.OrderBy(x => x.Name).ToArray()));
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
                var shape = Wrap(New.Profile_Settings(
                    AccountSettings: accountSettings,
                    User: currentUser,
                    Countries: _countryRepository.Table.ToArray()));
                return new ShapeResult(this, shape);    
            }

            profilePart.FirstName = accountSettings.FirstName.TrimSafe();
            profilePart.MiddleName = accountSettings.MiddleName.TrimSafe();
            profilePart.LastName = accountSettings.LastName.TrimSafe();
            profilePart.Country = accountSettings.CountryId != null ? _countryRepository.Get(accountSettings.CountryId.Value) : null;
            profilePart.AvatarType = accountSettings.AvatarType;

            if (accountSettings.DeleteAvatar == true) {
                _accountServices.DeleteAvatar(currentUser);
            }

            if (accountSettings.DeleteWallpaper == true) {
                _accountServices.DeleteWallpaper(currentUser);
            }

            var avatarFile = Request.Files["AvatarFile"];
            if (avatarFile.ContentLength > 0) {
                _accountServices.UpdateAvatar(currentUser, avatarFile);
            }

            var wallpaperFile = Request.Files["WallpaperFile"];
            if (wallpaperFile.ContentLength > 0) {
                _accountServices.UpdateWallpaper(currentUser, wallpaperFile);
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