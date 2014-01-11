using Orchard.ContentManagement;
using OrchardPros.Models;
using OrchardPros.Services;

namespace OrchardPros.Events {
    public class AssignExperienceTicketEventHandler : ITicketEventHandler {
        private readonly IUserManager _userManager;

        public AssignExperienceTicketEventHandler(IUserManager userManager) {
            _userManager = userManager;
        }

        public void Solved(TicketSolvedContext context) {
            _userManager.AddXp(context.Expert.As<UserProfilePart>(), context.Ticket.ExperiencePoints);
        }
    }
}