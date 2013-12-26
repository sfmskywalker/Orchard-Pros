namespace OrchardPros.Tickets.Models {
    public class TicketCategory {
        public virtual int Id { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual int CategoryId { get; set; }
    }
}