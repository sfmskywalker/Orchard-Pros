using Orchard.Users.Models;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.ViewModels {
    public class RecommendationEx : Recommendation {
        public UserPartRecord RecommendingUser { get; set; }
    }
}