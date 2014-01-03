using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OrchardPros.Models;

namespace OrchardPros.ViewModels {
    public class AccountSettingsViewModel {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        
        [Compare("Password", ErrorMessage = "The two passwords don't match.")]
        public string PasswordRepeat { get; set; }

        public AvatarType AvatarType { get; set; }
        public bool? DeleteAvatar { get; set; }
        public bool? DeleteWallpaper { get; set; }
        public IList<NotificationSettingViewModel> Notifications { get; set; }
    }
}