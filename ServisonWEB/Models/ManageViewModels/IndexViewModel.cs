using Rental.Models;
using Rental.Validators;
using System.ComponentModel.DataAnnotations;

namespace Default.Models.ManageViewModels
{
    public class IndexViewModel
    {
        [Display(Name = "Nazwa użytkownika")]
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string StatusMessage { get; set; }
    }
}
