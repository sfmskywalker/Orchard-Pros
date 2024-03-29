using Orchard.Events;
using OrchardPros.Models;

namespace OrchardPros.Services.Content {
    public interface ITicketEventHandler : IEventHandler {
        void Solved(TicketSolvedContext context);
    }
}