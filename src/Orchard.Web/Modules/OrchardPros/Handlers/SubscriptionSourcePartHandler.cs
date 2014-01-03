using System.Linq;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using OrchardPros.Models;
using OrchardPros.Services;

namespace OrchardPros.Handlers {
    public class SubscriptionSourcePartHandler : ContentHandler {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionSourcePartHandler(IRepository<SubscriptionSourcePartRecord> repository, ISubscriptionService subscriptionService) {
            _subscriptionService = subscriptionService;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<SubscriptionSourcePart>(SetupLazyFields);
        }

        private void SetupLazyFields(ActivatedContentContext context, SubscriptionSourcePart part) {
            part.SubscribersField.Loader(() => _subscriptionService.GetSubscribers(part).ToArray());
        }
    }
}