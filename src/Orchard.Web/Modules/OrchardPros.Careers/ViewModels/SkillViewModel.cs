using System.ComponentModel.DataAnnotations;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.ViewModels {
    public class SkillViewModel {
        [StringLength(64)] public string Name { get; set; }
        [UIHint("Rating")] public int Rating { get; set; }
        
        public ProfessionalProfilePart Profile { get; set; }
    }
}