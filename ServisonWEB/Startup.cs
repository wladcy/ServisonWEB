using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Default.Data;
using Default.Models;
using Default.Services;
using Admin.Services;
using Microsoft.Extensions.Hosting;

namespace Default
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            SettingsController.MailAccessModel.Mail = Configuration.GetSection("Mail").GetSection("mailAccount").Value;
            SettingsController.MailAccessModel.Password = Configuration.GetSection("Mail").GetSection("mailPassword").Value;
            SettingsController.MailAccessModel.Host = Configuration.GetSection("Mail").GetSection("host").Value;
            SettingsController.MailAccessModel.Port = Configuration.GetSection("Mail").GetSection("port").Value;
            SettingsController.MailAccessModel.EnableSSL = bool.Parse(Configuration.GetSection("Mail").GetSection("enableSsl").Value);
            SettingsController.AppName.Name = Configuration.GetSection("AppName").GetSection("Name").Value;
            LoggerController.Initialize();

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpints =>
            {
                endpints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
