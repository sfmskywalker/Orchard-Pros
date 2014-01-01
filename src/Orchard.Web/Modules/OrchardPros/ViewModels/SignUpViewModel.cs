using System.ComponentModel.DataAnnotations;

namespace OrchardPros.ViewModels {
    public class SignUpViewModel {
        [Required]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The two passwords don't match. Please try again.")]
        public string PasswordRepeat { get; set; }

        public bool TermsAccepted { get; set; }
    }
}