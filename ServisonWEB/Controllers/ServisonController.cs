using Default.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServisonWEB.Data;
using ServisonWEB.Models;
using ServisonWEB.Models.ServisonViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServisonWEB.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ServisonController : Controller
    {
        private readonly DatabaseHelper dh;
        public ServisonController(ApplicationDbContext context)
        {
            dh = new DatabaseHelper(context);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(createEmptyModel());
        }

        [HttpPost]
        public IActionResult Index(AddRepairViewModel model)
        {
            if (ModelState.IsValid)
            {
                dh.AddRepair(model);
                model = createEmptyModel();
                model.StatusMessage = "Dane naprawy zostały zapisane.";
            }
            return View(model);
        }

        [HttpGet]
        public string GetBrandsByUserData(string name, string lastName, string phone)
        {
            string retval = string.Empty;
            List<Values> brands = dh.GetAllBrandsByUserData(name, lastName, phone);
            if (brands.Count > 0)
            {
                retval = JsonConvert.SerializeObject(brands);
            }
            return retval;
        }

        [HttpGet]
        public string GetModels(string name, string lastName, string phone, string brand)
        {
            string retval = string.Empty;
            List<Values> brands = dh.GetModels(name, lastName, phone, brand);
            if (brands.Count > 0)
            {
                retval = JsonConvert.SerializeObject(brands);
            }
            return retval;
        }

        [HttpGet]
        public string GetClientComment(string name, string lastName, string phone)
        {
            string retval = string.Empty;
            Client client = dh.GetClient(name, lastName, phone);
            if (client != null)
            {
                retval = client.Comment;
            }
            return retval;
        }

        [HttpGet]
        public string GetDeviceComment(string name, string lastName, string phone, string brand, string model)
        {
            string retval = string.Empty;
            Device device = dh.GetDevice(name, lastName, phone, brand, model);
            if (device != null)
            {
                retval = device.Comment;
            }
            return retval;
        }

        [HttpGet]
        public IActionResult AllRepairs()
        {
            List<Repair> repairs = dh.GetAllRepair();
            List<AddRepairViewModel> model = new List<AddRepairViewModel>();
            int i = 0;
            foreach (Repair repair in repairs)
            {
                i++;
                AddRepairViewModel addRepair = createEmptyModel();
                addRepair.Client.LastName = repair.Device.Client.LastName.LastName;
                addRepair.Client.Name = repair.Device.Client.Name.Name;
                addRepair.Client.Phone = repair.Device.Client.Phone;
                addRepair.Device.Brand = repair.Device.Brand.Brand;
                addRepair.Device.ModelName = repair.Device.Model.Model;
                addRepair.Repair.DamageDescription = repair.RepairDetail;
                addRepair.Repair.DateOfAcceptance = repair.Acceptance;
                addRepair.Number = i;
                model.Add(addRepair);
            }
            return View(model);
        }

        private AddRepairViewModel createEmptyModel()
        {
            AddRepairViewModel model = new AddRepairViewModel();
            ClientViewModel clientModel = new ClientViewModel();
            clientModel.Names = dh.GetAllNames();
            clientModel.LastNames = dh.GetAllLastNames();
            model.Client = clientModel;
            DeviceViewModel deviceModel = new DeviceViewModel();
            model.Device = deviceModel;
            return model;
        }
    }
}
