using System.Collections.Generic;
using System.Linq;
using Orchard.Data;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Services {
    public class TicketService : ITicketService {
        private readonly IRepository<Ticket> _ticketRepository;

        public TicketService(IRepository<Ticket> ticketRepository) {
            _ticketRepository = ticketRepository;
        }

        public IEnumerable<Ticket> GetTicketsFor(int userId) {
            return _ticketRepository.Table.Where(x => x.UserId == userId);
        }
    }
}