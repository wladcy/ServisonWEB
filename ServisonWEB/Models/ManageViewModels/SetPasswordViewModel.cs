using Rental.Models;
using Rental.Validators;
using System.ComponentModel.DataAnnotations;

namespace Default.Models.ManageViewModels
{
    public class SetPasswordViewModel : IPasswordModel
    {
        [Required(ErrorMessage = "Nowe hasło jest wymagane")]
        [StringLength(100, ErrorMessage = "Hasło musi zawierać przynajmniej 6 znaków.", MinimumLength = 6)]
        [PasswordValidator]
        [DataType(DataType.Password)]
        [Display(Name = "Nowe hasło")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Powtórz hasło")]
        [Compare("NewPassword", ErrorMessage = "Podane hasła nie pasują.")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }
        public string Password { get; set; }
    }
}
