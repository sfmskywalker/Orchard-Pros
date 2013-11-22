using System.Collections.Generic;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Workflows.Services;

namespace OrchardPros.Membership.Events {
    public class UserEventHandler : IUserEventHandler {
        private readonly IWorkflowManager _workFlowManager;

        public UserEventHandler(IWorkflowManager workFlowManager) {
            _workFlowManager = workFlowManager;
        }

        public void Created(UserContext context) {
            _workFlowManager.TriggerEvent("UserSignedUp", context.User, () => new Dictionary<string, object>());
        }

        public void LoggedIn(IUser user) {
            _workFlowManager.TriggerEvent("UserSignedIn", user, () => new Dictionary<string, object>());
        }

        public void LoggedOut(IUser user) {
            _workFlowManager.TriggerEvent("UserSignedOut", null, () => new Dictionary<string, object>());
        }
        
        public void Creating(UserContext context) { }
        public void AccessDenied(IUser user) {}
        public void ChangedPassword(IUser user) {}
        public void SentChallengeEmail(IUser user) {}
        public void ConfirmedEmail(IUser user) {}
        public void Approved(IUser user) {}
    }
}