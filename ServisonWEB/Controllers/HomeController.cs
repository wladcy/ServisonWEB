using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Default.Models;
using Microsoft.AspNetCore.Diagnostics;
using Admin.Services;
using Default.Services;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Default.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(IndexViewModel model)
        { 
            return View(model);
        }

        public IActionResult Error()
        {
            var exceptionData = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionData.Error;
            LoggerController.AddExceptionLog(this.GetType().Name, exception);
            EmailSender smc = new EmailSender();
            smc.IsHtmlBody = false;
            smc.SendEmailAsync("mateusz.slezak1@gmail.com", SettingsController.AppName.Name + " - Error", "" +
                "Wiadomość błędu: " + exception.Message + "\nStos wyjątku: " +
                exception.StackTrace + "\nWyjątek: " + exception.InnerException);

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
