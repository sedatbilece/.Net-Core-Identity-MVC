using AspNetCoreIdentity.Web.OptionsModel;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace AspNetCoreIdentity.Web.Services
{
    public class EmailService : IEmailService
    {

        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public async Task SendResetEmail(string resetLink, string toEmail)
        {

            var smptClient = new SmtpClient();

            smptClient.Host = _emailSettings.Host;
            smptClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smptClient.UseDefaultCredentials = true;
            smptClient.Port = 587;
            smptClient.Credentials= new NetworkCredential(_emailSettings.Email,_emailSettings.Password);
            smptClient.EnableSsl = true;

            var mailMessage = new MailMessage();

            mailMessage.From= new MailAddress(_emailSettings.Email);
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = "Şifre sıfırlama linki";

            mailMessage.Body = $"<h4>Şifre yenileme için aşağıdaki linke tıklayınız</h4>\r\n\r\n                <p>\r\n                    <a href='{{resetLink}}'></a>\r\n                    \r\n                </p>";

            mailMessage.IsBodyHtml= true;

           await  smptClient.SendMailAsync(mailMessage);

            
        }
    }
}
