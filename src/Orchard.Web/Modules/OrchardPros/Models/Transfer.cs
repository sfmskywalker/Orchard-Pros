using System;

namespace OrchardPros.Models {
    public class Transfer {
        public virtual int Id { get; set; }
        public virtual int RecipientUserId { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual string Currency { get; set; }
        public virtual string Context { get; set; }
        public virtual TransferStatus Status { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
        public virtual DateTime? CompletedUtc { get; set; }
    }
}