using System.ComponentModel.DataAnnotations;

namespace Default.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Nie podano poprawnego adresu mailowego.")]
        public string Email { get; set; }
    }
}
