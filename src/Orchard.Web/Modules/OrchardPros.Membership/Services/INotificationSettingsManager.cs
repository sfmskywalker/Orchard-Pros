using System.Collections.Generic;
using Orchard;
using OrchardPros.Membership.Models;

namespace OrchardPros.Membership.Services {
    public interface INotificationSettingsManager : IDependency {
        IEnumerable<NotificationSettingDescriptor> GetNotificationSettings();
    }
}