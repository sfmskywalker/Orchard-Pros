using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.Data.Conventions;

namespace OrchardPros.Tickets.Models {
    public class Ticket {
        public Ticket() {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Categories = new List<TicketCategory>();
            Tags = new List<TicketTag>();
            Attachments = new List<Attachment>();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public virtual int Id { get; set; }

        public virtual int UserId { get; set; }

        [StringLength(256)]
        public virtual string Title { get; set; }
        
        [StringLengthMax]
        public virtual string Description { get; set; }

        public virtual TicketType Type { get; set; }

        public virtual IList<TicketCategory> Categories { get; set; }

        public virtual IList<TicketTag> Tags { get; set; }

        public virtual decimal? Bounty { get; set; }

        public virtual DateTime DeadlineUtc { get; set; }

        public virtual int ExperiencePoints { get; set; }

        public virtual int ViewCount { get; set; }

        public virtual DateTime CreatedUtc { get; set; }
        public virtual DateTime ModifiedUtc { get; set; }

        public virtual DateTime? SolvedUtc { get; set; }
        public virtual int? AnswerId { get; set; }
        public virtual DateTime? ArchivedUtc { get; set; }
        public virtual IList<Attachment> Attachments { get; set; }
        public virtual IList<Reply> Replies { get; set; }
    }
}