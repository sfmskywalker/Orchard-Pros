using System.Collections.Generic;
using Orchard.Localization;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace OrchardPros.Membership.Activities {
    public class AccountCreatedActivity : Event {

        public Localizer T { get; set; }

        public override bool CanStartWorkflow {
            get { return true; }
        }

        public override string Name {
            get { return "AccountCreated"; }
        }

        public override LocalizedString Description {
            get { return T("New account has been created."); }
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