using System;
using System.Collections.Generic;
using System.Linq;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Helpers {
    public static class TicketHelpers {
        public static IEnumerable<string> Tags(this Ticket ticket) {
            return !String.IsNullOrWhiteSpace(ticket.Tags)
                ? ticket.Tags.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower())
                : Enumerable.Empty<string>();
        }

        public static TimeSpan TimeLeft(this Ticket ticket) {
            var timeSpan = ticket.DeadlineUtc - DateTime.Now;
            return timeSpan.Ticks < 0 ? TimeSpan.Zero : timeSpan;
        }
    }
}