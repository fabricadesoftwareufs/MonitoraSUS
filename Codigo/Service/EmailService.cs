using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Model;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string emailDestino, string assunto, string mensagem)
        {
            try
            {
                string toEmail = string.IsNullOrEmpty(emailDestino) ? _configuration["EmailSettings:DestinatarioPadrao"] : emailDestino;

                var mail = new MailMessage
                {
                    From = new MailAddress(_configuration["EmailSettings:Email"], "Fabrica de Software"),
                };
                mail.To.Add(new MailAddress(toEmail));
                mail.CC.Add(new MailAddress(_configuration["EmailSettings:Backup"]));

                mail.Subject = assunto;
                mail.Body = mensagem;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                //outras opções
                //mail.Attachments.Add(new Attachment(arquivo));
                //

                var smtp = new SmtpClient(_configuration["EmailSettings:Dominio"], Convert.ToInt32(_configuration["EmailSettings:Porta"]))
                {
                    Credentials = new NetworkCredential(_configuration["EmailSettings:Email"], _configuration["EmailSettings:Senha"]),
                    EnableSsl = true
                };
                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
