using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NGM.OpenAuthentication.Models;
using NGM.OpenAuthentication.Services;
using NGM.OpenAuthentication.Services.Clients;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Orchard.Workflows.Services;
using OrchardPros.Membership.ViewModels;

namespace OrchardPros.Membership.Controllers {
    [Themed, Authorize]
    public class ProfileController : Controller {
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;
        private readonly IOrchardServices _services;

        public ProfileController(
            IShapeFactory shapeFactory, 
            IMembershipService membershipService, 
            IUserService userService, 
            IOrchardServices services) {

            New = shapeFactory;
            T = NullLocalizer.Instance;
            _membershipService = membershipService;
            _userService = userService;
            _services = services;
        }

        public Localizer T { get; set; }
        private dynamic New { get; set; }

        public ActionResult Index(string id = null) {
            var user = String.IsNullOrWhiteSpace(id) ? _services.WorkContext.CurrentUser : _membershipService.GetUser(id);
            var profileShape = Wrap(New.Profile(User: user, IsCurrentUser: _services.WorkContext.CurrentUser.Id == user.Id), user);
            return new ShapeResult(this, profileShape);
        }

        public ActionResult TicketsCreated() {
            var ticketsCreated = Wrap(New.Profile_TicketsCreated());
            return new ShapeResult(this, ticketsCreated);
        }

        public ActionResult TicketsFollowed() {
            var ticketsFollowed = Wrap(New.Profile_TicketsFollowed());
            return new ShapeResult(this, ticketsFollowed);
        }

        public ActionResult Settings() {
            var settings = Wrap(New.Profile_Settings());
            return new ShapeResult(this, settings);
        }

        private dynamic Wrap(dynamic shape, IUser user = null) {
            if (Request.IsAjaxRequest())
                return shape;

            var wrapper = New.Profile_Wrapper();
            wrapper.Add(shape);
            wrapper.User = user ?? _services.WorkContext.CurrentUser;
            return wrapper;
        }
    }
}