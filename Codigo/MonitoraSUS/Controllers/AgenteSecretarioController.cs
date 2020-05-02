using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model;
using Model.ViewModel;
using MonitoraSUS.Utils;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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

        // GET: AgenteSecretario
        public ActionResult Index()
        {
            return View();
        }

        // GET: AgenteSecretario/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AgenteSecretario/Create
        [AllowAnonymous]
        public ActionResult Create(int userType)
        {
            ViewBag.userType = userType;
            ViewBag.googleKey = _configuration["GOOGLE_KEY"];
            return View();
        }

        // POST: AgenteSecretario/Create
        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public ActionResult CreateAgent(IFormCollection collection)
        {
            try
            {
                // INSERTING A USER AND RETURNING THE ID.
                var idPessoaInserida = PeopleInserted(collection);
                // ===================== OTHERS ENTITIES =====================
                var atuacao = collection["areaAtuacao"];
                if (atuacao.Equals("Municipal"))
                {
                    if (idPessoaInserida == 0)
                    {
                        TempData["mensagemErro"] = "Você já possui um cadastro no sistema. Solicite ao "
                             + "gestor de saúde municipal sua inclusão como notificador.";
                        return RedirectToAction("Create", "AgenteSecretario");
                    }
                    if (_pessoaTrabalhaMunicipioService
                            .Insert(new PessoaTrabalhaMunicipioModel
                            {
                                IdPessoa = idPessoaInserida,
                                IdMunicipio = Convert.ToInt32(collection["select-Cidade"]),
                                EhSecretario = false,
                                SituacaoCadastro = EmpresaExameModel.SITUACAO_CADASTRO_SOLICITADA,
                                EhResponsavel = false
                            }))
                    {
                        TempData["mensagemSucesso"] = "Solicitação de cadastro realizado com sucesso! Por favor, aguarde e-mail " +
                            "que será enviado pelo MonitoraSUS assim que seu acesso ao sistema for autorizado pelo gestor de saúde municipal.";
                    }
                    else
                        TempData["mensagemErro"] = "Não foi possível concluir seu cadastro. Por favor, tente novamente.";
                    return RedirectToAction("Create", "AgenteSecretario");
                }

                if (atuacao.Equals("Estadual"))
                {
                    if (idPessoaInserida == 0)
                    {
                        TempData["mensagemErro"] = "Você já possui um cadastro no sistema. Solicite ao "
                             + "gestor de saúde estadual sua inclusão como notificador.";
                        return RedirectToAction("Create", "AgenteSecretario");
                    }
                    if (_pessoaTrabalhaEstadoService
                            .Insert(new PessoaTrabalhaEstadoModel
                            {
                                IdPessoa = idPessoaInserida,
                                IdEstado = Convert.ToInt32(collection["select-Estado"]),
                                EhSecretario = false,
                                EhResponsavel = false,
                                SituacaoCadastro = EmpresaExameModel.SITUACAO_CADASTRO_SOLICITADA,
                                IdEmpresaExame = EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO
                            }))
                    {
                        TempData["mensagemSucesso"] = "Solicitação de cadastro realizado com sucesso! Por favor, aguarde e-mail " +
                            "que será enviado pelo MonitoraSUS assim que seu acesso ao sistema for autorizado pelo gestor de saúde estadual.";
                    }
                    else
                        TempData["mensagemErro"] = "Não foi possível concluir seu cadastro. Por favor, tente novamente.";
                    return RedirectToAction("Create", "AgenteSecretario");
                }

                // Redirecting
                return RedirectToAction("Index", "Login");

            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }

        // POST: AgenteSecretario/Create
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult CreateSec(IFormCollection collection)
        {
            try
            {
                // INSERTING A USER AND RETURNING THE ID.
                var idPessoaInserida = PeopleInserted(collection);

                // ===================== OTHERS ENTITIES =====================
                var atuacao = collection["areaAtuacao"];
                if (atuacao.Equals("Municipal"))
                {

                    if (idPessoaInserida == 0)
                    {
                        TempData["mensagemErro"] = "Você já possui um cadastro no sistema. Solicite ao "
                             + "gestor de saúde municipal sua inclusão como gestor.";
                        return RedirectToAction("Create", "AgenteSecretario");
                    }
                    if (_pessoaTrabalhaMunicipioService
                            .Insert(new PessoaTrabalhaMunicipioModel
                            {
                                IdPessoa = idPessoaInserida,
                                IdMunicipio = Convert.ToInt32(collection["select-Cidade"]),
                                EhSecretario = false,
                                SituacaoCadastro = EmpresaExameModel.SITUACAO_CADASTRO_SOLICITADA,
                                EhResponsavel = true
                            }))
                    {
                        TempData["mensagemSucesso"] = "Solicitação de cadastro realizado com sucesso! Por favor, aguarde e-mail " +
                            "que será enviado pelo MonitoraSUS assim que seu acesso ao sistema for autorizado por um gestor de saúde municipal. " +
                            "Se você for secretário de saúde ou ainda não há gestores cadastrados no município, por favor, envie documentação comprobatória " +
                            " para fabricadesoftware@ufs.br para liberarmos o primeiro acesso para seu município."; ;
                    }
                    else
                        TempData["mensagemErro"] = "Não foi possível concluir seu cadastro. Por favor, tente novamente";
                    return RedirectToAction("Create", "AgenteSecretario");
                }

                if (atuacao.Equals("Estadual"))
                {
                    if (idPessoaInserida == 0)
                    {
                        TempData["mensagemErro"] = "Você já possui um cadastro no sistema. Solicite ao "
                             + "gestor de saúde estadual sua inclusão como gestor.";
                        return RedirectToAction("Create", "AgenteSecretario");
                    }
                    if (_pessoaTrabalhaEstadoService
                            .Insert(new PessoaTrabalhaEstadoModel
                            {
                                IdPessoa = idPessoaInserida,
                                IdEstado = Convert.ToInt32(collection["select-Estado"]),
                                EhSecretario = false,
                                EhResponsavel = true,
                                SituacaoCadastro = EmpresaExameModel.SITUACAO_CADASTRO_SOLICITADA,
                                IdEmpresaExame = EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO    //valor padrão
                            }))
                    {
                        TempData["mensagemSucesso"] = "Solicitação de cadastro realizado com sucesso! Por favor, aguarde e-mail " +
                            "que será enviado pelo MonitoraSUS assim que seu acesso ao sistema for autorizado por um gestor de saúde estadual." +
                            " Se você for secretário de saúde ou ainda não há gestores cadastrados no estado, por favor, envie documentação comprobatória " +
                            " para fabricadesoftware@ufs.br para liberarmos o primeiro acesso para seu estado.";
                    }
                    else
                        TempData["mensagemErro"] = "Não foi possível concluir seu cadastro. Por favor, tente novamente.";
                    return RedirectToAction("Create", "AgenteSecretario");
                }
                // Redirecting
                return RedirectToAction("Index", "Login");
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
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
			var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);

			var solicitantes = new List<SolicitanteAprovacaoViewModel>();
			var pessoaTrabalhaEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
			var pessoaTrabalhaMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
			string estadoTrabalha = "";

			if (pessoaTrabalhaEstado != null || usuario.RoleUsuario.Equals("ADM"))
			{
				var agentesEstado = new List<PessoaTrabalhaEstadoModel>();
				var agentesE = new List<AgenteMunicipioEstadoViewModel>();
				if (usuario.RoleUsuario.Equals("ADM"))
					agentesEstado = _pessoaTrabalhaEstadoService.GetAllGestoresEstado();
				else
				{

					estadoTrabalha = _estadoService.GetById(pessoaTrabalhaEstado.IdEstado).Nome;

					if (ehResponsavel == 0)
						agentesEstado = _pessoaTrabalhaEstadoService.GetAllAgentsEstado(pessoaTrabalhaEstado.IdEstado);
					else
						agentesEstado = _pessoaTrabalhaEstadoService.GetAllGestoresEstado(pessoaTrabalhaEstado.IdEstado);
				}
				agentesEstado.ForEach(item => agentesE.Add(new AgenteMunicipioEstadoViewModel { Pessoa = _pessoaService.GetById(item.IdPessoa), PessoaEstado = item, Situacao = item.SituacaoCadastro }));

				agentesE.ForEach(item => solicitantes.Add(new SolicitanteAprovacaoViewModel
				{
					IdPessoa = item.Pessoa.Idpessoa,
					Nome = item.Pessoa.Nome,
					Cpf = Methods.PatternCpf(item.Pessoa.Cpf),
					Estado = string.IsNullOrEmpty(estadoTrabalha) ? estadoTrabalha : _estadoService.GetById(item.PessoaEstado.IdEstado).Nome,
					Cidade = null,
					Status = item.Situacao,
					Situacao = ReturnStatus(item.Situacao)
				}));

			}
			if (pessoaTrabalhaMunicipio != null || usuario.RoleUsuario.Equals("ADM"))
			{
				string cidadeTrabalha = "";
				var agentesMunicipio = new List<PessoaTrabalhaMunicipioModel>();

				//var pessoaTrabalhaMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
				if (usuario.RoleUsuario.Equals("ADM"))
					agentesMunicipio = _pessoaTrabalhaMunicipioService.GetAllGestoresMunicipio();
				else
				{
					estadoTrabalha = _estadoService.GetByCodUf(Int32.Parse(_municipioService.GetById(pessoaTrabalhaMunicipio.IdMunicipio).Uf)).Nome;
					cidadeTrabalha = _municipioService.GetById(pessoaTrabalhaMunicipio.IdMunicipio).Nome;
					if (ehResponsavel == 0)
						agentesMunicipio = _pessoaTrabalhaMunicipioService.GetAllAgentsMunicipio(pessoaTrabalhaMunicipio.IdMunicipio);
					else
						agentesMunicipio = _pessoaTrabalhaMunicipioService.GetAllGestoresMunicipio(pessoaTrabalhaMunicipio.IdMunicipio);
				}
				var agentesM = new List<AgenteMunicipioEstadoViewModel>();
				agentesMunicipio.ForEach(item => agentesM.Add(new AgenteMunicipioEstadoViewModel { Pessoa = _pessoaService.GetById(item.IdPessoa), PessoaMunicipio = item, Situacao = item.SituacaoCadastro }));

				foreach (AgenteMunicipioEstadoViewModel agente in agentesM)
				{
					if (string.IsNullOrEmpty(estadoTrabalha))
					{
						var municipio = _municipioService.GetById(agente.PessoaMunicipio.IdMunicipio);
						estadoTrabalha = _estadoService.GetByCodUf(int.Parse(municipio.Uf)).Nome;
					}
					var solicitante = new SolicitanteAprovacaoViewModel
					{
						IdPessoa = agente.Pessoa.Idpessoa,
						Nome = agente.Pessoa.Nome,
						Cpf = agente.Pessoa.Cpf,
						Estado = estadoTrabalha,
						Cidade = cidadeTrabalha,
						Status = agente.Situacao,
						Situacao = ReturnStatus(agente.Situacao)

					};
					solicitantes.Add(solicitante);
				}
			}

			if (TempData["responseUp"] != null)
				ViewBag.responseUp = TempData["responseUp"];

			if (TempData["responseOp"] != null)
				ViewBag.responseOp = TempData["responseOp"];

			Tuple<List<SolicitanteAprovacaoViewModel>, List<EmpresaExameModel>> tupleModel = null;

			ViewBag.entidade = (ehResponsavel == 0) ? "Agente" : "Gestor";
			var estado = _estadoService.GetByName(estadoTrabalha);
			List<EmpresaExameModel> empresas = null;
			if (usuario.RoleUsuario.Equals("ADM")) {
				empresas = _empresaExameService.GetAll();
			}
			else {
				if (estado != null)
					empresas = _empresaExameService.GetByUF(estado.Uf);
			}
			if (empresas != null)
				tupleModel = new Tuple<List<SolicitanteAprovacaoViewModel>, List<EmpresaExameModel>>(solicitantes, empresas);
			else
				tupleModel = new Tuple<List<SolicitanteAprovacaoViewModel>, List<EmpresaExameModel>>(solicitantes, null);
			return View(tupleModel);
        }

        // GET: AgenteSecretario/ExcludeAgent/{agente|gestor}/id
        [HttpGet("[controller]/[action]/{entidade}/{idPessoa}")]
        public ActionResult ExcludeAgent(string entidade, int idPessoa)
        {
            var agenteEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(idPessoa);
            if (agenteEstado != null)
            {

                _pessoaTrabalhaEstadoService.Delete(agenteEstado.IdPessoa, agenteEstado.IdEstado, agenteEstado.IdEmpresaExame);

                var exames = _exameService.GetByIdPaciente(agenteEstado.IdPessoa);
                if (exames == null)
                {
                    _pessoaService.Delete(agenteEstado.IdPessoa);
                    int idUsuario = _usuarioService.GetByIdPessoa(agenteEstado.IdPessoa).IdUsuario;
                    _recuperarSenhaService.DeleteByUser(idUsuario);
                    _usuarioService.Delete(idUsuario);
                }
                else
                {
                    var usuario = _usuarioService.GetByIdPessoa(agenteEstado.IdPessoa);
                    usuario.TipoUsuario = 0;
                    _usuarioService.Update(usuario);
                }
            }
            else
            {
                var agenteMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(idPessoa);
                _pessoaTrabalhaMunicipioService.Delete(agenteMunicipio.IdPessoa, agenteMunicipio.IdMunicipio);

                var exames = _exameService.GetByIdPaciente(agenteMunicipio.IdPessoa);
                if (exames == null)
                {
                    _pessoaService.Delete(agenteMunicipio.IdPessoa);
                    int idUsuario = _usuarioService.GetByIdPessoa(agenteMunicipio.IdPessoa).IdUsuario;
                    _recuperarSenhaService.DeleteByUser(idUsuario);
                    _usuarioService.Delete(idUsuario);
                }
                else
                {
                    var usuario = _usuarioService.GetByIdPessoa(agenteMunicipio.IdPessoa);
                    usuario.TipoUsuario = 0;
                    _usuarioService.Update(usuario);
                }

            }

            int responsavel;
            if (entidade.Equals("Agente"))
                responsavel = 0;
            else
                responsavel = 1;

            TempData["responseOp"] = entidade + " excluído com sucesso!";

            return RedirectToAction(nameof(IndexApproveAgent), new { ehResponsavel = responsavel });
        }

        // GET: AgenteSecretario/ActivateAgent/{agente|gestor}/id
        [HttpGet("[controller]/[action]/{entidade}/{idPessoa}")]
        public async Task<ActionResult> ActivateAgent(string entidade, int idPessoa)
        {
            bool sucess = false;
            string responseOp = "";
            UsuarioModel usuarioModel = null;
            bool ehPerfilAdministrador = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity).RoleUsuario == "ADM";

            //caso o sujeito trabalhe no estado
            var agenteEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(idPessoa);
            if (agenteEstado != null)
            {
                //se o ator tiver o cadstro solicitado, será gerado um novo usuario pra ele 
                if (agenteEstado.SituacaoCadastro.Equals("S"))
                {
                    var pessoa = _pessoaService.GetById(agenteEstado.IdPessoa);

                    int tipoUsuario = entidade.Equals("Agente") ? UsuarioModel.PERFIL_AGENTE : UsuarioModel.PERFIL_COORDENADOR;

                    if (entidade.Equals("gestor") && ehPerfilAdministrador)
                        tipoUsuario = UsuarioModel.PERFIL_SECRETARIO;

                    var usuario = new UsuarioModel
                    {
                        IdPessoa = pessoa.Idpessoa,
                        Cpf = pessoa.Cpf,
                        Email = pessoa.Email,
                        Senha = Methods.GenerateToken(),
                        TipoUsuario = tipoUsuario
                    };
                    if (_usuarioService.GetByCpf(pessoa.Cpf) == null)
                        _usuarioService.Insert(usuario);

                    usuarioModel = usuario;

                    (bool nCpf, bool nUsuario, bool nToken) = await new
                        LoginController(_usuarioService, _pessoaService, _emailService, _recuperarSenhaService)
                        .GenerateToken(usuario.Cpf, 1);

                    responseOp = ReturnMsgOper(nCpf, nUsuario, nToken);

                    if (responseOp.Equals(""))
                        sucess = true;

                }
                else
                {
                    usuarioModel = _usuarioService.GetByIdPessoa(agenteEstado.IdPessoa);

                    (bool nCpf, bool nUsuario, bool nToken) = await new
                                          LoginController(_usuarioService, _pessoaService, _emailService, _recuperarSenhaService)
                                          .GenerateToken(usuarioModel.Cpf, 2);
                    responseOp = ReturnMsgOper(nCpf, nUsuario, nToken);

                    if (responseOp.Equals(""))
                        sucess = true;

                }

                if (sucess)
                {
                    // o administrador libera gestores sempre com perfil de secretario
					if (agenteEstado.EhSecretario == false)
						agenteEstado.EhSecretario = (ehPerfilAdministrador) ? true : false;
                    agenteEstado.SituacaoCadastro = "A";
                    _pessoaTrabalhaEstadoService.Update(agenteEstado);
                    responseOp += entidade + " foi ativado com sucesso. Um email foi enviado para notificá-lo!";
                }

                if (agenteEstado.SituacaoCadastro.Equals("S"))
                {
                    //  _recuperarSenhaService.DeleteByUser(usuarioModel.IdUsuario);
                    _usuarioService.Delete(usuarioModel.IdUsuario);
                }
            }

            // caso o sujeito trabalhe no municipio
            else
            {
                var agenteMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(idPessoa);

                //se o ator tiver o cadastro solicitado, será gerado um novo usuario pra ele 
                if (agenteMunicipio.SituacaoCadastro.Equals("S"))
                {
                    var pessoa = _pessoaService.GetById(agenteMunicipio.IdPessoa);

                    int tipoUsuario = entidade.Equals("Agente") ? UsuarioModel.PERFIL_AGENTE : UsuarioModel.PERFIL_COORDENADOR;

                    if (entidade.Equals("gestor") && ehPerfilAdministrador)
                        tipoUsuario = UsuarioModel.PERFIL_SECRETARIO;

                    var usuario = new UsuarioModel
                    {
                        IdPessoa = pessoa.Idpessoa,
                        Cpf = pessoa.Cpf,
                        Email = pessoa.Email,
                        Senha = Methods.GenerateToken(),
                        TipoUsuario = tipoUsuario
                    };
                    if (_usuarioService.GetByCpf(pessoa.Cpf) == null)
                        _usuarioService.Insert(usuario);

                    usuarioModel = usuario;

                    (bool nCpf, bool nUsuario, bool nToken) = await new
                                          LoginController(_usuarioService, _pessoaService, _emailService, _recuperarSenhaService)
                                          .GenerateToken(usuarioModel.Cpf, 1);

                    responseOp = ReturnMsgOper(nCpf, nUsuario, nToken);

                    if (responseOp.Equals(""))
                        sucess = true;
                }
                else
                {
                    usuarioModel = _usuarioService.GetByIdPessoa(agenteMunicipio.IdPessoa);

                    (bool nCpf, bool nUsuario, bool nToken) = await new
                        LoginController(_usuarioService, _pessoaService, _emailService, _recuperarSenhaService).
                        GenerateToken(usuarioModel.Cpf, 2);

                    responseOp = ReturnMsgOper(nCpf, nUsuario, nToken);

                    if (responseOp.Equals(""))
                        sucess = true;
                }

                if (sucess)
                {
                    // o administrador libera gestores sempre com perfil de secretario
                    agenteMunicipio.EhSecretario = (ehPerfilAdministrador) ? true : false;
                    agenteMunicipio.SituacaoCadastro = "A";
                    _pessoaTrabalhaMunicipioService.Update(agenteMunicipio);
                    responseOp += entidade + " foi ativado com sucesso! Um e-mail foi enviado para notificá-lo.";
                }
                if (agenteMunicipio.SituacaoCadastro.Equals("S"))
                {
                    //  _recuperarSenhaService.DeleteByUser(usuarioModel.IdUsuario);
                    _usuarioService.Delete(usuarioModel.IdUsuario);
                }

            }

            int responsavel;
            if (entidade.Equals("Agente"))
                responsavel = 0;

            else
                responsavel = 1;

            TempData["responseOp"] = responseOp;

            return RedirectToAction(nameof(IndexApproveAgent), new { ehResponsavel = responsavel });
        }

        // GET: AgenteSecretario/BlockAgent/{agente|gestor}/id
        [HttpGet("[controller]/[action]/{entidade}/{idPessoa}")]
        public ActionResult BlockAgent(string entidade, int idPessoa)
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

        // GET: AgenteSecretario/DownToAgent/{agente|gestor}/id
        [HttpGet("[controller]/[action]/{idPessoa}")]
        public ActionResult DownToAgent(int idPessoa)
        {
            var agenteEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(idPessoa);
            if (agenteEstado != null)
            {
                agenteEstado.EhResponsavel = false;
                agenteEstado.SituacaoCadastro = "I";
                _pessoaTrabalhaEstadoService.Update(agenteEstado);
            }
            else
            {
                var agenteMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(idPessoa);
                agenteEstado.EhResponsavel = false;
                agenteMunicipio.SituacaoCadastro = "I";
                _pessoaTrabalhaMunicipioService.Update(agenteMunicipio);
            }

            int responsavel = 1;

            TempData["responseOp"] = "Gestor agora possui permissões de notificador e ficou com acesso bloqueado. Autorize o acesso, caso necessário.";

            return RedirectToAction(nameof(IndexApproveAgent), new { ehResponsavel = responsavel });
        }

        /// <summary>
        /// Promove agente à gestor
        /// </summary>
        /// <param name="cpf">cpf do agente que será promovido</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpToGestor(string cpf)
        {
            // usuario logado
            var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);

            var pessoa = _pessoaService.GetByCpf(Methods.RemoveSpecialsCaracts(cpf));

            if (pessoa != null)
            {
                var pessoaTrabalhaEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
                var pessoaTrabalhaMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);

                var agenteEstado = _pessoaTrabalhaEstadoService.GetAgentEstadoByIdPessoa(pessoa.Idpessoa, pessoaTrabalhaEstado.IdEstado);

                if (pessoaTrabalhaEstado != null)
                {
                    if (agenteEstado != null)
                    {
                        agenteEstado.EhResponsavel = true;
                        agenteEstado.SituacaoCadastro = "A";
                        _pessoaTrabalhaEstadoService.Update(agenteEstado);
                        TempData["responseOp"] = "Gestor ativado com sucesso!";
                    }
                    else
                    {
                        var pessoaTrabalhaEstadoModel = new PessoaTrabalhaEstadoModel
                        {
                            IdPessoa = pessoa.Idpessoa,
                            IdEstado = pessoaTrabalhaEstado.IdEstado,
                            EhResponsavel = true,
                            EhSecretario = false,
                            SituacaoCadastro = "I",
                            IdEmpresaExame = 1
                        };

                        _pessoaTrabalhaEstadoService.Insert(pessoaTrabalhaEstadoModel);

                        return RedirectToAction(nameof(ActivateAgent), new { entidade = "Gestor", idPessoa = pessoaTrabalhaEstadoModel.IdPessoa });
                    }
                }
                else
                {
                    var agenteMunicipio = _pessoaTrabalhaMunicipioService.GetAgentMunicipioByIdPessoa(pessoa.Idpessoa, pessoaTrabalhaMunicipio.IdMunicipio);

                    if (agenteMunicipio != null)
                    {
                        agenteEstado.EhResponsavel = true;
                        agenteMunicipio.SituacaoCadastro = "A";

                        _pessoaTrabalhaMunicipioService.Update(agenteMunicipio);

                        TempData["responseOp"] = "Gestor ativado com sucesso!";
                    }
                    else
                    {
                        var pessoaTrabalhaMunicipioModel = new PessoaTrabalhaMunicipioModel
                        {
                            IdPessoa = pessoa.Idpessoa,
                            IdMunicipio = pessoaTrabalhaMunicipio.IdMunicipio,
                            EhResponsavel = true,
                            EhSecretario = false,
                            SituacaoCadastro = "I"
                        };

                        _pessoaTrabalhaMunicipioService.Insert(pessoaTrabalhaMunicipioModel);

                        return RedirectToAction(nameof(ActivateAgent), new { entidade = "Gestor", idPessoa = pessoaTrabalhaMunicipioModel.IdPessoa });
                    }

                }
            }
            else
                TempData["responseUp"] = "Notificador não encontrado.";

            int responsavel = 1;

            return RedirectToAction(nameof(IndexApproveAgent), new { ehResponsavel = responsavel });
        }

        // GET: AgenteSecretario/DownToAgent/{agente|gestor}/id
        [HttpGet("[controller]/[action]/{idEmpresa}/{cpf}")]
        public async Task<ActionResult> AssignAgentToCorp(int idEmpresa, string cpf)
        {
            var pessoa = _pessoaService.GetByCpf(Methods.RemoveSpecialsCaracts(cpf));
            var empresa = _empresaExameService.GetById(idEmpresa);
            var idEstado = _estadoService.GetByName(empresa.Estado).Id;

            if (empresa != null)
            {
                var trabalhaEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(pessoa.Idpessoa);
                if (trabalhaEstado != null)
                {
                    trabalhaEstado.IdEmpresaExame = empresa.Id;
                    _pessoaTrabalhaEstadoService.Update(trabalhaEstado);
                    TempData["responseOp"] = "O notificador foi associado a empresa com sucesso!";
                }
                else
                {
                    var trabalhaMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(pessoa.Idpessoa);
                    if (trabalhaMunicipio != null)
                    {
                        var novoTrabalhaEstado = new PessoaTrabalhaEstadoModel
                        {
                            IdPessoa = trabalhaMunicipio.IdPessoa,
                            IdEstado = idEstado,
                            EhResponsavel = false,
                            EhSecretario = false,
                            SituacaoCadastro = trabalhaMunicipio.SituacaoCadastro,
                            IdEmpresaExame = empresa.Id
                        };

                        if (_pessoaTrabalhaEstadoService.Insert(novoTrabalhaEstado))
                        {
                            if (_pessoaTrabalhaMunicipioService.Delete(trabalhaMunicipio.IdPessoa, trabalhaMunicipio.IdMunicipio))
                                TempData["responseOp"] = "O notificador foi associado a empresa e remanejado para o Estado com sucesso!";
                        }
                        else
                            TempData["responseOp"] = "Falha ao associar o Notificador à empresa.";

                    }
                    else // se for uma pessoa normal
                    {
                        var novoTrabalhaEstado = new PessoaTrabalhaEstadoModel
                        {
                            IdPessoa = pessoa.Idpessoa,
                            IdEstado = idEstado,
                            EhResponsavel = false,
                            EhSecretario = false,
                            SituacaoCadastro = "A",
                            IdEmpresaExame = empresa.Id
                        };

                        if (_pessoaTrabalhaEstadoService.Insert(novoTrabalhaEstado))
                        {
                            var usuario = new UsuarioModel
                            {
                                IdPessoa = pessoa.Idpessoa,
                                Cpf = pessoa.Cpf,
                                Email = pessoa.Email,
                                Senha = Methods.GenerateToken(),
                                TipoUsuario = Methods.ReturnRoleId("Agente")
                            };

                            if (_usuarioService.GetByCpf(pessoa.Cpf) == null)
                                _usuarioService.Insert(usuario);

                            (bool nCpf, bool nUsuario, bool nToken) = await new
                            LoginController(_usuarioService, _pessoaService, _emailService, _recuperarSenhaService).
                                GenerateToken(usuario.Cpf, 2);

                            var responseOp = ReturnMsgOper(nCpf, nUsuario, nToken);

                            if (responseOp.Equals(""))
                                TempData["responseOp"] = "O notificador foi associado a empresa e ativado com sucesso!";

                            else
                            {
                                TempData["responseOp"] = "Falha ao associar o Notificador à empresa.";
                                _recuperarSenhaService.DeleteByUser(usuario.IdUsuario);
                                _usuarioService.Delete(usuario.IdUsuario);
                            }
                        }
                        else
                            TempData["responseOp"] = "Falha ao associar o Notificador à empresa.";
                    }
                }
            }
            else
                TempData["responseOp"] = "Problemas para encontrar os dados da empresa.";

            int responsavel = 0;

            return RedirectToAction(nameof(IndexApproveAgent), new { ehResponsavel = responsavel });
        }

        public bool ExistsAgent(string cpf) => (_pessoaService.GetByCpf(Methods.RemoveSpecialsCaracts(cpf))) != null ? true : false;

        // ======================== PRIVATE METHODS ========================
        private int PeopleInserted(IFormCollection collection)
        {
            // Info Pessoal
            var cpf = Methods.ValidarCpf(collection["Cpf"]) ? Methods.RemoveSpecialsCaracts(collection["Cpf"]) : throw new Exception("Cpf invalido!");
            var nome = collection["Nome"];
            var dataNascimento = collection["DataNascimento"];
            var sexo = collection["sexo"];
            var cell = Methods.RemoveSpecialsCaracts(collection["FoneCelular"]);
            var fixo = Methods.RemoveSpecialsCaracts(collection["FoneFixo"]);
            var email = collection["Email"];

            // Localização
            var cep = Methods.RemoveSpecialsCaracts(collection["Cep"]);
            var rua = collection["Logradouro"];
            var numero = collection["Numero"];
            var bairro = collection["Bairro"];
            var cidade = collection["Localidade"];
            var estado = collection["UF"];
            var complemento = collection["Complemento"];
            var latitude = collection["Latitude"];
            var longitude = collection["Longitude"];

            // Doenças
            var hipertenso = collection["Hipertenso"];
            var diabetes = collection["Diabetes"];
            var obeso = collection["Obeso"];
            var cardiopata = collection["Cardiopatia"];
            var imunoDepri = collection["Imunodeprimido"];
            var cancer = collection["Cancer"];
            var doencaResp = collection["DoencaRespiratoria"];
            var outrasComorbidades = collection["OutrasComorbidades"];

            var pessoa = _pessoaService.GetByCpf(cpf);
            // Se pessoa existe retorna 0 para indicar que a pessoa já tem cadastro
            if (pessoa != null)
                return 0;

            // Inserção e recebendo o objeto inserido (ID)
            pessoa = _pessoaService.Insert(new PessoaModel
            {
                Cpf = cpf,
                Nome = nome,
                DataNascimento = Convert.ToDateTime(dataNascimento),
                Sexo = sexo,
                FoneCelular = cell,
                FoneFixo = fixo,
                Email = email,
                Cep = cep,
                Rua = rua,
                Numero = numero,
                Bairro = bairro,
                Cidade = cidade,
                Estado = estado,
                Complemento = complemento,
                Latitude = latitude,
                Longitude = longitude,
                Hipertenso = hipertenso.Contains("true") ? true : false,
                Cardiopatia = cardiopata.Contains("true") ? true : false,
                Cancer = cancer.Contains("true") ? true : false,
                Diabetes = diabetes.Contains("true") ? true : false,
                DoencaRespiratoria = doencaResp.Contains("true") ? true : false,
                Imunodeprimido = imunoDepri.Contains("true") ? true : false,
                Obeso = obeso.Contains("true") ? true : false,
                OutrasComorbidades = outrasComorbidades
            });

            return pessoa.Idpessoa;
        }

        private string ReturnStatus(string statusAbreviado)
        {
            switch (statusAbreviado)
            {
                case "I": return "Bloqueado";
                case "A": return "Ativo";
                case "S": return "Pendente";
                default: return "Undefined";
            }

        }

        private string ReturnMsgOper(bool nCpf, bool nUsuario, bool nToken)
        {
            string responseOp = "";

            if (!nCpf)
                responseOp += "CPF inválido. ";

            else if (!nUsuario)
                responseOp += "Um e-mail já foi enviado para esse usuário para alteração de senha. ";

            else if (!nToken)
                responseOp += "Ocorreu um erro com o envio do email, falha na operação. ";

            return responseOp;

        }


	}
}
