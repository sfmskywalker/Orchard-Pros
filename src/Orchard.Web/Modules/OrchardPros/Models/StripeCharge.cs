using System;

namespace OrchardPros.Models {
    public class StripeCharge {
        public string Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public bool LiveMode { get; set; }
        public bool Paid { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public bool Refunded { get; set; }
        public string BalanceTransaction { get; set; }
        public string FailureMessage { get; set; }
        public string FailureCode { get; set; }
    }
}