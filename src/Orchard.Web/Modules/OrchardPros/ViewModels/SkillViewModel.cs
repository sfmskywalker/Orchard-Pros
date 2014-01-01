using System.ComponentModel.DataAnnotations;
using OrchardPros.Models;

namespace OrchardPros.ViewModels {
    public class SkillViewModel {
        [StringLength(64)] public string Name { get; set; }
        [UIHint("Rating")] public int Rating { get; set; }
        
        public UserProfilePart Profile { get; set; }
    }
}