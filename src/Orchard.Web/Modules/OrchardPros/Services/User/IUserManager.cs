using Orchard;
using Orchard.Security;

namespace OrchardPros.Services.User {
    public interface IUserManager : IDependency {
        int CalculateXpWhenSolved(IUser user);
        int CalculateXpWhenReplied(IUser user);
        void AddXp(IUser user, int xp);
        void AddActivityPoints(IUser user, int points = 1);
    }
}