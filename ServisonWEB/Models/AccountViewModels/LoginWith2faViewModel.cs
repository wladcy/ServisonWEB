using System.ComponentModel.DataAnnotations;

namespace Default.Models.AccountViewModels
{
    public class LoginWith2faViewModel
    {
        [Required(ErrorMessage = "Kod weryfikacyjny jest wymagany")]
        [DataType(DataType.Text)]
        [Display(Name = "Kod weryfikacyjny")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "Zapamiętaj urządzenie")]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }
    }
}
