using System.Collections.Generic;
using Orchard.Localization;
using Orchard.Workflows.Models;

namespace OrchardPros.Membership.Activities {
    public class UserSignedUpActivity : EventBase {

        public override string Name {
            get { return "UserSignedUp"; }
        }

        public override LocalizedString Description {
            get { return T("A new user account has been created."); }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext) {
            return new[] { T("Created") };
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext) {
            yield return T("Created");
        }

        public override LocalizedString Category {
            get { return T("Account"); }
        }
    }
}