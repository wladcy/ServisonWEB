using Rental.Models;
using Rental.Validators;
using ServisonWEB.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServisonWEB.Models.ServisonViewModels
{
    public class ClientViewModel: IPhoneModel
    {
        [Required(ErrorMessage ="Imię i nazwisko jest wymagane.")]
        [NameValidator]
        [Display(Name="Imię i nazwisko")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Numer telefonu jest wymagany.")]
        [PhoneValidator]
        [Phone(ErrorMessage = "Podano niepoprawny numer telefonu.")]
        [Display(Name ="Numer telefonu")]
        public string Phone { get; set; }

        [Display(Name ="Dodatkowe informacje")]
        public string Comment { get; set; }

        public List<Values> Names { get; set; }
    }
}
