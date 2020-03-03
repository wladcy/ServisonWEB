using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Default.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "PotwierdŸ maila",
                $"Prosze potwierdziæ maila poprzez wejœcie w <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}
