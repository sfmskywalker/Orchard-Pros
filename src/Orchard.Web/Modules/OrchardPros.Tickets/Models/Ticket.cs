using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;
using Orchard.Data.Conventions;

namespace OrchardPros.Tickets.Models {
    public class Ticket {
        public virtual int Id { get; set; }

        public virtual int UserId { get; set; }

        [StringLength(256)]
        public virtual string Title { get; set; }
        
        [StringLengthMax]
        public virtual string Description { get; set; }

        public virtual TicketType Type { get; set; }

        public virtual int CategoryId { get; set; }
        
        [StringLengthMax]
        public virtual string Tags { get; set; }

        public virtual decimal? Bounty { get; set; }

        public virtual DateTime DeadlineUtc { get; set; }

        public virtual int ExperiencePoints { get; set; }

        public virtual DateTime CreatedUtc { get; set; }
        public virtual DateTime LastModifiedUtc { get; set; }

        public virtual bool Solved { get; set; }
        public virtual DateTime? SolvedUtc { get; set; }
        public virtual int? AnswerId { get; set; }
    }
}