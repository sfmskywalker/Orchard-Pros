using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Messaging.Models;
using Orchard.Messaging.Services;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace Orchard.Messaging.Activities {
    [OrchardFeature("Orchard.Messaging.Queuing")]
    public class MessageActivity : Task {
        private readonly IMessageQueueManager _messageQueueManager;

        public MessageActivity(IMessageQueueManager messageQueueManager) {
            _messageQueueManager = messageQueueManager;
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
            var recipientAddresses = Split(activityContext.GetState<string>("RecipientAddress")).ToList();
            var recipientNames = Split(activityContext.GetState<string>("RecipientName")).ToList();
            var body = activityContext.GetState<string>("Body");
            var subject = activityContext.GetState<string>("Subject");
            var channelName = activityContext.GetState<string>("Channel");
            var queueId = activityContext.GetState<int>("Queue");
            var priorityId = activityContext.GetState<int>("Priority");
            var recipients = BuildRecipientsList(recipientAddresses, recipientNames);
            var priority = _messageQueueManager.GetPriority(priorityId);

            _messageQueueManager.Send(recipients, channelName, subject, body, priority: priority, queueId: queueId);

            yield return T("Queued");
        }

        private static IEnumerable<MessageRecipient> BuildRecipientsList(IEnumerable<string> addresses, IList<string> names) {
            var i = 0;

            foreach (var address in addresses) {
                var name = names.Count - 1 >= i ? names[i] : default(string);
                yield return new MessageRecipient(address, name);
                i++;
            }
        }

        private static IEnumerable<string> Split(string value) {
            return !String.IsNullOrWhiteSpace(value) ? value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries) : Enumerable.Empty<string>();
        }
    }
}