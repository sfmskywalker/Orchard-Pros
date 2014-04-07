using System.Web.Mvc;
using Orchard.Mvc.Html;
using Orchard.Security;
using Orchard.Taxonomies.Models;
using OrchardPros.Models;

namespace OrchardPros.Helpers {
    public static class UrlExtensions {
        public const string Area = "OrchardPros";
        
        public static string AccountSignUp(this UrlHelper url) {
            return url.Action("SignUp", "Account", new { area = Area });
        }

        public static string AccountSignIn(this UrlHelper url) {
            return url.Action("SignIn", "Account", new { area = Area });
        }

        public static string AccountSignOut(this UrlHelper url) {
            return url.Action("SignOut", "Account", new { area = Area });
        }

        public static string Profile(this UrlHelper url, IUser user) {
            return url.Profile(user.UserName);
        }

        public static string Profile(this UrlHelper url, string userName) {
            return url.Action("Index", "Profile", new { userName = userName, area = Area });
        }

        public static string DownloadAttachment(this UrlHelper url, AttachmentPart attachment) {
            return url.Action("Download", "Attachment", new {id = attachment.UniqueIdentifier, area = Area});
        }

        public static string TicketDetails(this UrlHelper url, TicketPart ticket) {
            return url.ItemDisplayUrl(ticket);
        }

        public static string Reply(this UrlHelper url, ReplyPart reply) {
            return url.ItemDisplayUrl(reply.ContainingContent);
        }

        public static string Category(this UrlHelper url, TermPart category) {
            return url.Action("Index", "Ticket", new { categoryId = category.Id, area = Area });
        }

        public static string Tag(this UrlHelper url, TermPart tag) {
            return url.Action("Index", "Ticket", new { tagId = tag.Id, area = Area });
        }
    }
}