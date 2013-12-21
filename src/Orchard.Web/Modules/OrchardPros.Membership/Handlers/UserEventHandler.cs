using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Services;
using Orchard.Users.Events;
using OrchardPros.Membership.Models;

namespace OrchardPros.Membership.Handlers {
    public class UserEventHandler : IUserEventHandler {
        private readonly IClock _clock;

        public UserEventHandler(IClock clock) {
            _clock = clock;
        }

        public void Creating(UserContext context) {}

        public void Created(UserContext context) {
            context.User.As<UserProfilePart>().CreatedUtc = _clock.UtcNow;
        }

        public void LoggedIn(IUser user) {
            user.As<UserProfilePart>().LastLoggedInUtc = _clock.UtcNow;
        }

        public void LoggedOut(IUser user) {}

        public void AccessDenied(IUser user) {}

        public void ChangedPassword(IUser user) {}

        public void SentChallengeEmail(IUser user) {}

        public void ConfirmedEmail(IUser user) {}

        public void Approved(IUser user) {}
    }
}