using System.Web.Mvc;
using Orchard.Themes;

namespace OrchardPros.Membership.Controllers {
    [Themed]
    public class AccountController : Controller {
        public ActionResult SignUp() {
            return View();
        }
    }
}