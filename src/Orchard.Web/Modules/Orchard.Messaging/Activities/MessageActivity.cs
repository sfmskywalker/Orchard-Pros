using System.Collections.Generic;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace Orchard.Messaging.Activities {
    [OrchardFeature("Orchard.Messaging.Queuing")]
    public class MessageActivity : Task {

        public MessageActivity(){
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext) {
            return new[] { T("Queued") };
        }

        public override string Form {
            get {
                return "MessageActivity";
            }
        }

        public override LocalizedString Category {
            get { return T("Messaging"); }
        }

        public override string Name {
            get { return "SendMessage"; }
        }

        public override LocalizedString Description {
            get { return T("Sends a message to a specific user."); }
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext) {
            var recipientAddress = activityContext.GetState<string>("RecipientAddress");
            var recipientName = activityContext.GetState<string>("RecipientName");
            var body = activityContext.GetState<string>("Body");
            var subject = activityContext.GetState<string>("Subject");
            var channelName = activityContext.GetState<string>("Channel");
            var queueId = activityContext.GetState<int>("Queue");

            yield return T("Queued");
        }

        private static IEnumerable<string> Split(string value) {
            return value.Split(new[] { ',', ';', ' ' });
        }
    }
}