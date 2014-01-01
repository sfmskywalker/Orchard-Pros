using System.Collections.Generic;
using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public class NewsletterNotificationSettingsProvider : Component, INotificationSettingsProvider {
        public IEnumerable<NotificationSettingDescriptor> GetNotificationSettings() {
            yield return new NotificationSettingDescriptor {
                Name = "SubscribeToNewsletter",
                Description = T("Subscribe to the Orchard Pros newsletter")
            };
        }
    }
}