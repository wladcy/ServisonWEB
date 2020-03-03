using Rental.Models;
using Rental.Validators;
using System.ComponentModel.DataAnnotations;

namespace Default.Models.ManageViewModels
{
    public class ChangePasswordViewModel : IPasswordModel
    {
        [Required(ErrorMessage = "Aktualne hasło jest wymagane.")]
        [DataType(DataType.Password)]
        [Display(Name = "Aktualne hasło")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Nowe hasło jest wymagane")]
        [StringLength(100, ErrorMessage = "Hasło musi zawierać przynajmniej 6 znaków.", MinimumLength = 6)]
        [PasswordValidator]
        [DataType(DataType.Password)]
        [Display(Name = "Nowe hasło")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Powtórz nowe hasło")]
        [Compare("NewPassword", ErrorMessage = "Podane hasła nie pasują.")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }
        public string Password { get; set; }
    }
}
