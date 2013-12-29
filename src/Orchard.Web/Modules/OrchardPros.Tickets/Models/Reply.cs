using System;
using Orchard.Data.Conventions;

namespace OrchardPros.Tickets.Models {
    public class Reply {
        public virtual int Id { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual Reply ParentReply { get; set; }
        public virtual int UserId { get; set; }
        
        [StringLengthMax]
        public virtual string Body { get; set; }

        public virtual DateTime CreatedUtc { get; set; }
        public virtual DateTime ModifiedUtc { get; set; }
        public virtual int Votes { get; set; }
    }
}