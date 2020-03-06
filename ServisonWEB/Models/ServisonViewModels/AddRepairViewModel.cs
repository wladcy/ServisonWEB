using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServisonWEB.Models.ServisonViewModels
{
    public class AddRepairViewModel
    {
        public ClientViewModel Client { get; set; }
        public DeviceViewModel Device { get; set; } = new DeviceViewModel();
        public RepairViewModel Repair { get; set; } = new RepairViewModel();
        public string StatusMessage { get; set; }
        public int Number { get; set; }
    }
}
