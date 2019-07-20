using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ProcessService.Mail
{
    public class MailManager
    {
        private readonly IConfiguration _config;

        public MailManager(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendRegistrationMessageAsync(string email, string userId, string confirmationToken)
        {
            try
            {
                var client = SetupMailClient();

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("info@foodlocator.com");
                mailMessage.To.Add(email);
                mailMessage.Subject = "You've been added to the FoodLocator platoform!";

                string callbackUrl = $"{_config["AppSettings:Web:BaseUrl"]}/Account/Confirm?id={userId}&code={confirmationToken}";
                mailMessage.Body = $"callbackurl: {callbackUrl}";
                
                await client.SendMailAsync(mailMessage);
                client.Dispose();                
            }
            catch(Exception ex)
            {
                throw ex;
            }            
        }

        public async Task SendForgotPasswordMessageAsync(string email, string code)
        {
            try
            {
                var client = SetupMailClient();

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("info@foodlocator.com");
                mailMessage.To.Add(email);
                mailMessage.Subject = "Reset Your Password";                               

                string callbackUrl = $"{_config["AppSettings:Web:BaseUrl"]}/Account/ResetPassword?email={email}&code={code}";
                mailMessage.Body = $"callbackurl: {callbackUrl}";

                await client.SendMailAsync(mailMessage);
                client.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SmtpClient SetupMailClient()
        {
            SmtpClient client = new SmtpClient(_config["AppSettings:Email:Host"]);
            client.Port = Int32.Parse(_config["AppSettings:Email:Port"]);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_config["AppSettings:Email:Userame"], _config["AppSettings:Email:Password"]);

            return client;
        }

        
    }
}
