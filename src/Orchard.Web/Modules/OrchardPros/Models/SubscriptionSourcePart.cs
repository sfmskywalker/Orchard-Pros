using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Core.Common.Utilities;
using Orchard.Security;

namespace OrchardPros.Models {
    public class SubscriptionSourcePart : ContentPart<SubscriptionSourcePartRecord> {
        internal LazyField<IEnumerable<IUser>> SubscribersField = new LazyField<IEnumerable<IUser>>();

        public IList<Subscription> Subscriptions {
            get { return Record.Subscriptions; }
            set { Record.Subscriptions = value; }
        }

        public IEnumerable<IUser> Subscribers {
            get { return SubscribersField.Value; }
        }
    }
}