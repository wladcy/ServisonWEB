using System.ComponentModel.DataAnnotations;

namespace Default.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Nie podano poprawnego adresu mailowego")]
        [Display(Name = "Login")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Display(Name = "Zapamiętaj?")]
        public bool RememberMe { get; set; }
    }
}
