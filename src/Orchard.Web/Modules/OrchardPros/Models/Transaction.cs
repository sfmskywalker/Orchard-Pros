using System;

namespace OrchardPros.Models {
    public class Transaction {
        public virtual int Id { get; set; }
        public virtual string Handle { get; set; }
        public virtual int UserId { get; set; }
        public virtual string ProductName { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual TransactionStatus Status { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
        public virtual DateTime? ChargedUtc { get; set; }
        public virtual DateTime? CanceledUtc { get; set; }
        public virtual DateTime? DeclinedUtc { get; set; }
        public virtual string Reference { get; set; }
    }
}