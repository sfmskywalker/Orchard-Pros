using System.Collections.Generic;
using Orchard.ContentManagement.Records;

namespace OrchardPros.Models {
    public class SubscriptionSourcePartRecord : ContentPartRecord {
        public virtual IList<Subscription> Subscriptions { get; set; }
    }
}