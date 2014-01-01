using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Orchard.Mvc.Html;
using OrchardPros.Models;

namespace OrchardPros.Helpers {
    public static class HtmlExtensions {
        public const string Area = "OrchardPros";

        public static IHtmlString ProfileLink(this HtmlHelper html, string userName, object htmlAttributes = null) {
            return html.ActionLink(userName, "Index", "Profile", new {userName = userName, area = Area}, htmlAttributes);
        }

        public static IHtmlString TicketDetailsLink(this HtmlHelper html, TicketPart ticket, object htmlAttributes = null) {
            return html.ItemDisplayLink(ticket, htmlAttributes);
        }
    }
}