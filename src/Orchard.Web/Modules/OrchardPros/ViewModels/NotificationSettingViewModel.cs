using Orchard.Localization;

namespace OrchardPros.ViewModels {
    public class NotificationSettingViewModel {
        public string Name { get; set; }
        public bool Checked { get; set; }
        public LocalizedString Description { get; set; }
    }
}