using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using OrchardPros.Models;
using OrchardPros.Services.Commerce;

namespace OrchardPros.PayoutProviders {
    public class Stripe : PayoutProviderBase, IStripePayoutProvider {
        private readonly IOrchardServices _services;

        public Stripe(IOrchardServices services) {
            _services = services;
        }

        protected override IShape OnBuildDisplay(IShapeFactory shapeFactory) {
            dynamic shape = base.OnBuildDisplay(shapeFactory);
            var settings = _services.WorkContext.CurrentSite.As<StripeSettingsPart>();
            var user = _services.WorkContext.CurrentUser;
            var profile = user.As<UserProfilePart>();
            const string baseUrl = "https://connect.stripe.com/oauth/authorize";
            var parameters = new Dictionary<string, string> {
                {"response_type", "code"},
                {"client_id", settings.ClientId},
                {"stripe_user[email]", user.Email},
                {"stripe_user[country]", profile.Country != null ? profile.Country.Code : ""},
                {"stripe_user[first_name]", profile.FirstName},
                {"stripe_user[last_name]", profile.FullLastName}
            };
            var parametersQuery = parameters.Where(x => !String.IsNullOrWhiteSpace(x.Value));
            var connectUrl = String.Format("{0}?{1}", baseUrl, String.Join("&", parametersQuery.Select(x => String.Format("{0}={1}", x.Key, x.Value))));
            shape.ConnectUrl = connectUrl;
            return shape;
        }
    }
}