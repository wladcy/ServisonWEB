using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Default.Models.ManageViewModels
{
    public class EnableAuthenticatorViewModel
    {
        [Required(ErrorMessage = "Kod weryfikacyjny jest wymagany")]
        [StringLength(7, ErrorMessage = "Kod składa się z 6-7 znaków", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Kod weryfikacyjny")]
        public string Code { get; set; }

        [BindNever]
        public string SharedKey { get; set; }

        [BindNever]
        public string AuthenticatorUri { get; set; }
    }
}
