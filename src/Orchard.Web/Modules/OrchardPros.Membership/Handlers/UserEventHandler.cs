using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Services;
using Orchard.Users.Events;
using Orchard.Workflows.Services;
using OrchardPros.Membership.Activities;
using OrchardPros.Membership.Models;

namespace OrchardPros.Membership.Handlers {
    public class UserEventHandler : IUserEventHandler {
        private readonly IClock _clock;
        private readonly IWorkflowManager _workflowManager;

        public UserEventHandler(IClock clock, IWorkflowManager workflowManager) {
            _clock = clock;
            _workflowManager = workflowManager;
        }

        public void Creating(UserContext context) {}

        public void Created(UserContext context) {
            context.User.As<UserProfilePart>().CreatedUtc = _clock.UtcNow;
            _workflowManager.TriggerEvent(UserSignedUpActivity.ActivityName, context.User, () => new Dictionary<string, object>());
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