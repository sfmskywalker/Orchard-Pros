using System;
using System.ComponentModel.DataAnnotations;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.ViewModels {
    public class RecommendationViewModel {
        [StringLength(64), Required]
        public string RecommendingUserName { get; set; }

        [Required] 
        public string Text{ get; set; }
        public bool Approved { get; set; }
        public DateTime CreatedUtc { get; set; }

        public ProfessionalProfilePart Profile { get; set; }
    }
}