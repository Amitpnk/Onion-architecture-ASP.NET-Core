using Microsoft.Extensions.Options;
using OA.Domain;
using OA.Service.Contract;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OA.Service.Implementation
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
            message.To.Add(new MailAddress(mailRequest.ToEmail));
            message.Subject = mailRequest.Subject;

            message.IsBodyHtml = false;
            message.Body = mailRequest.Body;
            smtp.Port = _mailSettings.Port;
            smtp.Host = _mailSettings.Host;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            await smtp.SendMailAsync(message);
        }

    }
}