using System.ComponentModel.DataAnnotations;

namespace Default.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required(ErrorMessage = "Mail jest wymagany.")]
        [EmailAddress(ErrorMessage = "Podano niepoprawny mail.")]
        [Display(Name = "Mail")]
        public string Email { get; set; }
    }
}
