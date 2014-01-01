using OrchardPros.Models;

namespace OrchardPros.ViewModels {
    public class UserProfileViewModel {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public AvatarType AvatarType { get; set; }
        public string Bio { get; set; }
        public dynamic Tabs { get; set; }
    }
}