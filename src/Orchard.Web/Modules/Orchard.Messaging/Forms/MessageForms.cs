using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Messaging.Models;
using Orchard.Messaging.Services;

namespace Orchard.Messaging.Forms {
    public class MessageForms : Component, IFormProvider {
        private readonly IMessageQueueManager _messageQueueManager;
        protected dynamic New { get; set; }

        public MessageForms(IShapeFactory shapeFactory, IMessageQueueManager messageQueueManager) {
            New = shapeFactory;
            _messageQueueManager = messageQueueManager;
        }

        public void Describe(DescribeContext context) {
            Func<IShapeFactory, dynamic> form =
                shape => New.Form(
                Id: "MessageActivity",
                _Type: New.FieldSet(
                    Title: T("Send to"),
                    _RecipientAddress: New.Textbox(
                        Id: "recipient-address",
                        Name: "RecipientAddress",
                        Title: T("Address"),
                        Description: T("Specify a comma-separated list of recipient addresses, such as e-mail addresses, twitter aliases, or anything else that is applicable to the selected channel."),
                        Classes: new[] { "large", "text", "tokenized" }),
                    _RecipientName: New.Textbox(
                        Id: "recipient-name",
                        Name: "RecipientName",
                        Title: T("Name"),
                        Description: T("Specify a comma-separated list of recipient names."),
                        Classes: new[] { "large", "text", "tokenized" })),
                _Subject: New.Textbox(
                    Id: "Subject", Name: "Subject",
                    Title: T("Subject"),
                    Description: T("The subject of the message."),
                    Classes: new[] { "large", "text", "tokenized" }),
                _Message: New.Textarea(
                    Id: "Body", Name: "Body",
                    Title: T("Body"),
                    Description: T("The body of the message."),
                    Classes: new[] { "tokenized" }),
                _Priority: New.SelectList(
                    Id: "priority",
                    Name: "Priority",
                    Title: T("Priority"),
                    Description: ("The priority of this message."),
                    Items: GetPriorities().ToList()),
                _Channel: New.SelectList(
                    Id: "channel",
                    Name: "Channel",
                    Title: T("Channel"),
                    Description: ("The channel through which to send this message."),
                    Items: GetChannels().ToList()),
                _Queue: New.SelectList(
                    Id: "queue",
                    Name: "Queue",
                    Title: T("Queue"),
                    Description: ("The queue to add this message to."),
                    Items: GetQueues().ToList()));

            context.Form("MessageActivity", form);
        }

        private IEnumerable<SelectListItem> GetPriorities() {
            var priorities = _messageQueueManager.GetPriorities().ToList();
            if (!priorities.Any())
                priorities = _messageQueueManager.CreateDefaultPriorities().ToList();
            return priorities.Select(x => new SelectListItem { Text = x.DisplayText, Value = x.Id.ToString(CultureInfo.InvariantCulture) });
        }

        private IEnumerable<SelectListItem> GetQueues() {
            var queues = _messageQueueManager.GetQueues().ToList();
            if (!queues.Any())
                queues = new List<MessageQueue> {_messageQueueManager.CreateDefaultQueue()};
            return queues.Select(x => new SelectListItem {Text = x.Name, Value = x.Id.ToString(CultureInfo.InvariantCulture)});
        }

        private IEnumerable<SelectListItem> GetChannels() {
            var channels = _messageQueueManager.GetChannels();
            return channels.Select(x => new SelectListItem {Text = x.Name, Value = x.Name});
        }
    }

    public class MessageFormsValidator : IFormEventHandler {
        public Localizer T { get; set; }
        public void Building(BuildingContext context) {}
        public void Built(BuildingContext context) {}
        public void Validated(ValidatingContext context) { }

        public void Validating(ValidatingContext context) {
            if (context.FormName != "MessageActivity") return;

            var recipientAddress = context.ValueProvider.GetValue("RecipientAddress").AttemptedValue;
            var subject = context.ValueProvider.GetValue("Subject").AttemptedValue;
            var body = context.ValueProvider.GetValue("Body").AttemptedValue;
            var channel = context.ValueProvider.GetValue("Channel").AttemptedValue;
            var queue = context.ValueProvider.GetValue("Queue").AttemptedValue;

            if (String.IsNullOrWhiteSpace(recipientAddress)) {
                context.ModelState.AddModelError("RecipientAddress", T("You must specify at least one recipient.").Text);
            }

            if (String.IsNullOrWhiteSpace(subject)) {
                context.ModelState.AddModelError("Subject", T("You must provide a Subject.").Text);
            }

            if (String.IsNullOrWhiteSpace(body)) {
                context.ModelState.AddModelError("Body", T("You must provide a Body.").Text);
            }

            if (String.IsNullOrWhiteSpace(channel)) {
                context.ModelState.AddModelError("Channel", T("You must provide a Channel.").Text);
            }

            if (String.IsNullOrWhiteSpace(queue)) {
                context.ModelState.AddModelError("Queue", T("You must provide a Queue.").Text);
            }
        }

    }
}