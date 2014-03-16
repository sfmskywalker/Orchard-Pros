using System.ComponentModel.DataAnnotations;
using Orchard.Security;

namespace OrchardPros.ViewModels {
    public class BioViewModel {
        [UIHint("Markdown")]
        public string Bio { get; set; }
        public IUser User { get; set; }
    }
}