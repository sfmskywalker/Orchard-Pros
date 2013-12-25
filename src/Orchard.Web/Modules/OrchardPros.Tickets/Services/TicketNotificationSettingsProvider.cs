using System.Collections.Generic;
using Orchard;
using OrchardPros.Membership.Models;
using OrchardPros.Membership.Services;

namespace OrchardPros.Tickets.Services {
    public class TicketNotificationSettingsProvider : Component, INotificationSettingsProvider {
        public IEnumerable<NotificationSettingDescriptor> GetNotificationSettings() {
            yield return new NotificationSettingDescriptor {
                Name = "NewTicketCreatedByPersonIFollow",
                Description = T("Email me when someone I follow creates a new ticket")
            };
        }
    }
}