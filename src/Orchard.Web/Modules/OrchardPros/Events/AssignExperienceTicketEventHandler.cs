using OrchardPros.Models;
using OrchardPros.Services.Content;
using OrchardPros.Services.User;

namespace OrchardPros.Events {
    public class TicketEventHandler : ITicketEventHandler {
        private readonly IUserManager _userManager;

        public TicketEventHandler(IUserManager userManager) {
            _userManager = userManager;
        }

        public void Solved(TicketSolvedContext context) {
            _userManager.AddXp(context.Expert, context.Ticket.ExperiencePoints);
        }
    }
}