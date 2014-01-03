using System;

namespace OrchardPros.Models {
    public class Subscription {
        public virtual int Id { get; set; }
        public virtual int SubscriptionSourceId { get; set; }
        public virtual int UserId { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
    }
}