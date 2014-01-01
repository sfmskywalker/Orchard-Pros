using System.Collections.Generic;
using Orchard.Localization;
using Orchard.Workflows.Models;

namespace OrchardPros.Activities {
    public class UserSignedOutActivity : EventBase {
        public const string ActivityName = "UserSignedOut";

        public override string Name {
            get { return ActivityName; }
        }

        public override LocalizedString Description {
            get { return T("A user has signed out."); }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext) {
            return new[] { T("Signed Out") };
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext) {
            yield return T("Signed Out");
        }

        public override LocalizedString Category {
            get { return T("Account"); }
        }
    }
}