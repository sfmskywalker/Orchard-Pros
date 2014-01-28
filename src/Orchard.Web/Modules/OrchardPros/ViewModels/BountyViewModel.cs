using System.ComponentModel.DataAnnotations;
using OrchardPros.Models;

namespace OrchardPros.ViewModels {
    public class BountyViewModel {
        public TicketPart Ticket { get; set; }
        [Required]
        public decimal? Amount { get; set; }
    }
}