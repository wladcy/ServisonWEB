using Rental.Models;
using Rental.Validators;
using System.ComponentModel.DataAnnotations;

namespace Default.Models.AccountViewModels
{
    public class ResetPasswordViewModel : IPasswordModel
    {
        [Required(ErrorMessage = "Mail jest wymagany.")]
        [EmailAddress(ErrorMessage = "Podano niepoprawny mail.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [StringLength(100, ErrorMessage = "Hasło musi zawierać przynajmniej 6 znaków.", MinimumLength = 6)]
        [PasswordValidator]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwórz hasło")]
        [Compare("Password", ErrorMessage = "Podane hasła nie pasują.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
        public string NewPassword { get; set; }
    }
}
