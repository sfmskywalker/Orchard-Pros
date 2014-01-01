using OrchardPros.Models;

namespace OrchardPros.Services {
    public class ExperienceCalculator : IExperienceCalculator {
        public int CalculateForTicket(ExpertPart user) {
            return 5 + 15*user.Level;
        }
    }
}