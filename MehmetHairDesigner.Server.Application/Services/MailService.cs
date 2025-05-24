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
            using var smtp = new SmtpClient("smtp.sunucum.com")
            {
                Credentials = new NetworkCredential("kullanici", "sifre"),
                EnableSsl = true
            };

            var mail = new MailMessage("noreply@kuafor.com", to, subject, body);
            await smtp.SendMailAsync(mail);
        }
    }
}
