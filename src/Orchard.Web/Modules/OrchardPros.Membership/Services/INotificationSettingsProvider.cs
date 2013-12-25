using System.Collections.Generic;
using Orchard;
using OrchardPros.Membership.Models;

namespace OrchardPros.Membership.Services {
    public interface INotificationSettingsProvider : IDependency {
        IEnumerable<NotificationSettingDescriptor> GetNotificationSettings();
    }
}