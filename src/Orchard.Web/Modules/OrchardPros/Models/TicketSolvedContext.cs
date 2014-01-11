using Orchard.Security;

namespace OrchardPros.Models {
    public class TicketSolvedContext : TicketContext {        
        public IUser Expert { get; set; }
    }
}