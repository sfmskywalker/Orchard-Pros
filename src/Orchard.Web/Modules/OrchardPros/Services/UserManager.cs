using OrchardPros.Models;

namespace OrchardPros.Services {
    public class UserManager : IUserManager {
        private readonly IUserProfileEventHandler _userProfileEventHandlers;
        private readonly IXpCalculator _xpCalculator;

        public UserManager(IUserProfileEventHandler userProfileEventHandlers, IXpCalculator xpCalculator) {
            _userProfileEventHandlers = userProfileEventHandlers;
            _xpCalculator = xpCalculator;
        }

        public int CalculateXpWhenSolved(UserProfilePart user) {
            if (user.Level == 0)
                return 14;
            
            var xp = _xpCalculator.CalculateXp(user.Level);
            return xp/2;
        }

        public int CalculateXpWhenReplied(UserProfilePart user) {
            return user.Level + 1;
        }

        public void AddXp(UserProfilePart user, int xp) {
            user.ExperiencePoints += xp;
            _userProfileEventHandlers.XpReceived(new XpReceivedContext { User = user, Xp = xp});

            if (_xpCalculator.CanLevelUp(user.Level, user.ExperiencePoints)) {
                user.Level++;
                _userProfileEventHandlers.LevelUp(new LevelUpContext{ User = user, NewLevel = user.Level});
            }
        }
    }
}