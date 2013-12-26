using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.Taxonomies.Models;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.ViewModels {
    public class TicketViewModel {
        [Required, MaxLength(256)]
        public string Title { get; set; }

        [UIHint("TicketTypePicker")]
        public TicketType Type { get; set; }

        [Required]
        public string Description { get; set; }

        [Required, UIHint("TermsPicker")]
        public IList<int> Categories { get; set; }

        public string Tags { get; set; }
        public decimal? Bounty { get; set; }
        public int ExperiencePoints { get; set; }
        
        [Required]
        public DateTime? CreatedUtc { get; set; }

        [Required]
        public DateTime? DeadlineUtc { get; set; }

        public ExpertPart User { get; set; }
        public IEnumerable<TermPart> CategoryTerms { get; set; }
    }
}