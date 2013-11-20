using System.Web.Mvc;

namespace OrchardPros.Membership.Helpers {
    public static class UrlExtensions {
        public const string Area = "OrchardPros.Membership";
        public static string AccountSignUp(this UrlHelper url) {
            return url.Action("SignUp", "Account", new { area = Area });
        }
    }
}