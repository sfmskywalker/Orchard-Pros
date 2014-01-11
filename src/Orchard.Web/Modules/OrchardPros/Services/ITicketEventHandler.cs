using Orchard.Events;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface ITicketEventHandler : IEventHandler {
        void Solved(TicketSolvedContext context);
    }
}