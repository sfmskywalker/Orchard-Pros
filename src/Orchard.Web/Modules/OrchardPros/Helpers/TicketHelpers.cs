using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Localization;
using Orchard.Taxonomies.Models;
using OrchardPros.Models;

namespace OrchardPros.Helpers {
    public static class TicketHelpers {
        public static string ToDelimitedString(this IEnumerable<TermPart> terms) {
            return String.Join(",", terms.Select(x => x.Name));
        }

        public static LocalizedString GetExpirationText(this TicketPart ticket, Localizer T) {
            var timeLeft = ticket.RemainingTime;
            var deadlineUtc = ticket.DeadlineUtc;
            return timeLeft > TimeSpan.Zero
                ? T("Expires {0}d {1}h {2}m", timeLeft.Days, timeLeft.Hours, timeLeft.Minutes)
                : T("Expired on {0}", deadlineUtc);
        }
    }
}