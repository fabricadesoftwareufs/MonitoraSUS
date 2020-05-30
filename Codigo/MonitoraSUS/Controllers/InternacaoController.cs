using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service.Interface;

namespace MonitoraSUS.Controllers
{
    [Authorize(Roles = "GESTOR, SECRETARIO")]
    public class InternacaoController : Controller
    {
        private readonly IInternacaoService _internacaoContext;

        public InternacaoController(IInternacaoService internacaoContext)
        {
            _internacaoContext = internacaoContext;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(InternacaoModel internacao, int idVirusBacteria)
        {

            var idPaciente = internacao.IdPessoa;
            try
            {
                _internacaoContext.Insert(internacao);
            }
            catch
            {
                TempData["mensagemErro"] = "Houve problemas na insercao da internacao. Tente novamente em alguns minutos." +
                                            " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";

                
                return RedirectToAction("Edit", "MonitorarPaciente", new { idPaciente, idVirusBacteria });
            }

            TempData["mensagemSucessoInternacao"] = "Internacao cadastrada com sucesso!";
            return RedirectToAction("Edit", "MonitorarPaciente", new { idPaciente, idVirusBacteria });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(InternacaoModel internacao, int idVirusBacteria)
        {
            var idPaciente = internacao.IdPessoa;
            try
            {
                _internacaoContext.update(internacao);
            }
            catch
            {
                TempData["mensagemErro"] = "Houve problemas ao atualizar a internacao. Tente novamente em alguns minutos." +
                                            " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";
                return RedirectToAction("Edit", "MonitorarPaciente", new { idPaciente, idVirusBacteria });
            }

            TempData["mensagemSucessoInternacao"] = "Internacao atualizada com sucesso!";
            return RedirectToAction("Edit", "MonitorarPaciente", new { idPaciente, idVirusBacteria });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int idInternacao, int idPaciente, int idVirusBacteria)
        {

            try
            {
                _internacaoContext.Delete(idInternacao);
            }
            catch
            {
                TempData["mensagemErro"] = "Houve problemas na insercao da internacao. Tente novamente em alguns minutos." +
                                            " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";
                return RedirectToAction("Edit", "MonitorarPaciente", new { idPaciente, idVirusBacteria });
            }

            TempData["mensagemSucessoInternacao"] = "Internacao removida com sucesso!";
            return RedirectToAction("Edit", "MonitorarPaciente", new { idPaciente, idVirusBacteria });
        }
    }
}