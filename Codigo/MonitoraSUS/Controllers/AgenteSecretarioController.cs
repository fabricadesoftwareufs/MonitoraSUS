using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model;
using Model.ViewModel;
using Util;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Service;

namespace MonitoraSUS.Controllers
{
    [Authorize(Roles = "AGENTE, SECRETARIO, GESTOR, ADM")]
    public class AgenteSecretarioController : Controller
    {
        private readonly IMunicipioService _municipioService;
        private readonly IEstadoService _estadoService;
        private readonly IPessoaService _pessoaService;
        private readonly IPessoaTrabalhaEstadoService _pessoaTrabalhaEstadoService;
        private readonly IPessoaTrabalhaMunicipioService _pessoaTrabalhaMunicipioService;
        private readonly IUsuarioService _usuarioService;
        private readonly IConfiguration _configuration;
        private readonly IRecuperarSenhaService _recuperarSenhaService;
        private readonly IEmailService _emailService;
        private readonly IExameService _exameService;
        private readonly IEmpresaExameService _empresaExameService;

        public AgenteSecretarioController(IMunicipioService municipioService, IEstadoService estadoService,
            IPessoaService pessoaService, IPessoaTrabalhaMunicipioService pessoaTrabalhaMunicipioService,
            IPessoaTrabalhaEstadoService pessoaTrabalhaEstadoService, IUsuarioService usuarioService, IConfiguration configuration,
            IRecuperarSenhaService recuperarSenhaService, IEmailService emailService, IExameService exameService, IEmpresaExameService empresaExameService)
        {
            _municipioService = municipioService;
            _estadoService = estadoService;
            _pessoaService = pessoaService;
            _pessoaTrabalhaEstadoService = pessoaTrabalhaEstadoService;
            _pessoaTrabalhaMunicipioService = pessoaTrabalhaMunicipioService;
            _usuarioService = usuarioService;
            _configuration = configuration;
            _recuperarSenhaService = recuperarSenhaService;
            _emailService = emailService;
            _exameService = exameService;
            _empresaExameService = empresaExameService;
        }

        // GET: AgenteSecretario/Create
        [AllowAnonymous]
        public ActionResult Create(int userType)
        {
            ViewBag.userType = userType;
            ViewBag.googleKey = _configuration["GOOGLE_KEY"];
            ViewBag.recaptchaSiteKey = _configuration["GOOGLE_RECAPTCHA_SITE_KEY"];
            return View();
        }

        // POST: AgenteSecretario/Create
        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public async Task<ActionResult> CreateAgente(PessoaViewModel pessoaViewModel)
        {
            var captchaValue = await Methods.ValidateCaptcha(pessoaViewModel.RecaptchaResponse, _configuration["GOOGLE_RECAPTCHA_SECRET_KEY"]);
            if (captchaValue > 0.6)
            {
                if (!ModelState.IsValid)
                    return View("Create", pessoaViewModel);
                try
                {
                    _pessoaService.InsertAgente(pessoaViewModel);
                }
                catch (ServiceException se)
                {
                    TempData["mensagemErro"] = se.Message;
                    
                }
                TempData["mensagemSucesso"] = "Solicitação de cadastro realizado com sucesso! Por favor, aguarde e-mail " +
                        "que será enviado pelo MonitoraSUS assim que seu acesso ao sistema for autorizado.";
                return RedirectToAction("Create", "AgenteSecretario");
            }
            return RedirectToAction("Index", "Login");
        }

        // POST: AgenteSecretario/Create
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateGestor(PessoaViewModel pessoaViewModel)
        {
            var captchaValue = await Methods.ValidateCaptcha(pessoaViewModel.RecaptchaResponse, _configuration["GOOGLE_RECAPTCHA_SECRET_KEY"]);
            if (captchaValue > 0.6)
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.userType = 1;
                    return View("Create", pessoaViewModel);
                }
                try
                {
                    _pessoaService.InsertGestor(pessoaViewModel);
                }
                catch (ServiceException se)
                {
                    TempData["mensagemErro"] = se.Message;
                }
                TempData["mensagemSucesso"] = "Solicitação de cadastro realizado com sucesso! Por favor, aguarde e-mail " +
                                    "que será enviado pelo MonitoraSUS assim que seu acesso ao sistema for autorizado por um gestor de saúde municipal. " +
                                    "Se você for secretário de saúde ou ainda não há gestores cadastrados no município, por favor, envie documentação comprobatória " +
                                    " para fabricadesoftware@ufs.br para liberarmos o primeiro acesso para seu município.";
                return RedirectToAction("Create", "AgenteSecretario");
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost, AllowAnonymous]
        public IActionResult ReturnCities(string UF)
        {
            var listOfCities = _municipioService.GetByUFCode(UF);
            if (listOfCities.Count != 0)
                return Ok(listOfCities);
            else
                return NoContent();
        }

        [AllowAnonymous]
        public ActionResult ReturnStates() => Ok(_estadoService.GetAll());


        //========================= AGENTE, GESTOR CONTROL ACESS ====================
        /// <summary>
        /// Se for secretario poderá administrar o status do agente/gestor no sistema, A, I, S. 
        /// Gerenciar autorização do agente/gestor de saúde
        /// </summary>
        /// <param name="ehResponsavel">Se for 0 é agente e se for 1 é gestor</param>
        /// <returns></returns>
        [Authorize(Roles = "SECRETARIO, GESTOR, ADM")]
        // GET: AgenteSecretario/IndexApproveAgent/ehResponsavel
        [HttpGet("[controller]/[action]/{ehResponsavel}")]
        public ActionResult IndexApproveAgent(int ehResponsavel)
        {
            // usuario logado
            UsuarioViewModel usuarioAutenticado = _usuarioService.RetornLoggedUser((ClaimsIdentity)User.Identity);
            bool ehAdmin = usuarioAutenticado.RoleUsuario.Equals("ADM");
            bool ehGestor = usuarioAutenticado.RoleUsuario.Equals("GESTOR");
            bool ehSecretario = usuarioAutenticado.RoleUsuario.Equals("SECRETARIO");
            bool ehListarGestores = (ehResponsavel == 1);

            var solicitantes = new List<SolicitanteAprovacaoViewModel>();
            var autenticadoTrabalhaEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(usuarioAutenticado.UsuarioModel.IdPessoa);
            var autenticadoTrabalhaMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(usuarioAutenticado.UsuarioModel.IdPessoa);
            if (autenticadoTrabalhaEstado != null || ehAdmin)
            {
                var ehEmpresa = autenticadoTrabalhaEstado.IdEmpresaExame != EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO;

                if (ehAdmin)
                    solicitantes = _pessoaTrabalhaEstadoService.GetAllGestores();
                else if (ehEmpresa)
                {
                    if (ehSecretario && ehListarGestores)
                    {
                        solicitantes = _pessoaTrabalhaEstadoService.GetAllGestoresEmpresa(autenticadoTrabalhaEstado.IdEmpresaExame);
                    }
                    else if (!ehListarGestores)
                    {
                        solicitantes = _pessoaTrabalhaEstadoService.GetAllNotificadoresEmpresa(autenticadoTrabalhaEstado.IdEmpresaExame);
                    }

                }
                else
                {
                    if (ehSecretario && ehListarGestores)
                    {
                        solicitantes = _pessoaTrabalhaEstadoService.GetAllGestoresEstado(autenticadoTrabalhaEstado.IdEstado);
                    }
                    else if (!ehListarGestores)
                    {
                        solicitantes = _pessoaTrabalhaEstadoService.GetAllNotificadoresEstado(autenticadoTrabalhaEstado.IdEstado);
                    }
                }
            }
            if (autenticadoTrabalhaMunicipio != null || ehAdmin)
            {
                if (ehAdmin)
                    solicitantes = solicitantes.Concat(_pessoaTrabalhaMunicipioService.GetAllGestores()).ToList();
                else
                {
                    if (ehSecretario && ehListarGestores)
                        solicitantes = _pessoaTrabalhaMunicipioService.GetAllGestoresMunicipio(autenticadoTrabalhaMunicipio.IdMunicipio);
                    else if (!ehListarGestores)
                        solicitantes = _pessoaTrabalhaMunicipioService.GetAllNotificadoresMunicipio(autenticadoTrabalhaMunicipio.IdMunicipio);
                }
                foreach (SolicitanteAprovacaoViewModel solicitante in solicitantes)
                {
                    if (solicitante.Estado.All(char.IsDigit))
                    {
                        solicitante.Estado = _estadoService.GetById(Convert.ToInt32(solicitante.Estado)).Uf;
                    }
                }
            }
            if (TempData["responseOp"] != null)
                ViewBag.responseOp = TempData["responseOp"];

            Tuple<List<SolicitanteAprovacaoViewModel>, List<EmpresaExameModel>> tupleModel = null;

            ViewBag.entidade = (ehResponsavel == 0) ? "Agente" : "Gestor";
            List<EmpresaExameModel> empresas = null;
            if (ehAdmin)
                empresas = _empresaExameService.ListAll();
            else if (autenticadoTrabalhaEstado != null && autenticadoTrabalhaEstado.IdEmpresaExame != EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
                empresas = new List<EmpresaExameModel>() { _empresaExameService.GetById(autenticadoTrabalhaEstado.IdEmpresaExame) };
            else if (autenticadoTrabalhaEstado != null && autenticadoTrabalhaEstado.IdEmpresaExame == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
                empresas = _empresaExameService.ListByUF(_estadoService.GetById(autenticadoTrabalhaEstado.IdEstado).Uf);
            else if (autenticadoTrabalhaMunicipio != null)
                empresas = _empresaExameService.ListByUF(_estadoService.GetById(Convert.ToInt32(_municipioService.GetById(autenticadoTrabalhaMunicipio.IdMunicipio).Uf)).Uf);
            solicitantes = solicitantes.OrderBy(s => s.Nome).ToList();
            if (empresas != null)
                tupleModel = new Tuple<List<SolicitanteAprovacaoViewModel>, List<EmpresaExameModel>>(solicitantes, empresas);
            else
                tupleModel = new Tuple<List<SolicitanteAprovacaoViewModel>, List<EmpresaExameModel>>(solicitantes, null);
            return View(tupleModel);
        }

        // GET: AgenteSecretario/ExcludeAgent/{agente|gestor}/id
        [HttpGet("[controller]/[action]/{entidade}/{idPessoa}")]
        public ActionResult Delete(string entidade, int idPessoa)
        {
            var agenteEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(idPessoa);
            var agenteMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(idPessoa);
            if (agenteEstado != null)
                _pessoaTrabalhaEstadoService.Delete(idPessoa);
            if (agenteMunicipio != null)
                _pessoaTrabalhaMunicipioService.Delete(idPessoa);

            var exames = _exameService.GetByIdPaciente(idPessoa);

            if (exames.Count() == 0)
            {
                var examesRealizados = _exameService.GetByIdAgente(idPessoa, DateTime.MinValue, DateTime.MaxValue);
                var usuario = _usuarioService.GetByIdPessoa(idPessoa);
                if (usuario != null && examesRealizados.Count() == 0)
                {
                    _recuperarSenhaService.DeleteByUser(usuario.IdUsuario);
                    _usuarioService.Delete(usuario.IdUsuario);
                }
                _pessoaService.Delete(idPessoa);
            }
            else
            {
                var usuario = _usuarioService.GetByIdPessoa(idPessoa);
                usuario.TipoUsuario = 0;
                _usuarioService.Update(usuario);
            }

            int responsavel;
            if (entidade.Equals("Agente"))
                responsavel = 0;
            else
                responsavel = 1;

            TempData["responseOp"] = entidade + " excluído com sucesso!";

            return RedirectToAction(nameof(IndexApproveAgent), new { ehResponsavel = responsavel });
        }

        // GET: AgenteSecretario/Activate/{agente|gestor}/id/idEmpresa
        [Authorize(Roles = "SECRETARIO, GESTOR, ADM")]
        [HttpGet("[controller]/[action]/{ativarPerfil}/{cpf}/{idEmpresa}")]
        public async Task<ActionResult> Activate(string ativarPerfil, string cpf, int idEmpresa)
        {
            //string responseOp = "";
            UsuarioViewModel usuarioAutenticado = _usuarioService.RetornLoggedUser((ClaimsIdentity)User.Identity);
            bool ehAdmin = usuarioAutenticado.RoleUsuario.Equals("ADM");
            bool ehGestor = usuarioAutenticado.RoleUsuario.Equals("GESTOR");
            bool ehSecretario = usuarioAutenticado.RoleUsuario.Equals("SECRETARIO");

            //busca associacao do usuario autenticado
            var autenticadoTrabalhaEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(usuarioAutenticado.UsuarioModel.IdPessoa);
            var autenticadoTrabalhaMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(usuarioAutenticado.UsuarioModel.IdPessoa);
            int idPessoa = _pessoaService.GetByCpf(cpf).Idpessoa;
            if (ehAdmin)
            {
                var pessoaEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(idPessoa);
                if (pessoaEstado != null)
                {
                    _pessoaTrabalhaEstadoService.Delete(pessoaEstado.IdPessoa);
                    pessoaEstado.SituacaoCadastro = "A";
                    pessoaEstado.EhSecretario = (idEmpresa == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO) ? true : false;
                    pessoaEstado.EhResponsavel = true;
                    pessoaEstado.IdEmpresaExame = idEmpresa;
                    _pessoaTrabalhaEstadoService.Insert(pessoaEstado);
                }
                else
                {
                    var pessoaMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(idPessoa);
                    if (pessoaMunicipio != null)
                    {
                        _pessoaTrabalhaMunicipioService.Delete(idPessoa);
                        pessoaMunicipio.EhResponsavel = true;
                        pessoaMunicipio.SituacaoCadastro = "A";
                        pessoaMunicipio.EhSecretario = true;
                        _pessoaTrabalhaMunicipioService.Insert(pessoaMunicipio);
                    }
                }
            }
            else if (autenticadoTrabalhaEstado != null)
            {
                //exclui outras associações da pessoa em estado ou municipio
                _pessoaTrabalhaEstadoService.Delete(idPessoa);
                _pessoaTrabalhaMunicipioService.Delete(idPessoa);

                _pessoaTrabalhaEstadoService.Insert(
                new PessoaTrabalhaEstadoModel()
                {
                    EhResponsavel = ativarPerfil.Equals("Agente") ? false : true,
                    IdEmpresaExame = idEmpresa,
                    EhSecretario = false,
                    IdEstado = autenticadoTrabalhaEstado.IdEstado,
                    IdPessoa = idPessoa,
                    SituacaoCadastro = "A"
                });
            }
            else if (autenticadoTrabalhaMunicipio != null)
            {
                //exclui outras associações da pessoa em estado ou municipio
                _pessoaTrabalhaEstadoService.Delete(idPessoa);
                _pessoaTrabalhaMunicipioService.Delete(idPessoa);
                if (idEmpresa == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
                {
                    _pessoaTrabalhaMunicipioService.Insert(
                    new PessoaTrabalhaMunicipioModel()
                    {
                        EhResponsavel = ativarPerfil.Equals("Agente") ? false : true,
                        EhSecretario = false,
                        IdMunicipio = autenticadoTrabalhaMunicipio.IdMunicipio,
                        IdPessoa = idPessoa,
                        SituacaoCadastro = "A"
                    });
                }
                else
                {
                    _pessoaTrabalhaEstadoService.Insert(
                    new PessoaTrabalhaEstadoModel()
                    {
                        EhResponsavel = ativarPerfil.Equals("Agente") ? false : true,
                        IdEmpresaExame = idEmpresa,
                        EhSecretario = false,
                        IdEstado = _municipioService.GetById(autenticadoTrabalhaMunicipio.IdMunicipio).Id,
                        IdPessoa = idPessoa,
                        SituacaoCadastro = "A"
                    });
                }
            }

            UsuarioModel usuarioModel = _usuarioService.GetByIdPessoa(idPessoa);

            int tipoUsuario = ativarPerfil.Equals("Agente") ? UsuarioModel.PERFIL_AGENTE : UsuarioModel.PERFIL_GESTOR;
            if (ehAdmin && idEmpresa == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
                tipoUsuario = UsuarioModel.PERFIL_SECRETARIO;
            else if (ehAdmin && idEmpresa != EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
                tipoUsuario = UsuarioModel.PERFIL_GESTOR;


            string resposta = "";
            if (usuarioModel == null)
            {
                var pessoa = _pessoaService.GetById(idPessoa);
                usuarioModel = new UsuarioModel
                {
                    IdPessoa = pessoa.Idpessoa,
                    Cpf = pessoa.Cpf,
                    Email = pessoa.Email,
                    Senha = Methods.GenerateToken(),
                    TipoUsuario = tipoUsuario
                };
                _usuarioService.Insert(usuarioModel);
                if (tipoUsuario == UsuarioModel.PERFIL_SECRETARIO)
                {
                    (bool nCpf, bool nUsuario, bool nToken) = await new
                                          LoginController(
                                            _usuarioService,
                                            _pessoaService,
                                            _pessoaTrabalhaEstadoService,
                                            _pessoaTrabalhaMunicipioService,
                                            _estadoService,
                                            _municipioService,
                                            _empresaExameService,
                                            _emailService,
                                            _recuperarSenhaService)
                                          .GenerateToken(usuarioModel.Cpf, 4);
                    resposta = ReturnMsgOper(nCpf, nUsuario, nToken);
                }
                else
                {
                    (bool nCpf, bool nUsuario, bool nToken) = await new
                                          LoginController(
                                            _usuarioService,
                                            _pessoaService,
                                            _pessoaTrabalhaEstadoService,
                                            _pessoaTrabalhaMunicipioService,
                                            _estadoService,
                                            _municipioService,
                                            _empresaExameService,
                                            _emailService,
                                            _recuperarSenhaService)
                                          .GenerateToken(usuarioModel.Cpf, 1);
                    resposta = ReturnMsgOper(nCpf, nUsuario, nToken);
                }

            }
            else
            {
                _usuarioService.Update(usuarioModel);
                (bool nCpf, bool nUsuario, bool nToken) = await new
                                      LoginController(
                                        _usuarioService,
                                        _pessoaService,
                                        _pessoaTrabalhaEstadoService,
                                        _pessoaTrabalhaMunicipioService,
                                        _estadoService,
                                        _municipioService,
                                        _empresaExameService,
                                        _emailService,
                                        _recuperarSenhaService)
                                      .GenerateToken(usuarioModel.Cpf, 2);
                resposta = ReturnMsgOper(nCpf, nUsuario, nToken);
            }
            if (string.IsNullOrEmpty(resposta))
                resposta = ativarPerfil + " ativado com sucesso! Um email foi enviado para notificá-lo.";
            else
                resposta = ativarPerfil + " ativado com sucesso! " + resposta;
            TempData["responseOp"] = resposta;
            int responsavel = ativarPerfil.Equals("Agente") ? 0 : 1;
            return RedirectToAction(nameof(IndexApproveAgent), new { ehResponsavel = responsavel });
        }

        // GET: AgenteSecretario/BlockAgent/{agente|gestor}/id
        [HttpGet("[controller]/[action]/{entidade}/{idPessoa}")]
        public ActionResult Block(string entidade, int idPessoa)
        {

            var agenteEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(idPessoa);

            var idUsuario = _usuarioService.GetByIdPessoa(idPessoa).IdUsuario;
            if (idUsuario != -1)
                _recuperarSenhaService.SetTokenInvalid(idUsuario);

            if (agenteEstado != null)
            {
                agenteEstado.SituacaoCadastro = "I";
                _pessoaTrabalhaEstadoService.Update(agenteEstado);
            }
            else
            {
                var agenteMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(idPessoa);
                agenteMunicipio.SituacaoCadastro = "I";
                _pessoaTrabalhaMunicipioService.Update(agenteMunicipio);
            }

            int responsavel;
            if (entidade.Equals("Agente"))
                responsavel = 0;
            else
                responsavel = 1;

            TempData["responseOp"] = entidade + " bloqueado com sucesso!";

            return RedirectToAction(nameof(IndexApproveAgent), new { ehResponsavel = responsavel });
        }


        public bool ExistePessoa(string cpf) => (_pessoaService.GetByCpf(Methods.RemoveSpecialsCaracts(cpf))) != null ? true : false;

        private string ReturnMsgOper(bool nCpf, bool nUsuario, bool nToken)
        {
            string responseOp = "";

            if (!nCpf)
                responseOp += "CPF inválido. ";

            else if (!nUsuario)
                responseOp += "Um e-mail já foi enviado para notificá-lo.";

            else if (!nToken)
                responseOp += "Ocorreu um erro com o envio do email, falha na operação. ";

            return responseOp;

        }


    }
}
