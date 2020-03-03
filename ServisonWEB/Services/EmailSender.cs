using Admin.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;

namespace Default.Services
{
    
    public class EmailSender : IEmailSender
    {
        public bool IsHtmlBody { get; set; } = true;
        private Stopwatch s = new Stopwatch();

        public Task SendEmailAsync(string email, string subject, string message)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var fromAddress = new MailAddress(SettingsController.MailAccessModel.Mail, SettingsController.AppName.Name);
            var toAddress = new MailAddress(email);
            string fromPassword = SettingsController.MailAccessModel.Password;
            string body = message;

            var smtp = new SmtpClient
            {
                Host = SettingsController.MailAccessModel.Host,
                Port = int.Parse(SettingsController.MailAccessModel.Port),
                EnableSsl = SettingsController.MailAccessModel.EnableSSL,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var mail = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                mail.IsBodyHtml = IsHtmlBody;
                smtp.Send(mail);
            }
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return Task.FromResult(0);
        }
    }
}
