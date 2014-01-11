using Orchard.ContentManagement;
using OrchardPros.Models;
using OrchardPros.Services;

namespace OrchardPros.Events {
    public class AssignExperienceTicketEventHandler : ITicketEventHandler {
        public void Solved(TicketSolvedContext context) {
            context.Expert.As<UserProfilePart>().ExperiencePoints += context.Ticket.ExperiencePoints;
        }
    }
}