using System.ComponentModel.DataAnnotations;

namespace OrchardPros.Membership.ViewModels {
    public class SignInViewModel {
        [Required]
        public string UserNameOrEmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
    }
}