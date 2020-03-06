using Default.Data;
using Microsoft.EntityFrameworkCore;
using ServisonWEB.Models;
using ServisonWEB.Models.ServisonViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServisonWEB.Data
{
    public class DatabaseHelper
    {
        private readonly ApplicationDbContext _context;

        public DatabaseHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Values> GetAllNames()
        {
            List<Values> retval = new List<Values>();
            List<Names> names = _context.Name.ToList();
            foreach(Names name in names)
            {
                Values v = new Values()
                {
                    Value = name.Name,
                    ID = name.ID
                };
                retval.Add(v);
            }
            return retval;
        }

        public List<Values> GetAllLastNames()
        {
            List<Values> retval = new List<Values>();
            List<LastNames> names = _context.LastName.ToList();
            foreach (LastNames name in names)
            {
                Values v = new Values()
                {
                    Value = name.LastName,
                    ID = name.ID
                };
                retval.Add(v);
            }
            return retval;
        }

        public List<Values> GetAllBrands()
        {
            List<Values> retval = new List<Values>();
            List<Brands> names = _context.Brand.ToList();
            foreach (Brands name in names)
            {
                Values v = new Values()
                {
                    Value = name.Brand,
                    ID = name.ID
                };
                retval.Add(v);
            }
            return retval;
        }

        public List<Values> GetAllModels()
        {
            List<Values> retval = new List<Values>();
            List<Default.Data.Models> names = _context.Models.ToList();
            foreach (Default.Data.Models name in names)
            {
                Values v = new Values()
                {
                    Value = name.Model,
                    ID = name.ID
                };
                retval.Add(v);
            }
            return retval;
        }

        public List<Values> GetAllBrandsByUserData(string name, string lastName, string phone)
        {
            List<Values> retval = new List<Values>();
            List<Device> names = _context.Device.Include(x => x.Brand).
                Include(x => x.Client).Include(x => x.Client.Name).
                Include(x => x.Client.LastName).
                Where(d => d.Client.Name.Name.Equals(name) && 
                    d.Client.LastName.LastName.Equals(lastName) && 
                    d.Client.Phone.Equals(phone)).ToList();
            if (names.Count > 0)
            {
                foreach (Device device in names)
                {
                    Values v = new Values()
                    {
                        Value = device.Brand.Brand,
                        ID = device.BrandId
                    };
                    retval.Add(v);
                }
            }
            else
            {
                names = _context.Device.Include(x => x.Brand).ToList();
                foreach (Device device in names)
                {
                    Values v = new Values()
                    {
                        Value = device.Brand.Brand,
                        ID = device.BrandId
                    };
                    retval.Add(v);
                }
            }
            return retval;
        }

        public List<Values> GetModels(string name, string lastName, string phone, string brand)
        {
            List<Values> retval = new List<Values>();
            List<Device> names = _context.Device.Include(x => x.Brand).
                Include(x => x.Client).Include(x => x.Client.Name).
                Include(x => x.Client.LastName).Include(x=>x.Model).
                Where(d => d.Client.Name.Name.Equals(name) &&
                    d.Client.LastName.LastName.Equals(lastName) &&
                    d.Client.Phone.Equals(phone) && d.Brand.Brand.Equals(brand)).ToList();
            if (names.Count > 0)
            {
                foreach (Device device in names)
                {
                    Values v = new Values()
                    {
                        Value = device.Model.Model,
                        ID = device.BrandId
                    };
                    retval.Add(v);
                }
            }
            else
            {
                names = _context.Device.Include(x => x.Brand).Include(x=>x.Model).Where(d => d.Brand.Brand.Equals(brand)).ToList();
                foreach (Device device in names)
                {
                    Values v = new Values()
                    {
                        Value = device.Model.Model,
                        ID = device.BrandId
                    };
                    retval.Add(v);
                }
            }
            return retval;
        }

        public void AddRepair(AddRepairViewModel data)
        {
            Repair repair = new Repair();
            repair.Acceptance = data.Repair.DateOfAcceptance;
            repair.CreateTime = DateTime.Now;
            repair.RepairDetail = data.Repair.DamageDescription;
            repair.DeviceId = getDeviceId(data);
            _context.Entry(repair).State = EntityState.Added;
            _context.SaveChanges();
        }

        public Client GetClient(string name, string lastName, string phone)
        {
            Client client = _context.Client.Include(x => x.Name).
                Include(x => x.LastName).
                Where(c => c.LastName.LastName.Equals(lastName) && 
                    c.Name.Name.Equals(name) && c.Phone.Equals(phone)).
                FirstOrDefault();
            return client;
        }

        public Device GetDevice(string name, string lastName, string phone, string brand, string model)
        {
            Device device = _context.Device.Include(x => x.Client).
                Include(x => x.Client.LastName).Include(x=>x.Client.Name).
                Include(x=>x.Brand).Include(x=>x.Model).
                Where(d=>d.Client.Name.Name.Equals(name) && 
                    d.Client.LastName.LastName.Equals(lastName) && 
                    d.Client.Phone.Equals(phone) && 
                    d.Brand.Brand.Equals(brand) && d.Model.Model.Equals(model)).
                FirstOrDefault();
            return device;
        }

        private int getDeviceId(AddRepairViewModel data)
        {
            Device device = _context.Device.Include(x => x.Brand).
                Include(x => x.Model).Include(x=>x.Client).
                Where(d => d.Brand.Brand.Equals(data.Device.Brand) && 
                    d.Model.Model.Equals(data.Device.ModelName)).FirstOrDefault();
            if (device == null)
            {
                device = new Device();
                device.BrandId = getBrandId(data.Device.Brand);
                device.ClientId = getClientId(data.Client);
                device.Comment = data.Device.Comments;
                device.CreateTime = DateTime.Now;
                device.ModelId = getModelId(data.Device.ModelName);
                _context.Entry(device).State = EntityState.Added;
            }
            else
            {
                device.Client.Comment = data.Client.Comment;
                device.Comment = data.Device.Comments;
                _context.Entry(device).State = EntityState.Modified;
            }
            _context.SaveChanges();
            return device.ID;
        }

        private int getBrandId(string data)
        {
            Brands brand = _context.Brand.Where(b => b.Brand.Equals(data)).FirstOrDefault();
            if(brand == null)
            {
                brand = new Brands();
                brand.Brand = data;
                brand.CreateTime = DateTime.Now;
                _context.Entry(brand).State = EntityState.Added;
                _context.SaveChanges();
            }
            return brand.ID;
        }

        private int getClientId(ClientViewModel data)
        {
            Client client = _context.Client.Include(x => x.LastName).
                Include(x => x.Name).
                Where(c => c.Name.Name.Equals(data.Name) && 
                    c.LastName.LastName.Equals(data.LastName) && 
                    c.Phone.Equals(data.Phone)).FirstOrDefault();
            if (client == null)
            {
                client = new Client();
                client.ClientLastNameId = getLastNameId(data.LastName);
                client.ClientNameId = getNameId(data.Name);
                client.Comment = data.Comment;
                client.CreateTime = DateTime.Now;
                client.Phone = data.Phone;
                _context.Entry(client).State = EntityState.Added;
            }
            else
            {
                client.Comment = data.Comment;
                _context.Entry(client).State = EntityState.Modified;
            }
            _context.SaveChanges();
            return client.ID;
        }

        private int getLastNameId(string data)
        {
            LastNames lastName = _context.LastName.Where(l => l.LastName.Equals(data)).FirstOrDefault();
            if(lastName == null)
            {
                lastName = new LastNames();
                lastName.CreateTime = DateTime.Now;
                lastName.LastName = data;
                _context.Entry(lastName).State = EntityState.Added;
                _context.SaveChanges();
            }
            return lastName.ID;
        }

        private int getNameId(string data)
        {
            Names name = _context.Name.Where(n => n.Name.Equals(data)).FirstOrDefault();
            if (name == null)
            {
                name = new Names();
                name.CreateTime = DateTime.Now;
                name.Name = data;
                _context.Entry(name).State = EntityState.Added;
                _context.SaveChanges();
            }
            return name.ID;
        }

        private int getModelId(string data)
        {
            Default.Data.Models model = _context.Models.Where(m => m.Model.Equals(data)).FirstOrDefault();
            if (model == null)
            {
                model = new Default.Data.Models();
                model.CreateTime = DateTime.Now;
                model.Model = data;
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
            }
            return model.ID;
        }
    }
}
