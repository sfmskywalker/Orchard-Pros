using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Security;
using Orchard.Services;
using OrchardPros.Helpers;
using OrchardPros.Models;

namespace OrchardPros.Services.Content {
    public class SubscriptionService : ISubscriptionService {
        private readonly IContentManager _contentManager;
        private readonly IClock _clock;
        private readonly IRepository<Subscription> _subscriptionRepository;

        public SubscriptionService(IContentManager contentManager, IClock clock, IRepository<Subscription> subscriptionRepository) {
            _contentManager = contentManager;
            _clock = clock;
            _subscriptionRepository = subscriptionRepository;
        }

        public IEnumerable<IUser> GetSubscribers(SubscriptionSourcePart subscriptionSource) {
            var userIds = subscriptionSource.Subscriptions.Select(x => x.UserId).ToArray();
            return _contentManager.GetMany<IUser>(userIds, VersionOptions.Published, QueryHints.Empty);
        }

        public Subscription Subscribe(SubscriptionSourcePart subscriptionSource, IUser user) {
            var subscription = subscriptionSource.Record.Subscriptions.FirstOrDefault(x => x.UserId == user.Id);

            if(subscription != null)
                throw new InvalidOperationException("The specified user is already subscribed to the specified source.");

            subscription = new Subscription {
                CreatedUtc = _clock.UtcNow,
                UserId = user.Id,
                SubscriptionSourceId = subscriptionSource.Id
            };

            _subscriptionRepository.Create(subscription);
            return subscription;
        }

        public void Unsubscribe(SubscriptionSourcePart subscriptionSource, IUser user) {
            var subscription = subscriptionSource.Record.Subscriptions.FirstOrDefault(x => x.UserId == user.Id);

            if (subscription == null)
                throw new InvalidOperationException("The specified user is already unsubscribed from the specified source.");

            _subscriptionRepository.Delete(subscription);
        }

        public IPagedList<SubscriptionSourcePart> GetSubscriptionSourcesByUser(int userId, int? skip = null, int? take = null) {
            var query = _subscriptionRepository.Table.Where(x => x.UserId == userId).Select(x => x.SubscriptionSourceId);
            var count = query.Count();

            if (skip != null && take != null)
                query = query.Skip(skip.Value).Take(take.Value);

            var ids = query.ToArray();
            return _contentManager.GetMany<SubscriptionSourcePart>(ids, VersionOptions.Published, QueryHints.Empty).ToPagedList(count);
        }
    }
}