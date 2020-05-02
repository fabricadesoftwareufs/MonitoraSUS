using Microsoft.Extensions.Configuration;
using Service.Interface;
using System;
using System.Net;
using System.Net.Mail;
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
                var email = "";
                if (!string.IsNullOrEmpty(emailDestino))
                    email = emailDestino;
                else
                    throw new Exception("Email não pode ser vazio!!");

                var mail = new MailMessage
                {
                    From = new MailAddress(_configuration["EmailSettings:Email"], "Monitora SUS"),
                };
                mail.To.Add(new MailAddress(email));

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
                Console.WriteLine("An exception ({0}) occurred.",
                     ex.GetType().Name);
                Console.WriteLine("   Message:\n{0}", ex.Message);
                Console.WriteLine("   Stack Trace:\n   {0}", ex.StackTrace);
                throw ex.InnerException;
            }
        }
    }
}
