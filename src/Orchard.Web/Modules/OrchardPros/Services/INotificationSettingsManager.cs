using System.Collections.Generic;
using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface INotificationSettingsManager : IDependency {
        IEnumerable<NotificationSettingDescriptor> GetNotificationSettings();
    }
}