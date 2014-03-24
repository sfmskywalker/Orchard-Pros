using System.Collections.Generic;
using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services.Notifications {
    public interface INotificationSettingsManager : IDependency {
        IEnumerable<NotificationSettingDescriptor> GetNotificationSettings();
    }
}