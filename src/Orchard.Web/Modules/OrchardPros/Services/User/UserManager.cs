using Orchard.ContentManagement;
using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.Services.User {
    public class UserManager : IUserManager {
        private readonly IUserProfileEventHandler _userProfileEventHandlers;
        private readonly IXpCalculator _xpCalculator;

        public UserManager(IUserProfileEventHandler userProfileEventHandlers, IXpCalculator xpCalculator) {
            _userProfileEventHandlers = userProfileEventHandlers;
            _xpCalculator = xpCalculator;
        }

        public int CalculateXpWhenSolved(IUser user) {
            var profile = user.As<UserProfilePart>();
            if (profile.Level == 0)
                return 14;

            var xp = _xpCalculator.CalculateXp(profile.Level);
            return xp/2;
        }

        public int CalculateXpWhenReplied(IUser user) {
            var profile = user.As<UserProfilePart>();
            return profile.Level + 1;
        }

        public void AddXp(IUser user, int xp) {
            var profile = user.As<UserProfilePart>();
            profile.ExperiencePoints += xp;
            _userProfileEventHandlers.XpReceived(new XpReceivedContext { User = profile, Xp = xp });

            if (_xpCalculator.CanLevelUp(profile.Level, profile.ExperiencePoints)) {
                profile.Level++;
                _userProfileEventHandlers.LevelUp(new LevelUpContext { User = profile, NewLevel = profile.Level });
            }
        }

        public void AddActivityPoints(IUser user, int points = 1) {
            var profile = user.As<UserProfilePart>();
            profile.ActivityPoints += 1;
        }
    }
}