using System.ComponentModel.DataAnnotations;
using Orchard.Security;

namespace OrchardPros.ViewModels {
    public class RecommendationViewModel {
        public RecommendationViewModel() {
            AllowPublication = true;
        }

        [Required] 
        public string Text{ get; set; }
        public bool AllowPublication { get; set; }
        public IUser User { get; set; }
    }
}