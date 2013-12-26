using System;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.ViewModels {
    public class TicketRow {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Categories { get; set; }
        public string Description { get; set; }
        public TicketType Type { get; set; }
        public string Title { get; set; }
        public string Tags { get; set; }
        public decimal? Bounty { get; set; }
        public DateTime DeadlineUtc { get; set; }
        public int ExperiencePoints { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastModifiedUtc { get; set; }
        public DateTime? SolvedUtc { get; set; }
        public int? AnswerId { get; set; }
    }
}