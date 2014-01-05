using System;

namespace OrchardPros.Models {
    public class Vote {
        public virtual int Id { get; set; }
        public virtual int ContentItemId { get; set; }
        public virtual int UserId { get; set; }
        public virtual int Points { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
    }
}