using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System;
using System.Threading.Tasks;

namespace MonitoraSUS.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<ActionResult> RecuperarSenha()
        {
            try
            {
                await _emailService.SendEmailAsync("gabriel.sistemasjr@gmail.com", "MonitoraSUS", "Testando via controller");
                return RedirectToActionPermanent("Index", "Home", new { sendMail = "Sucesso" });
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }
    }
}