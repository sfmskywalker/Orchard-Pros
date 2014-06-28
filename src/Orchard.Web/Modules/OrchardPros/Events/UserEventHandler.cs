using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Services;
using Orchard.Users.Events;
using Orchard.Workflows.Services;
using OrchardPros.Activities;
using OrchardPros.Models;

namespace OrchardPros.Events {
    public class UserEventHandler : IUserEventHandler {
        private readonly IWorkflowManager _workFlowManager;
        private readonly IClock _clock;

        public UserEventHandler(IWorkflowManager workFlowManager, IClock clock) {
            _workFlowManager = workFlowManager;
            _clock = clock;
        }

        public void Created(UserContext context) {
            context.User.As<UserProfilePart>().CreatedUtc = _clock.UtcNow;
            _workFlowManager.TriggerEvent(UserSignedUpActivity.ActivityName, context.User, () => new Dictionary<string, object> { {"User", context.User} });
        }

        public void LoggedIn(IUser user) {
            _workFlowManager.TriggerEvent(UserSignedInActivity.ActivityName, user, () => new Dictionary<string, object>());
        }

        public void LoggedOut(IUser user) {
            _workFlowManager.TriggerEvent(UserSignedOutActivity.ActivityName, null, () => new Dictionary<string, object>());
        }
        
        public void Creating(UserContext context) { }
        public void AccessDenied(IUser user) {}
        public void ChangedPassword(IUser user) {}
        public void SentChallengeEmail(IUser user) {}
        public void ConfirmedEmail(IUser user) {}
        public void Approved(IUser user) {}
        public void LoggingIn(string userNameOrEmail, string password) { }
    }
}