using OrchardPros.Models;

namespace OrchardPros.Services {
    public class ExperienceCalculator : IExperienceCalculator {
        public int CalculateForTicket(UserProfilePart user) {
            return 5 + 15*user.Level;
        }
    }
}