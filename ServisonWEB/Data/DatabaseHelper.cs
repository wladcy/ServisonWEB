﻿using Admin.Services;
using Default.Data;
using Microsoft.EntityFrameworkCore;
using ServisonWEB.Models;
using ServisonWEB.Models.ServisonViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ServisonWEB.Data
{
    public class DatabaseHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly Stopwatch s = new Stopwatch();

        public DatabaseHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Values> GetAllNames()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            List<Values> retval = new List<Values>();
            List<Client> clients = _context.Client.Include(x=>x.Name).Include(x=>x.LastName).ToList();
            foreach (Client client in clients)
            {
                Values v = new Values()
                {
                    Value = client.Name.Name + " " + client.LastName.LastName,
                    ID = client.ID
                };
                Values name = retval.Where(x => x.Value.Equals(v.Value)).FirstOrDefault();
                if (name == null)
                {
                    retval.Add(v);
                }
            }
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return retval;
        }

        public List<Values> GetAllBrands()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
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
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return retval;
        }

        public List<Values> GetAllModels()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
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
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return retval;
        }

        public List<Values> GetAllBrandsByUserData(string name, string phone)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            string[] tmp = name.Split(' ');
            string _name = tmp[0];
            string _lastName = string.Empty;
            if (tmp.Length > 1)
            {
                _lastName = tmp[1];
            }
            List<Values> retval = new List<Values>();
            List<Device> names = _context.Device.Include(x => x.Brand).
                Include(x => x.Client).Include(x => x.Client.Name).
                Include(x => x.Client.LastName).
                Where(d => d.Client.Name.Name.Equals(_name) &&
                    d.Client.LastName.LastName.Equals(_lastName) &&
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
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return retval;
        }

        public List<Values> GetModels(string name, string phone, string brand)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            string[] tmp = name.Split(' ');
            string _name = tmp[0];
            string _lastName = string.Empty;
            if (tmp.Length > 1)
            {
                _lastName = tmp[1];
            }
            List<Values> retval = new List<Values>();
            List<Device> names = _context.Device.Include(x => x.Brand).
                Include(x => x.Client).Include(x => x.Client.Name).
                Include(x => x.Client.LastName).Include(x => x.Model).
                Where(d => d.Client.Name.Name.Equals(_name) &&
                    d.Client.LastName.LastName.Equals(_lastName) &&
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
                names = _context.Device.Include(x => x.Brand).Include(x => x.Model).Where(d => d.Brand.Brand.Equals(brand)).ToList();
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
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return retval;
        }

        public void AddRepair(AddRepairViewModel data)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            Repair repair = new Repair();
            repair.Acceptance = data.Repair.DateOfAcceptance;
            repair.CreateTime = DateTime.Now;
            repair.RepairDetail = data.Repair.DamageDescription;
            repair.DeviceId = getDeviceId(data);
            _context.Entry(repair).State = EntityState.Added;
            _context.SaveChanges();
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
        }

        public Client GetClient(string name, string phone)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            string[] tmp = name.Split(' ');
            string _name = tmp[0];
            string _lastName = string.Empty;
            if (tmp.Length > 1)
            {
                _lastName = tmp[1];
            }
            Client client = _context.Client.Include(x => x.Name).
                Include(x => x.LastName).
                Where(c => c.LastName.LastName.Equals(_lastName) &&
                    c.Name.Name.Equals(_name) && c.Phone.Equals(phone)).
                FirstOrDefault();
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return client;
        }

        public Device GetDevice(string name, string phone, string brand, string model)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            string[] tmp = name.Split(' ');
            string _name = tmp[0];
            string _lastName = string.Empty;
            if (tmp.Length > 1)
            {
                _lastName = tmp[1];
            }
            Device device = _context.Device.Include(x => x.Client).
                Include(x => x.Client.LastName).Include(x => x.Client.Name).
                Include(x => x.Brand).Include(x => x.Model).
                Where(d => d.Client.Name.Name.Equals(_name) &&
                    d.Client.LastName.LastName.Equals(_lastName) &&
                    d.Client.Phone.Equals(phone) &&
                    d.Brand.Brand.Equals(brand) && d.Model.Model.Equals(model)).
                FirstOrDefault();
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return device;
        }

        public List<Repair> GetAllRepair()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            List<Repair> repairs = _context.Repair.Include(x => x.Device).
                Include(x => x.Device.Brand).Include(x => x.Device.Model).
                Include(x => x.Device.Client).
                Include(x => x.Device.Client.LastName).
                Include(x => x.Device.Client.Name).ToList();
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return repairs;
        }

        private int getDeviceId(AddRepairViewModel data)
        {
            string name = data.Client.Name.Split(' ')[0];
            string lastName = data.Client.Name.Split(' ')[1];
            Device device = _context.Device.Include(x => x.Brand).
                Include(x => x.Model).Include(x => x.Client).
                Where(d => d.Brand.Brand.Equals(data.Device.Brand) &&
                    d.Model.Model.Equals(data.Device.ModelName) && 
                    d.Client.Name.Name.Equals(name) && 
                    d.Client.LastName.LastName.Equals(lastName) && 
                    d.Client.Phone.Equals(data.Client.Phone)).FirstOrDefault();
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
            if (brand == null)
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
            string name = data.Name.Split(' ')[0];
            string lastName = data.Name.Split(' ')[1];
            Client client = _context.Client.Include(x => x.LastName).
                Include(x => x.Name).
                Where(c => c.Name.Name.Equals(name) &&
                    c.LastName.LastName.Equals(lastName) &&
                    c.Phone.Equals(data.Phone)).FirstOrDefault();
            if (client == null)
            {
                client = new Client();
                client.ClientLastNameId = getLastNameId(lastName);
                client.ClientNameId = getNameId(name);
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
            if (lastName == null)
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
