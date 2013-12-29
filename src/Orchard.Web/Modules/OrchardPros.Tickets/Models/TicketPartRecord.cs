using System;
using Orchard.ContentManagement.Records;

namespace OrchardPros.Tickets.Models {
    public class TicketPartRecord : ContentPartRecord {
        public virtual TicketType Type { get; set; }
        public decimal? Bounty { get; set; }
        public DateTime DeadlineUtc { get; set; }
        public int ExperiencePoints { get; set; }
        public DateTime? SolvedUtc { get; set; }
        public int? AnswerId { get; set; }
    }
}