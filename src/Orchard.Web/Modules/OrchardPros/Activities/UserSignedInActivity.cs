using System.Collections.Generic;
using Orchard.Localization;
using Orchard.Workflows.Models;

namespace OrchardPros.Activities {
    public class UserSignedInActivity : EventBase {

        public const string ActivityName = "UserSignedIn";

        public override string Name {
            get { return ActivityName; }
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