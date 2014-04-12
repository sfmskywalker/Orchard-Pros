using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using OrchardPros.Models;
using OrchardPros.Services.Content;
using OrchardPros.Services.User;

namespace OrchardPros.Handlers {
    public class ReplyPartHandler : ContentHandler {
        private readonly IUserManager _userManager;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IEmailService _emailService;

        public ReplyPartHandler(
            IUserManager userManager, 
            ISubscriptionService subscriptionService, 
            IEmailService emailService) {

            _userManager = userManager;
            _subscriptionService = subscriptionService;
            _emailService = emailService;
            T = NullLocalizer.Instance;

            OnCreated<ReplyPart>(UpdateStats);
            OnCreated<ReplyPart>(Subscribe);
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

        private void Subscribe(CreateContentContext context, ReplyPart part)
        {
            var user = part.User;
            var ticket = part.ContainingContent.As<TicketPart>();
            var subscriptionSource = ticket.As<SubscriptionSourcePart>();

            if(!_subscriptionService.HasSubscription(subscriptionSource, user))
                _subscriptionService.Subscribe(subscriptionSource, user);
        }

        private void SendNotifications(CreateContentContext context, ReplyPart part) {
            var ticket = part.ContainingContent.As<TicketPart>();
            var subscriptionSource = ticket.As<SubscriptionSourcePart>();
            var ticketOwner = ticket.User;
            var subscribers = subscriptionSource.Subscribers.ToList();

            subscribers.Add(ticketOwner);
            subscribers.RemoveAll(x => x.UserName == part.User.UserName);

            foreach (var subscriber in subscribers) {
                _emailService.Queue(T("New Ticket Reply"), subscriber.Email, "Template_Notification_NewReply", new {
                    Ticket = ticket,
                    Reply = part,
                    Recipient = subscriber
                });
            }
        }
    }
}