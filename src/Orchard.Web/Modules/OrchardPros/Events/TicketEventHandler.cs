using Orchard;
using OrchardPros.Models;
using OrchardPros.Services.Content;
using OrchardPros.Services.User;

namespace OrchardPros.Events {
    public class TicketEventHandler : Component, ITicketEventHandler {
        private readonly IUserManager _userManager;
        private readonly IEmailService _emailService;

        public TicketEventHandler(IUserManager userManager, IEmailService emailService) {
            _userManager = userManager;
            _emailService = emailService;
        }

        public void Solved(TicketSolvedContext context) {
            _userManager.AddXp(context.Expert, context.Ticket.ExperiencePoints);
            _emailService.Queue(T("You solved a ticket"), context.Expert.Email, "Template_Notification_TicketSolved", new {
                Ticket = context.Ticket,
                Expert = context.Expert
            });
        }
    }
}