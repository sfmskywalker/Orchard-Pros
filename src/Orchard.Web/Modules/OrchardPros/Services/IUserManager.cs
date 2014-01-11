using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface IUserManager : IDependency {
        int CalculateXpWhenSolved(UserProfilePart user);
        int CalculateXpWhenReplied(UserProfilePart user);
        void AddXp(UserProfilePart user, int xp);
    }
}