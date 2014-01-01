using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface IExperienceCalculator : IDependency {
        int CalculateForTicket(UserProfilePart user);
    }
}