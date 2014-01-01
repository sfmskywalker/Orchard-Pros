using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OrchardPros.Models;

namespace OrchardPros.ViewModels {
    public class ExperienceViewModel {
        [UIHint("PositionPicker")]
        public int? PositionId { get; set; }

        [Required]
        public string Description { get; set; }
        public DateTime CreatedUtc { get; set; }

        public UserProfilePart Profile { get; set; }
        public IList<Position> AvailablePositions { get; set; }
    }
}