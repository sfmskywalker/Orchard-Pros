using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Orchard.ContentManagement;
using Orchard.Mvc.Html;
using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.Helpers {
    public static class HtmlExtensions {
        public const string Area = "OrchardPros";

        public static IHtmlString ProfileLink(this HtmlHelper html, IUser user, object htmlAttributes = null) {
            return html.ProfileLink(user.As<UserProfilePart>().DisplayName, user.UserName, htmlAttributes);
        }

        public static IHtmlString ProfileLink(this HtmlHelper html, string linkText, IUser user, object htmlAttributes = null) {
            return html.ProfileLink(linkText, user.UserName, htmlAttributes);
        }

        public static IHtmlString ProfileLink(this HtmlHelper html, string userName, object htmlAttributes = null) {
            return html.ProfileLink(userName, userName, htmlAttributes);
        }

        public static IHtmlString ProfileLink(this HtmlHelper html, string linkText, string userName, object htmlAttributes = null) {
            return html.ActionLink(linkText, "Index", "Profile", new { userName = userName, area = Area }, htmlAttributes);
        }

        public static IHtmlString TicketDetailsLink(this HtmlHelper html, TicketPart ticket, object htmlAttributes = null) {
            return html.ItemDisplayLink(ticket, htmlAttributes);
        }
    }
}