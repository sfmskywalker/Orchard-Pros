using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Taxonomies.Models;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Helpers {
    public static class TicketHelpers {
        public static string ToDelimitedString(this IEnumerable<TermPart> terms) {
            return String.Join(",", terms.Select(x => x.Name));
        }

        public static IDictionary<int, string> CategoriesDictionary(this TicketPart ticket, IDictionary<int, string> categoryDictionary) {
            return ticket.Categories.ToDictionary(x => x.CategoryId, x => categoryDictionary[x.CategoryId]);
        }

        public static IDictionary<int, string> TagsDictionary(this Ticket ticket, IDictionary<int, string> tagDictionary) {
            return ticket.Tags.Sanitize(tagDictionary).ToDictionary(x => x.TagId, x => tagDictionary[x.TagId]);
        }

        public static DateTime LastModifiedUtc(this Ticket ticket) {
            var lastReply = ticket.Replies.LastOrDefault();
            return lastReply != null ? lastReply.ModifiedUtc : ticket.ModifiedUtc;
        }

        public static IUser LastModifier(this Ticket ticket, IContentManager contentManager) {
            var lastReply = ticket.Replies.LastOrDefault();
            var lastModifierId = lastReply != null ? lastReply.UserId : ticket.UserId;
            return contentManager.Get<IUser>(lastModifierId);
        }

        public static LocalizedString GetExpirationText(this Ticket ticket, Localizer T) {
            return GetExpirationText(ticket.DeadlineUtc, ticket.TimeLeft(), T);
        }

        public static LocalizedString GetExpirationText(this TicketSummary ticket, Localizer T) {
            return GetExpirationText(ticket.DeadlineUtc, ticket.TimeLeft, T);
        }

        private static LocalizedString GetExpirationText(DateTime deadlineUtc, TimeSpan timeLeft, Localizer T) {
            return timeLeft > TimeSpan.Zero
                ? T("Expires {0}d {1}h {2}m", timeLeft.Days, timeLeft.Hours, timeLeft.Minutes)
                : T("Expired on {0}", deadlineUtc);
        }
    }
}