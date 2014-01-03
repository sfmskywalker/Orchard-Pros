using System.ComponentModel.DataAnnotations;
using Orchard.Security;

namespace OrchardPros.ViewModels {
    public class SkillViewModel {
        public int? Id { get; set; }
        [StringLength(64)] public string Name { get; set; }
        [UIHint("Rating")] public int Rating { get; set; }
        
        public IUser User { get; set; }
    }
}