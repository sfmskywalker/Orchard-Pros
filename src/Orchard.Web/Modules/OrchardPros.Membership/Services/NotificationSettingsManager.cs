using System.Collections.Generic;
using System.Linq;
using OrchardPros.Membership.Models;

namespace OrchardPros.Membership.Services {
    public class NotificationSettingsManager : INotificationSettingsManager {
        private readonly IEnumerable<INotificationSettingsProvider> _providers;

        public NotificationSettingsManager(IEnumerable<INotificationSettingsProvider> providers) {
            _providers = providers;
        }

        public IEnumerable<NotificationSettingDescriptor> GetNotificationSettings() {
            return _providers.SelectMany(x => x.GetNotificationSettings());
        }
    }
}