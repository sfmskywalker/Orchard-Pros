using System;

namespace OrchardPros.Tickets.Models {
    public class Vote {
        public virtual int Id { get; set; }
        public virtual int ContentId { get; set; }
        public virtual int UserId { get; set; }
        public virtual int Points { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
    }
}