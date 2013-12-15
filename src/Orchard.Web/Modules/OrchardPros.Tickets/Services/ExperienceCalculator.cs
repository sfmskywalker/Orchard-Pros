using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Services {
    public class ExperienceCalculator : IExperienceCalculator {
        public int CalculateForTicket(ExpertPart user) {
            return 15*user.Level;
        }
    }
}