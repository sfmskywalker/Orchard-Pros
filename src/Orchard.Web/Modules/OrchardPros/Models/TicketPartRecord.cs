using System;
using Orchard.ContentManagement.Records;

namespace OrchardPros.Models {
    public class TicketPartRecord : ContentPartRecord {
        public virtual TicketType Type { get; set; }
        public virtual decimal? Bounty { get; set; }
        public virtual DateTime DeadlineUtc { get; set; }
        public virtual int ExperiencePoints { get; set; }
        public virtual DateTime? SolvedUtc { get; set; }
        public virtual int? AnswerId { get; set; }
        public virtual int? SolvedByUserId { get; set; }
        public virtual string Categories { get; set; }
        public virtual string Tags { get; set; }
    }
}