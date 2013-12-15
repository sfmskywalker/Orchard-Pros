using System.Collections.Generic;
using Orchard;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Services {
    public interface ITicketService : IDependency {
        IEnumerable<Ticket> GetTicketsFor(int userId);
    }
}