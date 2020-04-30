using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model;
using MonitoraSUS.Utils;
using Service.Interface;
using System.Collections.Generic;
using System.Security.Claims;

namespace MonitoraSUS.Controllers
{
    [Authorize(Roles = "COORDENADOR, SECRETARIO")]
    public class EmpresaExameController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IEmpresaExameService _empresaContext;
        private readonly IExameService _exameContext;
        private readonly IPessoaService _pessoaContext;
        private readonly IPessoaTrabalhaEstadoService _trabalhaEstadoContext;



        public EmpresaExameController(IConfiguration configuration,
                                      IEmpresaExameService empresaContext,
                                      IExameService exameContext,
                                      IPessoaService pessoaContext,
                                      IPessoaTrabalhaEstadoService trabalhaEstadoContext)
        {
            _configuration = configuration;
            _empresaContext = empresaContext;
            _exameContext = exameContext;
            _pessoaContext = pessoaContext;
            _trabalhaEstadoContext = trabalhaEstadoContext;
        }

        public IActionResult Index()
        {
            return View(GetAllEmpresas());
        }

        public List<EmpresaExameModel> GetAllEmpresas() 
        {
            var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);
            
            var empresas = new List<EmpresaExameModel>();
            if (usuario.RoleUsuario.Equals("SECRETARIO") || usuario.RoleUsuario.Equals("COORDENADOR"))
            {
                empresas = _empresaContext.GetByUF(_pessoaContext.GetById(usuario.UsuarioModel.IdPessoa).Estado);
            }

            return empresas;
                
        }

        public IActionResult Details(int id)
        {
            return View(_empresaContext.GetById(id));
        }

        public IActionResult Create()
        {
            ViewBag.googleKey = _configuration["GOOGLE_KEY"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmpresaExameModel empresa)
        {
            ViewBag.googleKey = _configuration["GOOGLE_KEY"];
            if (Methods.ValidarCnpj(empresa.Cnpj))
            {

                if (VerificaQtdLeitos(empresa) == 1)
                {
                    TempData["MensagemErro"] = "A quantidade de leitos disponíveis não pode ser menor do que " +
                                               "a quantidade total de leitos!";
                    return View(empresa);
                }
                else if (VerificaQtdLeitos(empresa) == 2)
                {
                    TempData["MensagemErro"] = "A quantidade de leitos não pode ser um valor menor que zero!";
                    return View(empresa);
                }

                empresa = RemoveCaracteresEspeciais(empresa);
                if (_empresaContext.GetByCnpj(empresa.Cnpj) == null)
                {
                    try
                    {
                        _empresaContext.Insert(empresa);
                    }
                    catch
                    {
                        TempData["MensagemErro"] = "Algo deu errado, tente novamente.";
                        return View(empresa);
                    }

                    TempData["MensagemSucesso"] = "Organização Cadastrada com sucesso!";
                    return View();
                }
                else
                {
                    TempData["MensagemCnpj"] = "Já existe uma empresa com este CNPJ.";
                    return View(empresa);
                }

            }
            else
            {
                TempData["MensagemCnpj"] = "Esse CNPJ não é válido.";
                return View(empresa);
            }
        }

        public IActionResult Edit(int id)
        {
            ViewBag.googleKey = _configuration["GOOGLE_KEY"];
            return View(_empresaContext.GetById(id));
        }

        /// <summary>
        /// Edita um exame existente da base de dados
        /// </summary>
        /// <param name="exame"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EmpresaExameModel empresa)
        {
            ViewBag.googleKey = _configuration["GOOGLE_KEY"];

            if (Methods.ValidarCnpj(empresa.Cnpj))
            {
                empresa = RemoveCaracteresEspeciais(empresa);
                if (_empresaContext.GetByCnpj(empresa.Cnpj).Id == empresa.Id)
                {

                    if (VerificaQtdLeitos(empresa) == 1)
                    {
                        TempData["MensagemErro"] = "A quantidade de leitos disponíveis não pode ser menor do que " +
                                                   "a quantidade total de leitos!";
                        return View(empresa);
                    }
                    else if (VerificaQtdLeitos(empresa) == 2)
                    {
                        TempData["MensagemErro"] = "A quantidade de leitos não pode ser um valor menor que zero!";
                        return View(empresa);
                    }

                    try
                    {
                        _empresaContext.Update(empresa);
                    }
                    catch
                    {
                        TempData["MensagemErro"] = "Algo deu errado, tente novamente.";
                        return View(empresa);
                    }

                    TempData["MensagemSucesso"] = "Atualização feita com sucesso!";
                    return View();
                }
                else
                {
                    TempData["MensagemCnpj"] = "Já existe uma empresa com este CNPJ.";
                    return View(empresa);
                }

            }
            else
            {
                TempData["MensagemCnpj"] = "Esse CNPJ não é válido.";
                return View(empresa);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection collection)
        {
            var exames = _exameContext.GetByIdEmpresa(id);
            var pessoaEstado = _trabalhaEstadoContext.GetByIdEmpresa(id);

            try
            {
                if (exames == null && pessoaEstado == null)
                {
                    _empresaContext.Delete(id);
                    TempData["mensagemSucesso"] = "Laboratório removido com sucesso!";
                }
                else
                {
                    TempData["mensagemErro"] = "Esse laboratório não pode ser removido pois existem " +
                                                  "profissionais e exames cadastrados que dependem dele!";
                }

                
            }
            catch
            {
                TempData["mensagemErro"] = "Houve um problema ao remover laboratório, tente novamente!";
            }

            return RedirectToAction(nameof(Index));
        }

        public int VerificaQtdLeitos(EmpresaExameModel empresa) 
        {
            var status = 0; 
            if (empresa.PossuiLeitosInternacao)
            {
                if (empresa.NumeroLeitosDisponivel > empresa.NumeroLeitos)
                    status = 1;

                if (empresa.NumeroLeitosUtidisponivel > empresa.NumeroLeitosUti)
                    status = 1;

                if (empresa.NumeroLeitosDisponivel < 0 || empresa.NumeroLeitos < 0 ||
                    empresa.NumeroLeitosUtidisponivel < 0 || empresa.NumeroLeitosUti < 0)
                    status =  2;
            }

            return status;
        }


        public EmpresaExameModel RemoveCaracteresEspeciais(EmpresaExameModel empresa)
        {
            empresa.Cep = Methods.RemoveSpecialsCaracts(empresa.Cep);
            empresa.FoneCelular = Methods.RemoveSpecialsCaracts(empresa.FoneCelular);
            empresa.Cnpj = Methods.RemoveSpecialsCaracts(empresa.Cnpj);

            if (empresa.FoneFixo != null)
                empresa.FoneFixo = Methods.RemoveSpecialsCaracts(empresa.FoneFixo);

            return empresa;
        }
    }
}