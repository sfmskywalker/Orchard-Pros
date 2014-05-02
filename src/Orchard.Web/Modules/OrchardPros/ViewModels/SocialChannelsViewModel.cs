using Orchard.Security;

namespace OrchardPros.ViewModels {
    public class SocialChannelsViewModel {
        public string TwitterAlias { get; set; }
        public string FacebookUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string CompanyWebsiteUrl { get; set; }
        public string BlogUrl { get; set; }
        public IUser User { get; set; }
    }
}