using System.Collections.Generic;
using Orchard;
using OrchardPros.Membership.Models;

namespace OrchardPros.Membership.Services {
    public class NewsletterNotificationSettingsProvider : Component, INotificationSettingsProvider {
        public IEnumerable<NotificationSettingDescriptor> GetNotificationSettings() {
            yield return new NotificationSettingDescriptor {
                Name = "SubscribeToNewsletter",
                Description = T("Subscribe to the Orchard Pros newsletter")
            };
        }
    }
}