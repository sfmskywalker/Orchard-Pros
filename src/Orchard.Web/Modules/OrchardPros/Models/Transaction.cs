using System;

namespace OrchardPros.Models {
    public class Transaction {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? PaidUtc { get; set; }
        public DateTime? CanceledUtc { get; set; }
        public DateTime? PaymentDeclinedUtc { get; set; }
    }
}