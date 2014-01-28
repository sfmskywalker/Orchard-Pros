using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.Taxonomies.Models;
using OrchardPros.Models;

namespace OrchardPros.ViewModels {
    public class TicketViewModel {
        public int? Id { get; set; }
        [Required, MaxLength(256)]
        public string Subject { get; set; }

        [UIHint("TicketTypePicker")]
        public TicketType Type { get; set; }

        [Required, UIHint("Markdown")]
        public string Body { get; set; }

        [UIHint("TermsPicker")]
        public IList<int> Categories { get; set; }

        public string Tags { get; set; }
        public int ExperiencePoints { get; set; }

        [Required]
        public DateTime? DeadlineUtc { get; set; }

        public UserProfilePart User { get; set; }
        public IEnumerable<TermPart> CategoryTerms { get; set; }
        public AttachmentsViewModel Attachments { get; set; }
    }
}