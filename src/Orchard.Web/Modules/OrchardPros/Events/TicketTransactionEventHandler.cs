using System;
using OrchardPros.Models;
using OrchardPros.Services;

namespace OrchardPros.Events {
    public class TicketTransactionEventHandler : TransactionEventHandlerBase {
        private readonly ITicketService _ticketService;

        public TicketTransactionEventHandler(ITicketService ticketService) {
            _ticketService = ticketService;
        }

        public override void Charged(TransactionChargedContext context) {
            if (context.Transaction.ProductName != "Bounty")
                return;

            var ticketId = Int32.Parse(context.Transaction.Context);
            var ticket = _ticketService.GetTicket(ticketId);

            ticket.Bounty = context.Transaction.Amount;
        }
    }
}