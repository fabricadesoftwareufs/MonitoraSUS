using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model;
using MonitoraSUS.Utils;
using Service.Interface;

namespace MonitoraSUS.Controllers
{
    public class EmpresaExameController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IEmpresaExameService _empresaContext;
        private readonly IExameService _exameContext;
        private readonly IPessoaTrabalhaEstadoService _trabalhaEstadoContext;


        public EmpresaExameController(IConfiguration configuration,
                                      IEmpresaExameService empresaContext,
                                      IExameService exameContext,
                                      IPessoaTrabalhaEstadoService trabalhaEstadoContext)
        {
            _configuration = configuration;
            _empresaContext = empresaContext;
            _exameContext = exameContext;
            _trabalhaEstadoContext = trabalhaEstadoContext;
        }

        public IActionResult Index()
        {
            return View(_empresaContext.GetAll());
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
                empresa = RemoveCaracteresEspeciais(empresa);
                if (_empresaContext.GetByCnpj(empresa.Cnpj) == null)
                {
                    try
                    {
                        _empresaContext.Insert(empresa);
                    }
                    catch
                    {
                        TempData["MensagemSucesso"] = "Algo deu errado, tente novamente.";
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
                    try
                    {
                        _empresaContext.Update(empresa);
                    }
                    catch
                    {
                        TempData["MensagemSucesso"] = "Algo deu errado, tente novamente.";
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
                foreach (var item in exames)
                {
                    item.IdEmpresaSaude = null;
                    _exameContext.Update(item);
                }

                foreach (var item in pessoaEstado)
                {
                    item.IdEmpresaExame = null;
                    _trabalhaEstadoContext.Update(item);
                }

                _empresaContext.Delete(id);

                TempData["mensagemSucesso"] = "Laboratório removido com sucesso!";
            }
            catch
            {
                TempData["mensagemErro"] = "Houve um problema ao remover laboratório, tente novamente!";
            }

            return RedirectToAction(nameof(Index));
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