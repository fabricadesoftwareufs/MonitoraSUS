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
			UsuarioViewModel usuarioAutenticado = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);
			bool ehAdmin = usuarioAutenticado.RoleUsuario.Equals("ADM");
			bool ehGestor = usuarioAutenticado.RoleUsuario.Equals("GESTOR");
			bool ehSecretario = usuarioAutenticado.RoleUsuario.Equals("SECRETARIO");
			bool ehListarGestores = (ehResponsavel == 1);

			var solicitantes = new List<SolicitanteAprovacaoViewModel>();
			var autenticadoTrabalhaEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(usuarioAutenticado.UsuarioModel.IdPessoa);
			var autenticadoTrabalhaMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(usuarioAutenticado.UsuarioModel.IdPessoa);
			var estadoAutenticado = _estadoService.GetById(autenticadoTrabalhaEstado.IdEstado);
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
			}
			if (TempData["responseOp"] != null)
				ViewBag.responseOp = TempData["responseOp"];

			Tuple<List<SolicitanteAprovacaoViewModel>, List<EmpresaExameModel>> tupleModel = null;

			ViewBag.entidade = (ehResponsavel == 0) ? "Agente" : "Gestor";
			List<EmpresaExameModel> empresas = null;
			if (autenticadoTrabalhaEstado != null && autenticadoTrabalhaEstado.IdEmpresaExame != EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
				empresas = new List<EmpresaExameModel>() { _empresaExameService.GetById(autenticadoTrabalhaEstado.IdEmpresaExame) };
			else
				empresas = _empresaExameService.ListByUF(estadoAutenticado.Uf);

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
			if (agenteEstado != null)
			{

				_pessoaTrabalhaEstadoService.Delete(agenteEstado.IdPessoa);

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
				_pessoaTrabalhaMunicipioService.Delete(agenteMunicipio.IdPessoa);

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

		// GET: AgenteSecretario/Activate/{agente|gestor}/id/idEmpresa
		[Authorize(Roles = "SECRETARIO, GESTOR, ADM")]
		[HttpGet("[controller]/[action]/{ativarPerfil}/{cpf}/{idEmpresa}")]
		public async Task<ActionResult> Activate(string ativarPerfil, string cpf, int idEmpresa)
		{
			//string responseOp = "";
			UsuarioViewModel usuarioAutenticado = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);
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
					pessoaEstado.EhSecretario = (idEmpresa == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)?true:false;
					pessoaEstado.EhResponsavel = true;
					pessoaEstado.IdEmpresaExame = idEmpresa;
					_pessoaTrabalhaEstadoService.Insert(pessoaEstado);
				}
				else
				{
					var pessoaMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(idPessoa);
					_pessoaTrabalhaMunicipioService.Delete(pessoaMunicipio.IdPessoa);
					pessoaMunicipio.EhResponsavel = true;
					pessoaMunicipio.SituacaoCadastro = "A";
					pessoaMunicipio.EhSecretario = true;
					_pessoaTrabalhaMunicipioService.Insert(pessoaMunicipio);
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
			if (ativarPerfil.Equals("gestor") && ehAdmin)
				tipoUsuario = UsuarioModel.PERFIL_SECRETARIO;

			string resposta = "";
			if (usuarioModel == null)
			{
				var pessoa = _pessoaService.GetById(idPessoa);
				var usuario = new UsuarioModel
				{
					IdPessoa = pessoa.Idpessoa,
					Cpf = pessoa.Cpf,
					Email = pessoa.Email,
					Senha = Methods.GenerateToken(),
					TipoUsuario = tipoUsuario
				};
				_usuarioService.Insert(usuario);
				(bool nCpf, bool nUsuario, bool nToken) = await new
									  LoginController(_usuarioService, _pessoaService, _emailService, _recuperarSenhaService)
									  .GenerateToken(usuarioModel.Cpf, 1);
				resposta = ReturnMsgOper(nCpf, nUsuario, nToken);
			}
			else
			{
				_usuarioService.Update(usuarioModel);
				(bool nCpf, bool nUsuario, bool nToken) = await new
									  LoginController(_usuarioService, _pessoaService, _emailService, _recuperarSenhaService)
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
