using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServisonWEB.Models.ServisonViewModels
{
    public class DeviceViewModel
    {
        [Required(ErrorMessage ="Marka jest wymagana.")]
        [Display(Name ="Marka")]
        public string Brand { get; set; }

        [Required(ErrorMessage ="Model jest wymagany.")]
        [Display(Name ="Model")]
        public string ModelName { get; set; }

        [Display(Name = "Dodatkowe informacje")]
        public string Comments { get; set; }

        public List<Values> Brands { get; set; } = new List<Values>();
        public List<Values> ModelNames { get; set; } = new List<Values>();
    }
}
