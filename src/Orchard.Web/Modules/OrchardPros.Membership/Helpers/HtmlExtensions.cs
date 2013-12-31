using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace OrchardPros.Membership.Helpers {
    public static class HtmlExtensions {
        public const string Area = "OrchardPros.Membership";

        public static IHtmlString ProfileLink(this HtmlHelper html, string userName, object htmlAttributes = null) {
            return html.ActionLink(userName, "Index", "Profile", new {userName = userName, area = Area}, htmlAttributes);
        }
    }
}