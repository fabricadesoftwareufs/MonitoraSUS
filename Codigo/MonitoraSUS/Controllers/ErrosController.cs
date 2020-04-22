using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace MonitoraSUS.Controllers
{
    public class ErrosController : Controller
    {

        private readonly IEmailService _emailService;

        public ErrosController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet("Error/500")]
        public IActionResult Error500()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var mensagem = "";
            if (exceptionFeature != null)
            {
                ViewBag.ErrorMessage = "Alguma coisa deu errado no servidor";
                ViewBag.MensagemOrientacao = "Ocorreu um erro inesperado, por favor tente executar a operação novamente. " +
                           " Se o erro persistir, por favor, entre em contato com a Fábrica de Software da UFS pelo email ";

                mensagem = " Exceção: " + exceptionFeature.Error.Message + "\n" +
                 " - " + exceptionFeature.Error.StackTrace + "\n" +
                 " - " + exceptionFeature.Error.InnerException + "\n" +
                 " - " + exceptionFeature.Path;
            }

            sendMessage(mensagem);

            return View();
        }

        [HttpGet("Error/{statusCode}")]
        public IActionResult HandleErrorCode(int statusCode)
        {

            switch (statusCode)
            {
                case 401:
                    ViewBag.ErrorMessage = "Você não tem permissão para acessar essa página";
                    ViewBag.MensagemOrientacao = "Caso seja necessário acessar essa página, por favor, entre em contato com a " +
                                                 "Fábrica de Software da UFS pelo email ";

                    break;
                case 404:
                    ViewBag.ErrorMessage = "A página requisitada não foi encontrada";
                    ViewBag.MensagemOrientacao = "Ocorreu um erro inesperado, por favor tente executar a operação novamente. " +
                               " Se o erro persistir, por favor, entre em contato com a Fábrica de Software da UFS pelo email ";

                    break;
                case 500:
                    ViewBag.ErrorMessage = "Alguma coisa deu errado no servidor";
                    ViewBag.MensagemOrientacao = "Ocorreu um erro inesperado, por favor tente executar a operação novamente. " +
                               " Se o erro persistir, por favor, entre em contato com a Fábrica de Software da UFS pelo email ";
                    break;

                default:
                    ViewBag.ErrorMessage = "Alguma coisa deu errado no sistema";
                    ViewBag.MensagemOrientacao = "Ocorreu um erro inesperado, por favor tente executar a operação novamente. " +
                               " Se o erro persistir, por favor, entre em contato com a Fábrica de Software da UFS pelo email ";
                    break;
            }

            var statusCodeData = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            var mensagem = " Código da exceção: " + statusCode + " - " +
                           " Caminho: " + statusCodeData.OriginalPath;

            sendMessage(mensagem);

            return View();
        }

        public void sendMessage(string mensagem)
        {
            _emailService.SendEmailAsync("monitorasus.ufs@gmail.com", "MonitoraSUS - LANÇAMENTO DE EXCEÇÃO NO APLICATIVO", mensagem);
        }
    }
}