using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Taxonomies.Models;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Helpers {
    public static class TicketHelpers {
        public static TimeSpan TimeLeft(this Ticket ticket) {
            var timeSpan = ticket.DeadlineUtc - DateTime.Now;
            return timeSpan.Ticks < 0 ? TimeSpan.Zero : timeSpan;
        }

        public static string ToDelimitedString(this IEnumerable<TermPart> terms) {
            return String.Join(",", terms.Select(x => x.Name));
        }

        public static IEnumerable<TicketTag> Sanitize(this IEnumerable<TicketTag> tags, IDictionary<int, string> tagDictionary) {
            return tags.Where(x => tagDictionary.ContainsKey(x.TagId));
        }

        public static IEnumerable<TicketCategory> Sanitize(this IEnumerable<TicketCategory> categories, IDictionary<int, string> categoryDictionary) {
            return categories.Where(x => categoryDictionary.ContainsKey(x.CategoryId));
        }
    }
}