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

	[Authorize(Roles = "AGENTE, SECRETARIO, ADM")]
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
		public AgenteSecretarioController(IMunicipioService municipioService, IEstadoService estadoService,
			IPessoaService pessoaService, IPessoaTrabalhaMunicipioService pessoaTrabalhaMunicipioService,
			IPessoaTrabalhaEstadoService pessoaTrabalhaEstadoService, IUsuarioService usuarioService, IConfiguration configuration,
			IRecuperarSenhaService recuperarSenhaService, IEmailService emailService)
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
					if (_pessoaTrabalhaMunicipioService
							.Insert(new PessoaTrabalhaMunicipioModel
							{
								IdPessoa = idPessoaInserida,
								IdMunicipio = Convert.ToInt32(collection["select-Cidade"]),
								EhSecretario = false,
								EhResponsavel = false
							}))
					{
						TempData["mensagemSucesso"] = "Solicitação de cadastro realizado com sucesso! Por favor, aguarde e-mail" +
							"que será enviado pelo MonitoraSUS assim que seu acesso ao sistema for autorizado pelo gestor de saúde municipal.";
					}
					else
						TempData["mensagemErro"] = "Não foi possível concluir seu cadastro. Por favor, tente novamente.";
					return RedirectToAction("Create", "AgenteSecretario");
				}

				if (atuacao.Equals("Estadual"))
				{
					if (_pessoaTrabalhaEstadoService
							.Insert(new PessoaTrabalhaEstadoModel
							{
								IdPessoa = idPessoaInserida,
								IdEstado = Convert.ToInt32(collection["select-Estado"]),
								EhSecretario = false,
								EhResponsavel = false
							}))
					{
						TempData["mensagemSucesso"] = "Solicitação de cadastro realizado com sucesso! Por favor, aguarde e-mail" +
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
					if (_pessoaTrabalhaMunicipioService
							.Insert(new PessoaTrabalhaMunicipioModel
							{
								IdPessoa = idPessoaInserida,
								IdMunicipio = Convert.ToInt32(collection["select-Cidade"]),
								EhSecretario = false,
								EhResponsavel = true
							}))
					{
						TempData["mensagemSucesso"] = "Solicitação de cadastro realizado com sucesso! Por favor, aguarde e-mail" +
							"que será enviado pelo MonitoraSUS assim que seu acesso ao sistema for autorizado por um gestor de saúde municipal.";
					}
					else
						TempData["mensagemErro"] = "Não foi possível concluir seu cadastro. Por favor, tente novamente";
					return RedirectToAction("Create", "AgenteSecretario");
				}

				if (atuacao.Equals("Estadual"))
				{
					if (_pessoaTrabalhaEstadoService
							.Insert(new PessoaTrabalhaEstadoModel
							{
								IdPessoa = idPessoaInserida,
								IdEstado = Convert.ToInt32(collection["select-Estado"]),
								EhSecretario = false,
								EhResponsavel = true
							}))
					{
						TempData["mensagemSucesso"] = "Solicitação de cadastro realizado com sucesso! Por favor, aguarde e-mail" +
							"que será enviado pelo MonitoraSUS assim que seu acesso ao sistema for autorizado por um gestor de saúde estadual.";
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

		// GET: AgenteSecretario/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: AgenteSecretario/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, IFormCollection collection)
		{
			try
			{
				// TODO: Add update logic here

				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: AgenteSecretario/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: AgenteSecretario/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, IFormCollection collection)
		{
			try
			{
				// TODO: Add delete logic here

				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		//========================= AGENTE, GESTOR CONTROL ACESS ====================
		/// <summary>
		/// Se for secretario poderá administrar o status do agente/gestor no sistema, A, I, S. 
		/// Gerenciar autorização do agente/gestor de saúde
		/// </summary>
		/// <param name="ehResponsavel">Se for 0 é agente e se for 1 é gestor</param>
		/// <returns></returns>
		[Authorize(Roles = "SECRETARIO, ADM")]
		// GET: AgenteSecretario/IndexApproveAgent/ehResponsavel
		[HttpGet("[controller]/[action]/{ehResponsavel}")]
		public ActionResult IndexApproveAgent(int ehResponsavel)
		{
			// usuario logado
			var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);

			var agentes = new List<AgenteMunicipioEstadoViewModel>();
			var solicitantes = new List<SolicitanteAprovacaoModelView>();
			var pessoaTrabalhaEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
			if (pessoaTrabalhaEstado != null)
			{
				var agentesEstado = new List<PessoaTrabalhaEstadoModel>();
				if (ehResponsavel == 0)
					agentesEstado = _pessoaTrabalhaEstadoService.GetAllAgentsEstado(pessoaTrabalhaEstado.IdEstado);
				else
					agentesEstado = _pessoaTrabalhaEstadoService.GetAllGestoresEstado(pessoaTrabalhaEstado.IdEstado);

				agentesEstado.ForEach(item => agentes.Add(new AgenteMunicipioEstadoViewModel { Pessoa = _pessoaService.GetById(item.IdPessoa), PessoaEstado = item, Situacao = item.SituacaoCadastro }));

				agentes.ForEach(item => solicitantes.Add(new SolicitanteAprovacaoModelView
				{
					IdPessoa = item.Pessoa.Idpessoa,
					Nome = item.Pessoa.Nome,
					Cpf = Methods.PatternCpf(item.Pessoa.Cpf),
					Estado = _estadoService.GetById(item.PessoaEstado.IdEstado).Nome,
					Cidade = null,
					Status = item.Situacao,
					Situacao = ReturnStatus(item.Situacao)
				}));

			}
			else
			{
				var pessoaTrabalhaMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
				var agentesMunicipio = new List<PessoaTrabalhaMunicipioModel>();
				if (ehResponsavel == 0)
					agentesMunicipio = _pessoaTrabalhaMunicipioService.GetAllAgentsMunicipio(pessoaTrabalhaMunicipio.IdMunicipio);
				else
					agentesMunicipio = _pessoaTrabalhaMunicipioService.GetAllGestoresMunicipio(pessoaTrabalhaMunicipio.IdMunicipio);

				agentesMunicipio.ForEach(item => agentes.Add(new AgenteMunicipioEstadoViewModel { Pessoa = _pessoaService.GetById(item.IdPessoa), PessoaMunicipio = item, Situacao = item.SituacaoCadastro }));

				agentes.ForEach(item => solicitantes.Add(new SolicitanteAprovacaoModelView
				{
					IdPessoa = item.Pessoa.Idpessoa,
					Nome = item.Pessoa.Nome,
					Cpf = item.Pessoa.Cpf,
					Estado = _estadoService.GetByUf(_municipioService.GetById(item.PessoaMunicipio.IdMunicipio).Uf).Nome,
					Cidade = _municipioService.GetById(item.PessoaMunicipio.IdMunicipio).Nome,
					Status = item.Situacao,
					Situacao = ReturnStatus(item.Situacao)

				}));
			}

			if (ehResponsavel == 0)
				ViewBag.entidade = "Agente";
			else
				ViewBag.entidade = "Gestor";

			return View(solicitantes);
		}

		// GET: AgenteSecretario/ExcludeAgent/{agente|gestor}/id
		[HttpGet("[controller]/[action]/{entidade}/{idPessoa}")]
		public ActionResult ExcludeAgent(string entidade, int idPessoa)
		{
			var agenteEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(idPessoa);
			if (agenteEstado != null)
			{
				_pessoaTrabalhaEstadoService.Delete(agenteEstado.IdPessoa, agenteEstado.IdEstado);

				_pessoaService.Delete(agenteEstado.IdPessoa);

			}
			else
			{
				var agenteMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(idPessoa);
				_pessoaTrabalhaMunicipioService.Delete(agenteMunicipio.IdPessoa, agenteMunicipio.IdMunicipio);

				_pessoaService.Delete(agenteMunicipio.IdPessoa);
			}

			int responsavel;
			if (entidade.Equals("Agente"))
				responsavel = 0;
			else
				responsavel = 1;

			return RedirectToAction(nameof(IndexApproveAgent), new { ehResponsavel = responsavel });
		}

		// GET: AgenteSecretario/ActivateAgent/{agente|gestor}/id
		[HttpGet("[controller]/[action]/{entidade}/{idPessoa}")]
		public async Task<ActionResult> ActivateAgent(string entidade, int idPessoa)
		{
			bool sucess = true;

			var agenteEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(idPessoa);
			if (agenteEstado != null)
			{
				//se o ator tiver o cadstro solicitado, será gerado um novo usuario pra ele 
				if (agenteEstado.SituacaoCadastro.Equals("S"))
				{
					var pessoa = _pessoaService.GetById(agenteEstado.IdPessoa);

					var usuario = new UsuarioModel
					{
						IdPessoa = pessoa.Idpessoa,
						Cpf = pessoa.Cpf,
						Email = pessoa.Email,
						Senha = Methods.GenerateToken(),
						TipoUsuario = Methods.ReturnRoleId(entidade)
					};
					if (_usuarioService.GetByCpf(pessoa.Cpf) == null)
						_usuarioService.Insert(usuario);

					(bool nCpf, bool nUsuario, bool nToken) =
											await new LoginController(_usuarioService, _pessoaService, _emailService, _recuperarSenhaService).GenerateToken(usuario.Cpf, 1);

					if (!(nCpf && nUsuario && nToken))
					{
						_usuarioService.Delete(usuario.IdUsuario);
						sucess = false;
					}

				}
				if (sucess)
				{
					agenteEstado.SituacaoCadastro = "A";
					_pessoaTrabalhaEstadoService.Update(agenteEstado);
				}
			}
			else
			{
				var agenteMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(idPessoa);

				//se o ator tiver o cadstro solicitado, será gerado um novo usuario pra ele 
				if (agenteMunicipio.SituacaoCadastro.Equals("S"))
				{
					var pessoa = _pessoaService.GetById(agenteMunicipio.IdPessoa);

					var usuario = new UsuarioModel
					{
						IdPessoa = pessoa.Idpessoa,
						Cpf = pessoa.Cpf,
						Email = pessoa.Email,
						Senha = Methods.GenerateToken(),
						TipoUsuario = Methods.ReturnRoleId(entidade)
					};
					if (_usuarioService.GetByCpf(pessoa.Cpf) == null)
						_usuarioService.Insert(usuario);

					(bool nCpf, bool nUsuario, bool nToken) =
						await new LoginController(_usuarioService, _pessoaService, _emailService, _recuperarSenhaService).GenerateToken(usuario.Cpf, 1);

					if (!(nCpf && nUsuario && nToken))
					{
						_usuarioService.Delete(usuario.IdUsuario);
						sucess = false;
					}

				}
				if (sucess)
				{
					agenteMunicipio.SituacaoCadastro = "A";
					_pessoaTrabalhaMunicipioService.Update(agenteMunicipio);
				}
			}

			int responsavel;
			if (entidade.Equals("Agente"))
				responsavel = 0;
			else
				responsavel = 1;

			return RedirectToAction(nameof(IndexApproveAgent), new { ehResponsavel = responsavel });
		}

		// GET: AgenteSecretario/BlockAgent/{agente|gestor}/id
		[HttpGet("[controller]/[action]/{entidade}/{idPessoa}")]
		public ActionResult BlockAgent(string entidade, int idPessoa)
		{

			var agenteEstado = _pessoaTrabalhaEstadoService.GetByIdPessoa(idPessoa);
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

			return RedirectToAction(nameof(IndexApproveAgent), new { ehResponsavel = responsavel });
		}

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

			// Inserção e recebendo o objeto inserido (ID)
			var pessoa = _pessoaService.Insert(new PessoaModel
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
				case "S": return "Pedente";
				default: return "Undefined";
			}

		}
	}
}
