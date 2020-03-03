using Rental.Models;
using Rental.Validators;
using System.ComponentModel.DataAnnotations;

namespace Default.Models.AccountViewModels
{
    public class RegisterViewModel : IPasswordModel
    {
        [Required(ErrorMessage = "Mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Podany adres mail nie jest poprawny")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [StringLength(100, ErrorMessage = "Hasło musi mieć przynajmniej 6 znaków.", MinimumLength = 6)]
        [PasswordValidator]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Powtórz hasło")]
        [Compare("Password", ErrorMessage = "Podane hasła nie pasują.")]
        public string ConfirmPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
