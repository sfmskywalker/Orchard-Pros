using System;
using System.Collections.Generic;

namespace OrchardPros.Tickets.Models {
    public class TicketSummary {
        public TicketSummary() {
            Categories = new Dictionary<int, string>();
            Tags = new Dictionary<int, string>();
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public TicketType Type { get; set; }
        public IDictionary<int, string> Categories { get; set; }
        public IDictionary<int, string> Tags { get; set; }
        public decimal? Bounty { get; set; }
        public DateTime DeadlineUtc { get; set; }
        public int ExperiencePoints { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastModifiedUtc { get; set; }
        public DateTime? SolvedUtc { get; set; }
        public IList<ReplySummary> Replies { get; set; }
        public TimeSpan TimeLeft { get; set; }
    }
}