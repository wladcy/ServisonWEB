using Rental.Models;
using Rental.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServisonWEB.Models.ServisonViewModels
{
    public class ClientViewModel: IPhoneModel
    {
        [Required(ErrorMessage ="Imię jest wymagane.")]
        [Display(Name="Imię")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Nazwisko jest wymagane.")]
        [Display(Name="Nazwisko")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="Numer telefonu jest wymagany.")]
        [PhoneValidator]
        [Phone(ErrorMessage = "Podano niepoprawny numer telefonu.")]
        [Display(Name ="Numer telefonu")]
        public string Phone { get; set; }

        [Display(Name ="Dodatkowe informacje")]
        public string Comment { get; set; }

        public List<Values> Names { get; set; }
        public List<Values> LastNames { get; set; }
    }
}
