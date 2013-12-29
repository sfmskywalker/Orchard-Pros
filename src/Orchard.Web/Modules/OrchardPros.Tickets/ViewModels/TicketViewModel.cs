using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.Taxonomies.Models;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.ViewModels {
    public class TicketViewModel {
        public int? Id { get; set; }
        [Required, MaxLength(256)]
        public string Subject { get; set; }

        [UIHint("TicketTypePicker")]
        public TicketType Type { get; set; }

        [Required]
        public string Body { get; set; }

        [UIHint("TermsPicker")]
        public IList<int> Categories { get; set; }

        public string Tags { get; set; }
        public decimal? Bounty { get; set; }
        public int ExperiencePoints { get; set; }

        [Required]
        public DateTime? DeadlineUtc { get; set; }

        public ExpertPart User { get; set; }
        public IEnumerable<TermPart> CategoryTerms { get; set; }
        public IList<string> UploadedFileNames { get; set; }
        public IList<string> OriginalFileNames { get; set; }
    }
}