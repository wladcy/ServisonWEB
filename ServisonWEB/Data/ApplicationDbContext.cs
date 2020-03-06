using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Default.Models;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Default.Data
{
    public class Names
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class LastNames
    {
        [Key]
        public int ID { get; set; }
        public string LastName { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class Client
    {
        [Key]
        public int ID { get; set; }
        public int ClientNameId { get; set; }
        public int ClientLastNameId { get; set; }
        public string Phone { get; set; }
        public string Comment { get; set; }
        public DateTime CreateTime { get; set; }

        [ForeignKey("ClientNameId")]
        public virtual Names Name { get; set; }

        [ForeignKey("ClientLastNameId")]
        public virtual LastNames LastName { get; set; }
    }

    public class Brands
    {
        [Key]
        public int ID { get; set; }

        public string Brand { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class Models
    {
        [Key]
        public int ID { get; set; }

        public string Model { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class Device
    {
        [Key]
        public int ID { get; set; }

        public int BrandId { get; set; }
        public int ModelId { get; set; }
        public int ClientId { get; set; }
        public string Comment { get; set; }
        public DateTime CreateTime { get; set; }

        [ForeignKey("BrandId")]
        public virtual Brands Brand { get; set; }

        [ForeignKey("ModelId")]
        public virtual Models Model { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

    }

    public class Repair
    {
        [Key]
        public int ID { get; set; }

        public int DeviceId { get; set; }
        public string RepairDetail { get; set; }
        public DateTime Acceptance { get; set; }
        public DateTime CreateTime { get; set; }

        [ForeignKey("DeviceId")]
        public virtual Device Device { get; set; }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Names> Name { get; set; }
        public virtual DbSet<LastNames> LastName { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<Brands> Brand { get; set; }
        public virtual DbSet<Models> Models { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<Repair> Repair { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
