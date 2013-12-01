using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.ViewModels {
    public class ExperienceViewModel {
        [UIHint("PositionPicker")]
        public int? PositionId { get; set; }

        [Required]
        public string Description { get; set; }
        public DateTime CreatedUtc { get; set; }

        public ProfessionalProfilePart Profile { get; set; }
        public IList<Position> AvailablePositions { get; set; }
    }
}