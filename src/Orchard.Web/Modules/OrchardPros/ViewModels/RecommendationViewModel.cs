using System;
using System.ComponentModel.DataAnnotations;
using OrchardPros.Models;

namespace OrchardPros.ViewModels {
    public class RecommendationViewModel {
        [StringLength(64), Required]
        public string RecommendingUserName { get; set; }

        [Required] 
        public string Text{ get; set; }
        public bool Approved { get; set; }
        public DateTime CreatedUtc { get; set; }

        public UserProfilePart Profile { get; set; }
    }
}