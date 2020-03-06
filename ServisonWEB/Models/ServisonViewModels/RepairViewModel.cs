using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServisonWEB.Models.ServisonViewModels
{
    public class RepairViewModel
    {
        [Required(ErrorMessage ="Opis usterki jest wymagany.")]
        [Display(Name ="Opis usterki")]
        public string DamageDescription { get; set; }

        [Required(ErrorMessage = "Data przyjęcia jest wymagana.")]
        [DataType(DataType.Date, ErrorMessage = "Podano niepoprawny format daty.")]
        [Display(Name = "Data przyjęcia")]
        public DateTime DateOfAcceptance { get; set; } = DateTime.Today;
    }
}
