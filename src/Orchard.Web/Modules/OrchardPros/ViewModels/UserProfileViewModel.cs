using OrchardPros.Models;

namespace OrchardPros.ViewModels {
    public class UserProfileViewModel {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public AvatarType AvatarType { get; set; }
        public string Bio { get; set; }
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }
        public string TwitterAlias { get; set; }
        public string FacebookUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string CompanyWebsiteUrl { get; set; }
        public string BlogUrl { get; set; }
        public dynamic Tabs { get; set; }
    }
}