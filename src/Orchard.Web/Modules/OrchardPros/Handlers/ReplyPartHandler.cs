using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.DisplayManagement;
using Orchard.Email.Services;
using Orchard.JobsQueue.Services;
using Orchard.Localization;
using OrchardPros.Models;
using OrchardPros.Services.User;

namespace OrchardPros.Handlers {
    public class ReplyPartHandler : ContentHandler {
        private readonly IUserManager _userManager;
        private readonly IShapeFactory _shapeFactory;
        private readonly IShapeDisplay _shapeDisplay;
        private readonly IJobsQueueService _jobsQueueService;

        public ReplyPartHandler(IUserManager userManager, IShapeFactory shapeFactory, IShapeDisplay shapeDisplay, IJobsQueueService jobsQueueService) {
            _userManager = userManager;
            _shapeFactory = shapeFactory;
            _shapeDisplay = shapeDisplay;
            _jobsQueueService = jobsQueueService;
            T = NullLocalizer.Instance;

            OnCreated<ReplyPart>(UpdateStats);
            OnCreated<ReplyPart>(SendNotifications);
        }

        public Localizer T { get; set; }

        private void UpdateStats(CreateContentContext context, ReplyPart part) {
            var user = part.User;
            var ticketOwner = part.ContainingContent.As<TicketPart>().User;
            var xpToAdd = _userManager.CalculateXpWhenReplied(ticketOwner);
            _userManager.AddXp(user, xpToAdd);
            _userManager.AddActivityPoints(user, 5);
        }

        private void SendNotifications(CreateContentContext context, ReplyPart part) {
            var ticket = part.ContainingContent.As<TicketPart>();
            var subscriptionSource = ticket.As<SubscriptionSourcePart>();
            var ticketOwner = ticket.User;
            var subscribers = subscriptionSource.Subscribers.ToList();

            subscribers.Add(ticketOwner);
            subscribers.RemoveAll(x => x.UserName == part.User.UserName);

            foreach (var subscriber in subscribers) {
                var template = _shapeFactory.Create("Template_Notification_NewReply", Arguments.From(new {
                    Ticket = ticket,
                    Reply = part,
                    Recipient = subscriber
                }));

                template.Metadata.Wrappers.Add("Template_Notification_Wrapper");

                var messageParameters = new Dictionary<string, object> {
                    {"Subject", T("New Ticket Reply").Text},
                    {"Body", _shapeDisplay.Display(template)},
                    {"Recipients", subscriber.Email }
                };
                var jobParameters = new {
                    type = SmtpMessageChannel.MessageType, 
                    parameters = messageParameters
                };

                _jobsQueueService.Enqueue("IMessageService.Send", jobParameters, 0);
            }
        }
    }
}