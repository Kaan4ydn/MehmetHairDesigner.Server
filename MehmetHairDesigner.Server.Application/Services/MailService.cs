using MehmetHairDesigner.Server.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Services
{
    public class MailService : IMailService
    {
        public async Task SendAsync(string to, string subject, string body)
        {
            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("halidansaha@gmail.com", "dbjcoiegcnufkweu"),
                EnableSsl = true
            };

            var mail = new MailMessage("halidansaha@gmail.com", to, subject, body);
            await smtp.SendMailAsync(mail);
        }
    }
}
