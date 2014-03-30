using System.ComponentModel.DataAnnotations;
using Orchard.Security;

namespace OrchardPros.ViewModels {
    public class RecommendationViewModel {
        [Required]
        [UIHint("Markdown")]
        public string Text{ get; set; }
        public IUser User { get; set; }
    }
}