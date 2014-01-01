using System.Collections.Generic;
using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface INotificationSettingsProvider : IDependency {
        IEnumerable<NotificationSettingDescriptor> GetNotificationSettings();
    }
}