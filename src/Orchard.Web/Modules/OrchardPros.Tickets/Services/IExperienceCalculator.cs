using Orchard;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Services {
    public interface IExperienceCalculator : IDependency {
        int CalculateForTicket(ExpertPart user);
    }
}