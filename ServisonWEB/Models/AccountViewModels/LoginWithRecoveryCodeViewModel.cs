using System.ComponentModel.DataAnnotations;

namespace Default.Models.AccountViewModels
{
    public class LoginWithRecoveryCodeViewModel
    {
        [Required(ErrorMessage = "Kod bezpieczeństwa jest wymagany")]
        [DataType(DataType.Text)]
        [Display(Name = "Kod bezpieczeństwa")]
        public string RecoveryCode { get; set; }
    }
}
