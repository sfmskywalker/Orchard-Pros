using System.Collections.Generic;
using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services.Notifications {
    public class TicketNotificationSettingsProvider : Component, INotificationSettingsProvider {
        public IEnumerable<NotificationSettingDescriptor> GetNotificationSettings() {
            yield return new NotificationSettingDescriptor {
                Name = "NewTicketCreatedByPersonIFollow",
                Description = T("Email me when someone I follow creates a new ticket")
            };
        }
    }
}