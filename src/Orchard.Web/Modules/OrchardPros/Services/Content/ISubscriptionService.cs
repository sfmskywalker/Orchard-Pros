using System.Collections.Generic;
using Orchard;
using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.Services.Content {
    public interface ISubscriptionService : IDependency {
        IEnumerable<IUser> GetSubscribers(SubscriptionSourcePart subscriptionSource);
        Subscription Subscribe(SubscriptionSourcePart subscriptionSource, IUser user);
        void Unsubscribe(SubscriptionSourcePart subscriptionSource, IUser user);
        IPagedList<SubscriptionSourcePart> GetSubscriptionSourcesByUser(int userId, int? skip = null, int? take = null);
        bool HasSubscription(SubscriptionSourcePart subscriptionSource, IUser user);
    }
}