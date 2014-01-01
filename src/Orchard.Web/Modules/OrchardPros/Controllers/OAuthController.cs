using System.Web.Mvc;
using NGM.OpenAuthentication.Extensions;
using NGM.OpenAuthentication.Mvc;
using Orchard.Themes;

namespace OrchardPros.Controllers {
    [Themed]
    public class OAuthController : Controller {
        public ActionResult SignIn(string id, string returnUrl) {
            return new OpenAuthLoginResult(id, Url.OpenAuthLogOn(returnUrl));
        }
    }
}