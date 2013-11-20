using System.ComponentModel.DataAnnotations;

namespace OrchardPros.Membership.ViewModels {
    public class SignupFormViewModel {
        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The two passwords don't match. Please try again.")]
        public string PasswordRepeat { get; set; }
    }
}