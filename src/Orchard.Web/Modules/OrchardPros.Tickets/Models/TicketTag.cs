namespace OrchardPros.Tickets.Models {
    public class TicketTag {
        public virtual int Id { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual int TagId { get; set; }
    }
}