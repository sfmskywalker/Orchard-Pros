using System.Collections.Generic;
using NGM.OpenAuthentication.Activities;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Workflows.Models;

namespace OrchardPros.Activities {
    public class SignIn : TaskBase {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipService _membershipService;

        public SignIn(IAuthenticationService authenticationService, IMembershipService membershipService) {

            _authenticationService = authenticationService;
            _membershipService = membershipService;
        }

        public const string ActivityName = "SignIn";
        
        public override string Name {
            get { return ActivityName; }
        }

        public override LocalizedString Category {
            get { return T("Authentication"); }
        }

        public override LocalizedString Description {
            get { return T("Signs in the user using the username value currently stored in the workflow."); }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext) {
            yield return T("Done");
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext) {
            var userName = workflowContext.GetState<string>("UserName");
            _authenticationService.SignIn(_membershipService.GetUser(userName), true);
            yield return T("Done");
        }
    }
}