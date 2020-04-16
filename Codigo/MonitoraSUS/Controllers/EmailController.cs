﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Service.Interface;

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