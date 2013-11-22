using System.Collections.Generic;
using Orchard.Localization;
using Orchard.Workflows.Models;

namespace OrchardPros.Membership.Activities {
    public class UserSignedInActivity : EventBase {

        public override string Name {
            get { return "UserSignedIn"; }
        }

        public override LocalizedString Description {
            get { return T("A user has signed in."); }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext) {
            return new[] { T("Signed In") };
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext) {
            yield return T("Signed In");
        }

        public override LocalizedString Category {
            get { return T("Account"); }
        }
    }
}