using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Workflows.Models;

namespace OrchardPros.Membership.Activities {
    public class UserSignedUpActivity : EventBase {

        public const string ActivityName = "UserSignedUp";

        public override string Name {
            get { return ActivityName; }
        }

        public override LocalizedString Description {
            get { return T("A new user account has been created."); }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext) {
            return new[] { T("Created") };
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext) {
            workflowContext.SetState("UserName", workflowContext.Content.As<IUser>().UserName);
            yield return T("Created");
        }

        public override LocalizedString Category {
            get { return T("Account"); }
        }
    }
}