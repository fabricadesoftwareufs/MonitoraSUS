using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Model;
using Model.ViewModel;
using MonitoraSUS.Util;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MonitoraSUS.Controllers
{
	[Authorize(Roles = "AGENTE, GESTOR, SECRETARIO")]
	public class ExameController : Controller
	{
		private readonly IVirusBacteriaService _virusBacteriaContext;
		private readonly IExameService _exameContext;
		private readonly IPessoaService _pessoaContext;
		private readonly IMunicipioService _municicpioContext;
		private readonly IEstadoService _estadoContext;
		private readonly ISituacaoVirusBacteriaService _situacaoPessoaContext;
		private readonly IPessoaTrabalhaEstadoService _pessoaTrabalhaEstadoContext;
		private readonly IPessoaTrabalhaMunicipioService _pessoaTrabalhaMunicipioContext;
		private readonly IAreaAtuacaoService _areaAtuacaoContext;
		private readonly IUsuarioService _usuarioContext;
		private readonly IConfiguration _configuration;

		public ExameController(IVirusBacteriaService virusBacteriaContext,
							   IExameService exameContext,
							   IPessoaService pessoaContext,
							   IMunicipioService municicpioContext,
							   IEstadoService estadoContext,
							   IConfiguration configuration,
							   ISituacaoVirusBacteriaService situacaoPessoaContext,
							   IPessoaTrabalhaEstadoService pessoaTrabalhaEstado,
							   IPessoaTrabalhaMunicipioService pessoaTrabalhaMunicipioContext,
							   IAreaAtuacaoService areaAtuacaoContext,
							   IUsuarioService usuarioContext)
		{
			_virusBacteriaContext = virusBacteriaContext;
			_exameContext = exameContext;
			_pessoaContext = pessoaContext;
			_municicpioContext = municicpioContext;
			_estadoContext = estadoContext;
			_situacaoPessoaContext = situacaoPessoaContext;
			_pessoaTrabalhaEstadoContext = pessoaTrabalhaEstado;
			_pessoaTrabalhaMunicipioContext = pessoaTrabalhaMunicipioContext;
			_configuration = configuration;
			_areaAtuacaoContext = areaAtuacaoContext;
			_usuarioContext = usuarioContext;
		}

		public IActionResult Index(PesquisaExameViewModel pesquisaExame)
		{
			return View(BuscaExamesViewModel(pesquisaExame));
		}


		public IActionResult Notificate(PesquisaExameViewModel pesquisaExame)
		{
			return View(BuscaExamesViewModel(pesquisaExame));
		}

		[Authorize(Roles = "GESTOR, SECRETARIO")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EnviarSMS(int id, PesquisaExameViewModel pesquisaExame, IFormCollection collection)
		{
			ExameViewModel exameView = _exameContext.GetById(id);
			var usuario = _usuarioContext.RetornLoggedUser((ClaimsIdentity)User.Identity);
			var trabalhaMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
			var trabalhaEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);

			ConfiguracaoNotificarModel configuracaoNotificar = null;
			if (trabalhaEstado != null)
			{
				configuracaoNotificar = _exameContext.BuscarConfiguracaoNotificar(trabalhaEstado.IdEstado, trabalhaEstado.IdEmpresaExame);
			}
			else if (trabalhaMunicipio != null)
			{
				configuracaoNotificar = _exameContext.BuscarConfiguracaoNotificar(trabalhaMunicipio.IdMunicipio);
			}
			if (configuracaoNotificar == null)
			{
				TempData["mensagemErro"] = "Não possui créditos para notificações por SMS. Por favor entre em contato pelo email fabricadesoftware@ufs.br para saber como usar esse serviço no MonitoraSUS.";
			}
			else
			{
				if (configuracaoNotificar.QuantidadeSmsdisponivel == 0 && exameView.Exame.StatusNotificacao.Equals(ExameModel.NOTIFICADO_NAO))
				{
					TempData["mensagemErro"] = "Não possui créditos para enviar SMS. " +
						"Por favor entre em contato pelo email fabricadesoftware@ufs.br se precisar novos créditos.";
				}
				else
				{
					//var pacienteModel = _pessoaContext.GetById(exame.Exame.IdPaciente);
					string statusAnteriorSMS = exameView.Exame.StatusNotificacao;
					ExameModel exame = exameView.Exame;
					if (exameView.Paciente.TemFoneCelularValido)
					{
						if (exame.StatusNotificacao.Equals(ExameModel.NOTIFICADO_ENVIADO))
						{
							exame = await _exameContext.ConsultarSMSExameAsync(configuracaoNotificar, exame);
						}
						else if (exame.StatusNotificacao.Equals(ExameModel.NOTIFICADO_NAO) || exame.StatusNotificacao.Equals(ExameModel.NOTIFICADO_PROBLEMAS))
						{
							exame = await _exameContext.EnviarSMSResultadoExameAsync(configuracaoNotificar, exame, exameView.Paciente);
						}
					}
					if (statusAnteriorSMS.Equals(ExameModel.NOTIFICADO_ENVIADO) && exame.StatusNotificacao.Equals(ExameModel.NOTIFICADO_SIM))
					{
						TempData["mensagemSucesso"] = "SMS foi entregue com SUCESSO!";
					}
					else if (statusAnteriorSMS.Equals(ExameModel.NOTIFICADO_NAO) && exame.StatusNotificacao.Equals(ExameModel.NOTIFICADO_ENVIADO))
					{
						TempData["mensagemSucesso"] = "SMS enviado com SUCESSO!";
					}
					else if (statusAnteriorSMS.Equals(ExameModel.NOTIFICADO_NAO) && exame.StatusNotificacao.Equals(ExameModel.NOTIFICADO_NAO))
					{
						TempData["mensagemErro"] = "Ocorreram problemas no envio do SMS. Favor conferir telefone e repetir operação em alguns minutos.";
					}
					else if (statusAnteriorSMS.Equals(ExameModel.NOTIFICADO_ENVIADO) && exame.StatusNotificacao.Equals(ExameModel.NOTIFICADO_ENVIADO))
					{
						TempData["mensagemErro"] = "Ainda aguardando resposta da operadora. Favor repetir a consulta em alguns minutos.";
					}
					else if (statusAnteriorSMS.Equals(ExameModel.NOTIFICADO_ENVIADO) && exame.StatusNotificacao.Equals(ExameModel.NOTIFICADO_PROBLEMAS))
					{
						TempData["mensagemErro"] = "Operadora não conseguiu entregar o SMS. Favor conferir telefone e repetir envio em alguns minutos.";
					}
				}
			}

			return RedirectToAction("Notificate", "Exame", pesquisaExame);
		}

		[Authorize(Roles = "GESTOR, SECRETARIO")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ConsultarSMSEnviados(List<ExameViewModel> exames, PesquisaExameViewModel pesquisaExame)
		{
			var usuario = _usuarioContext.RetornLoggedUser((ClaimsIdentity)User.Identity);
			var trabalhaMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
			var trabalhaEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);

			ConfiguracaoNotificarModel configuracaoNotificar = null;
			if (trabalhaEstado != null)
			{
				configuracaoNotificar = _exameContext.BuscarConfiguracaoNotificar(trabalhaEstado.IdEstado, trabalhaEstado.IdEmpresaExame);
			}
			else if (trabalhaMunicipio != null)
			{
				configuracaoNotificar = _exameContext.BuscarConfiguracaoNotificar(trabalhaMunicipio.IdMunicipio);
			}
			if (configuracaoNotificar == null)
			{
				TempData["mensagemErro"] = "Não possui configuração para notificações por SMS. Por favor entre em contato pelo email fabricadesoftware@ufs.br para saber como usar esse serviço no MonitoraSUS.";
			}
			else
			{
				int entregasSucesso = 0;
				int entregasFalhas = 0;
				int entregasAguardando = 0;
				foreach (ExameViewModel exame in exames)
				{
					var exameModel = _exameContext.GetById(exame.Exame.IdExame);
					var pacienteModel = _pessoaContext.GetById(exame.Paciente.Idpessoa);
					if (pacienteModel.TemFoneCelularValido)
					{
						if (exameModel.Exame.StatusNotificacao.Equals(ExameModel.NOTIFICADO_ENVIADO))
						{
							var exameNotificado = await _exameContext.ConsultarSMSExameAsync(configuracaoNotificar, exameModel.Exame);
							if (exameNotificado.StatusNotificacao.Equals(ExameModel.NOTIFICADO_SIM))
								entregasSucesso++;
							else if (exameNotificado.StatusNotificacao.Equals(ExameModel.NOTIFICADO_PROBLEMAS))
								entregasFalhas++;
							else
								entregasAguardando++;
						}
					}
				}
				string mensagem = "";
				mensagem += (entregasSucesso > 0) ? "Foram entregues " + entregasSucesso + " SMS com SUCESSO. " : "";
				mensagem += (entregasFalhas > 0) ? "Ocorreram problemas no envio de " + entregasFalhas + " SMS. " : "";
				mensagem += (entregasAguardando > 0) ? "Aguardando resposta da operadora de " + entregasAguardando + " SMS." : "";

				TempData["mensagemSucesso"] = "Consultas aos SMS enviadas com sucesso! " + mensagem;
			}
			return RedirectToAction("Notificate", "Exame", pesquisaExame);
		}

		[Authorize(Roles = "GESTOR, SECRETARIO")]
		public IActionResult TotaisExames()
		{
			var usuario = _usuarioContext.RetornLoggedUser((ClaimsIdentity)User.Identity);

			var autenticadoTrabalhaEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
			var autenticadoTrabalhaMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);

			List<TotalEstadoMunicipioBairro> totaisRealizado = new List<TotalEstadoMunicipioBairro>();

			if (autenticadoTrabalhaMunicipio != null)
			{
				totaisRealizado = _exameContext.GetTotaisRealizadosByMunicipio(autenticadoTrabalhaMunicipio.IdMunicipio);
			}
			else if (autenticadoTrabalhaEstado != null)
			{
				if (autenticadoTrabalhaEstado.IdEmpresaExame == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
					totaisRealizado = _exameContext.GetTotaisRealizadosByEstado(autenticadoTrabalhaEstado.IdEstado);
				else
					totaisRealizado = _exameContext.GetTotaisRealizadosByEmpresa(autenticadoTrabalhaEstado.IdEmpresaExame);
			}
			List<TotalEstadoMunicipioBairro> totaisPopulacao = new List<TotalEstadoMunicipioBairro>();

			if (autenticadoTrabalhaMunicipio != null)
			{
				var cidade = _municicpioContext.GetById(autenticadoTrabalhaMunicipio.IdMunicipio);
				var estado = _estadoContext.GetById(Convert.ToInt32(cidade.Uf));
				totaisPopulacao = _exameContext.GetTotaisPopulacaoByMunicipio(estado.Uf, cidade.Nome);
			}
			else if (autenticadoTrabalhaEstado != null)
			{
				var estado = _estadoContext.GetById(autenticadoTrabalhaEstado.IdEstado);
				if (autenticadoTrabalhaEstado.IdEmpresaExame == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
					totaisPopulacao = _exameContext.GetTotaisPopulacaoByEstado(estado.Uf);
			}
			TotalTestesGestaoPopulacao totais = new TotalTestesGestaoPopulacao();
			totais.Gestao = totaisRealizado;
			totais.TotalGestaoRecuperados = totaisRealizado.Sum(t => t.TotalRecuperados);
			totais.TotalGestaoIndeterminados = totaisRealizado.Sum(t => t.TotalIndeterminados);
			totais.TotalGestaoNegativos = totaisRealizado.Sum(t => t.TotalNegativos);
			totais.TotalGestaoPositivos = totaisRealizado.Sum(t => t.TotalPositivos);
			totais.TotalGestaoAguardando = totaisRealizado.Sum(t => t.TotalAguardando);
			totais.TotalGestaoIgGIgM = totaisRealizado.Sum(t => t.TotalIgGIgM);

			totais.Populacao = totaisPopulacao;
			totais.TotalPopulacaoRecuperados= totaisPopulacao.Sum(t => t.TotalRecuperados);
			totais.TotalPopulacaoIndeterminados = totaisPopulacao.Sum(t => t.TotalIndeterminados);
			totais.TotalPopulacaoNegativos = totaisPopulacao.Sum(t => t.TotalNegativos);
			totais.TotalPopulacaoPositivos = totaisPopulacao.Sum(t => t.TotalPositivos);
			totais.TotalPopulacaoAguardando = totaisPopulacao.Sum(t => t.TotalAguardando);
			totais.TotalPopulacaoIgGIGM = totaisPopulacao.Sum(t => t.TotalIgGIgM);
			return View(totais);
		}


		public IActionResult Export(List<ExameViewModel> exames)
		{
			var builder = new StringBuilder();
			builder.AppendLine("Código Coleta;Vírus;Data Exame;Resultado;Situação Paciente;CPF/RG/Temp; Nome; Data Nascimento;Estado;Município;Bairro;Rua;Numero;Complemento;Fone;Diabetes;Cardiopatia;Hipertenso;Imunodeprimido;Obeso;Cancer;Doença Respiratória;Outras Comorbidades");
			foreach (var e in exames)
			{
				var exame = _exameContext.GetById(e.Exame.IdExame);
				string coleta = exame.Exame.CodigoColeta;
				string cpfRgTemp = exame.Paciente.Cpf;
				string nome = exame.Paciente.Nome;
				string situacaoSaude = exame.Paciente.SituacaoSaudeDescricao;
				string dataNascimento = exame.Paciente.DataNascimento.ToString("dd/MM/yyyy");
				string estado = exame.Paciente.Estado;
				string municipio = exame.Paciente.Cidade;
				string bairro = exame.Paciente.Bairro;
				string rua = exame.Paciente.Rua;
				string numero = exame.Paciente.Numero;
				string complemento = exame.Paciente.Complemento;
				string foneCelular = exame.Paciente.FoneCelular;
				string diabetes = exame.Paciente.Diabetes ? "Sim" : "Não";
				string cardiopatia = exame.Paciente.Cardiopatia ? "Sim" : "Não";
				string hipertenso = exame.Paciente.Hipertenso ? "Sim" : "Não";
				string imunodeprimido = exame.Paciente.Imunodeprimido ? "Sim" : "Não";
				string obeso = exame.Paciente.Obeso ? "Sim" : "Não";
				string cancer = exame.Paciente.Cancer ? "Sim" : "Não";
				string doencaRespiratoria = exame.Paciente.DoencaRespiratoria ? "Sim" : "Não";
				string outrasComorbidades = exame.Paciente.OutrasComorbidades;
				string dataExame = exame.Exame.DataExame.ToString("dd/MM/yyyy");
				string resultadoExame = exame.Exame.Resultado;
				string virus = exame.VirusBacteria.Nome;
				builder.AppendLine($"{coleta};{virus};{dataExame};{resultadoExame};{situacaoSaude};{cpfRgTemp};{nome};{dataNascimento};{estado};{municipio};{bairro};{rua};{numero};{complemento};{foneCelular};{diabetes};{cardiopatia};{hipertenso};{imunodeprimido};{obeso};{cancer};{doencaRespiratoria};{outrasComorbidades};");
			}

			return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "exames.csv");
		}

		
		public IActionResult Details(int id)
		{
			return View(_exameContext.GetById(id));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Delete(int id, PesquisaExameViewModel pesquisaExame, IFormCollection collection)
		{
			var exame = _exameContext.GetById(id);
			
			try
			{
				_exameContext.Delete(id);

				var situacoes = _situacaoPessoaContext.GetByIdPaciente(exame.Paciente.Idpessoa);
				var exames = _exameContext.GetByIdPaciente(exame.Paciente.Idpessoa);
				var pessoaTrabalhaEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(exame.Paciente.Idpessoa);
				var pessoaTrabalhaMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(exame.Paciente.Idpessoa);
				var examesPaciente = _exameContext.GetByIdPaciente(exame.Paciente.Idpessoa);
				var examesNotificados = _exameContext.GetByIdAgente(exame.Paciente.Idpessoa, DateTime.MinValue, DateTime.MaxValue);
				if (situacoes.Count == 1 && pessoaTrabalhaEstado == null && pessoaTrabalhaMunicipio == null &&
					examesPaciente.Count == 0 && examesNotificados.Count == 0)
				{
					var situacao = situacoes.First();
					_situacaoPessoaContext.Delete(situacao.Idpessoa, situacao.IdVirusBacteria);
					_pessoaContext.Delete(exame.Paciente.Idpessoa);
				}
			}
			catch
			{
				TempData["mensagemErro"] = "Houve problemas na exclusão do exame. Tente novamente em alguns minutos." +
										   " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";
				return RedirectToAction("Index", "Exame", pesquisaExame);
			}

			TempData["mensagemSucesso"] = "O Exame foi removido com sucesso!";
			return RedirectToAction("Index", "Exame", pesquisaExame);
		}

		public IActionResult Edit(int id)
		{
			ViewBag.googleKey = _configuration["GOOGLE_KEY"];
			ViewBag.VirusBacteria = new SelectList(_virusBacteriaContext.GetAll(), "IdVirusBacteria", "Nome");
			ViewBag.AreaAtuacao = new SelectList(_areaAtuacaoContext.GetAll(), "IdAreaAtuacao", "Descricao");
			return View(_exameContext.GetById(id));
		}

		/// <summary>
		/// Edita um exame existente da base de dados
		/// </summary>
		/// <param name="exameViewModel"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(ExameViewModel exameViewModel)
		{
			ViewBag.VirusBacteria = new SelectList(_virusBacteriaContext.GetAll(), "IdVirusBacteria", "Nome");
			ViewBag.AreaAtuacao = new SelectList(_areaAtuacaoContext.GetAll(), "IdAreaAtuacao", "Descricao");
			ViewBag.googleKey = _configuration["GOOGLE_KEY"];

			exameViewModel.Paciente.Cpf = exameViewModel.Paciente.Cpf ?? "";
			if (Methods.SoContemNumeros(exameViewModel.Paciente.Cpf) && !exameViewModel.Paciente.Cpf.Equals(""))
			{
				if (!Methods.ValidarCpf(exameViewModel.Paciente.Cpf))
				{
					TempData["resultadoPesquisa"] = "Esse esse cpf não é válido!";
					var exameVazio = new ExameViewModel();
					exameVazio.Paciente.Cpf = exameViewModel.Paciente.Cpf;
					return View(exameVazio);
				}
			}

			try
			{
				/* 
                 * Verificando se o usuario está atualizando 
                 * cpf/rg duplicado duplicado 
                 */
				var usuarioDuplicado = _pessoaContext.GetByCpf(exameViewModel.Paciente.Cpf);
				if (usuarioDuplicado != null)
				{
					if (!(usuarioDuplicado.Idpessoa == exameViewModel.Paciente.Idpessoa))
					{
						TempData["mensagemErro"] = "Já existe um paciente com esse CPF/RG, tente novamente!";
						return View(exameViewModel);
					}
				}

				/* 
                 * Verificando duplicidade de exames no mesmo dia
                 * na hora de atulizar um registro
                 */
				var check = _exameContext.CheckDuplicateExamToday(exameViewModel.Paciente.Idpessoa, exameViewModel.VirusBacteria.IdVirusBacteria, exameViewModel.Exame.DataExame, exameViewModel.Exame.MetodoExame);
				if (check.Count > 0)
				{
					var status = false;
					foreach (var item in check)
					{
						if (item.IdExame == exameViewModel.Exame.IdExame)
							status = true;
					}

					if (!status)
					{
						TempData["mensagemErro"] = "Notificação DUPLICADA! Já existe um exame registrado desse paciente para esse Vírus/Bactéria na " +
													"data informada. Por favor, verifique se os dados da notificação estão corretos.";
						return View(exameViewModel);
					}
				}

				var situacao = _situacaoPessoaContext.GetById(exameViewModel.Paciente.Idpessoa, exameViewModel.VirusBacteria.IdVirusBacteria);
				if (situacao == null)
					_situacaoPessoaContext.Insert(CreateSituacaoPessoaModelByExame(exameViewModel, situacao));
				else
					_situacaoPessoaContext.Update(CreateSituacaoPessoaModelByExame(exameViewModel, situacao));

				_pessoaContext.Update(CreatePessoaModelByExame(exameViewModel), false);
				var usuario = _usuarioContext.RetornLoggedUser((ClaimsIdentity)User.Identity);
				exameViewModel.Exame.IdAgenteSaude = usuario.UsuarioModel.IdPessoa;
				_exameContext.Update(exameViewModel.Exame);

				TempData["mensagemSucesso"] = "Edição realizada com SUCESSO!";

				return View(new ExameViewModel());

			}
			catch (Exception e)
			{
				TempData["mensagemErro"] = "Houve um problema ao atualizar as informações, tente novamente." +
				  " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";

				return View(exameViewModel);
			}
		}



		public IActionResult Create()
		{
			ViewBag.googleKey = _configuration["GOOGLE_KEY"];
			ViewBag.VirusBacteria = new SelectList(_virusBacteriaContext.GetAll(), "IdVirusBacteria", "Nome");
			ViewBag.AreaAtuacao = new SelectList(_areaAtuacaoContext.GetAll(), "IdAreaAtuacao", "Descricao");
			return View(new ExameViewModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(ExameViewModel exameViewModel)
		{
			ViewBag.googleKey = _configuration["GOOGLE_KEY"];
			ViewBag.VirusBacteria = new SelectList(_virusBacteriaContext.GetAll(), "IdVirusBacteria", "Nome");
			ViewBag.AreaAtuacao = new SelectList(_areaAtuacaoContext.GetAll(), "IdAreaAtuacao", "Descricao");

			exameViewModel.Paciente.Cpf = exameViewModel.Paciente.Cpf ?? "";
			if (Methods.SoContemNumeros(exameViewModel.Paciente.Cpf) && !exameViewModel.Paciente.Cpf.Equals(""))
			{
				if (!Methods.ValidarCpf(exameViewModel.Paciente.Cpf))
				{
					TempData["resultadoPesquisa"] = "Esse esse cpf não é válido!";
					var exameVazio = new ExameViewModel();
					exameVazio.Paciente.Cpf = exameViewModel.Paciente.Cpf;
					return View(exameVazio);
				}
			}

			/* 
             * verificando se é pra pesquisar ou inserir um novo exame 
             */
			if (exameViewModel.PesquisarCpf == 1)
			{
				var cpf = Methods.RemoveSpecialsCaracts(exameViewModel.Paciente.Cpf); // cpf sem caracteres especiais
				var pessoa = _pessoaContext.GetByCpf(cpf);
				if (pessoa != null)
				{
					exameViewModel.Paciente = pessoa;
					return View(exameViewModel);
				}
				else
				{
					var exameVazio = new ExameViewModel();
					exameVazio.Paciente.Cpf = exameViewModel.Paciente.Cpf;
					return View(exameVazio);
				}
			}
			else
			{
				CreatePessoaModelByExame(exameViewModel);
				if (_exameContext.CheckDuplicateExamToday(exameViewModel.Paciente.Idpessoa, exameViewModel.VirusBacteria.IdVirusBacteria, exameViewModel.Exame.DataExame, exameViewModel.Exame.MetodoExame).Count > 0)
				{
					TempData["mensagemErro"] = "Notificação DUPLICADA! Já existe um exame registrado desse paciente para esse Vírus/Bactéria na " +
												"data informada e método aplicado. Por favor, verifique se os dados da notificação estão corretos.";
					return View(exameViewModel);
				}


				try
				{
					if (!ModelState.IsValid)
					{
						return View(exameViewModel);
					}
					
					if (exameViewModel.Paciente.Idpessoa == 0)
						exameViewModel.Paciente = _pessoaContext.Insert(exameViewModel.Paciente);
					else
						exameViewModel.Paciente = _pessoaContext.Update(exameViewModel.Paciente, false);
					

					// inserindo o resultado do exame (situacao da pessoa)                  
					var situacaoPessoa = _situacaoPessoaContext.GetById(exameViewModel.Paciente.Idpessoa, exameViewModel.VirusBacteria.IdVirusBacteria);
					if (situacaoPessoa == null)
						_situacaoPessoaContext.Insert(CreateSituacaoPessoaModelByExame(exameViewModel, situacaoPessoa));
					else
						_situacaoPessoaContext.Update(CreateSituacaoPessoaModelByExame(exameViewModel, situacaoPessoa));
					var usuario = _usuarioContext.RetornLoggedUser((ClaimsIdentity)User.Identity);
					exameViewModel.Exame.IdAgenteSaude = usuario.UsuarioModel.IdPessoa;
					_exameContext.Insert(exameViewModel.Exame);
				}
				catch (Exception)
				{
					TempData["mensagemErro"] = "Cadastro não pode ser concluido pois houve um problema ao inserir os dados do exame, tente novamente." +
											   " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";
					return View(exameViewModel);
				}

				TempData["mensagemSucesso"] = "Notificação realizada com SUCESSO!";

				return RedirectToAction(nameof(Create));
			}
		}

		public SituacaoPessoaVirusBacteriaModel CreateSituacaoPessoaModelByExame(ExameViewModel exame, SituacaoPessoaVirusBacteriaModel situacao)
		{

			if (situacao != null)
			{
				situacao.UltimaSituacaoSaude = exame.Exame.ResultadoStatus;
			}
			else
			{
				situacao = new SituacaoPessoaVirusBacteriaModel();
				situacao.IdVirusBacteria = exame.VirusBacteria.IdVirusBacteria;
				situacao.Idpessoa = _pessoaContext.GetByCpf(Methods.RemoveSpecialsCaracts(exame.Paciente.Cpf)).Idpessoa;
				situacao.UltimaSituacaoSaude = exame.Exame.ResultadoStatus;
				situacao.DataUltimoMonitoramento = null;
			}

			return situacao;
		}

		public PesquisaExameViewModel BuscaExamesViewModel(PesquisaExameViewModel pesquisaExame)
		{
			
			var usuario = _usuarioContext.RetornLoggedUser((ClaimsIdentity)User.Identity);
			var pessoaTrabalhaMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
			
			bool dataInicialIndeterminada = pesquisaExame.DataInicial.Equals(DateTime.MinValue);
			var exames = new List<ExameBuscaModel>();
			if (usuario.RoleUsuario.Equals("AGENTE") || usuario.RoleUsuario.Equals("ADM"))
			{
				if (dataInicialIndeterminada)
					exames = _exameContext.GetByIdAgente(usuario.UsuarioModel.IdPessoa, 15);
				else
					exames = _exameContext.GetByIdAgente(usuario.UsuarioModel.IdPessoa, pesquisaExame.DataInicial, pesquisaExame.DataFinal);
			}
			else if (usuario.RoleUsuario.Equals("GESTOR") || usuario.RoleUsuario.Equals("SECRETARIO"))
			{
				if (pessoaTrabalhaMunicipio != null)
				{
					if (dataInicialIndeterminada)
						exames = _exameContext.GetByIdMunicipio(pessoaTrabalhaMunicipio.IdMunicipio, 15);
					else
						exames = _exameContext.GetByIdMunicipio(pessoaTrabalhaMunicipio.IdMunicipio, pesquisaExame.DataInicial, pesquisaExame.DataFinal);
				}
				else
				{
					var pessoaTrabalhaEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
					if (pessoaTrabalhaEstado != null)
					{
						if (pessoaTrabalhaEstado.IdEmpresaExame != EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
						{
							if (dataInicialIndeterminada)
								exames = _exameContext.GetByIdEmpresa(pessoaTrabalhaEstado.IdEmpresaExame, 15);
							else
								exames = _exameContext.GetByIdEmpresa(pessoaTrabalhaEstado.IdEmpresaExame, pesquisaExame.DataInicial, pesquisaExame.DataFinal);
						}
						else
						{
							if (dataInicialIndeterminada)
								exames = _exameContext.GetByIdEstado(pessoaTrabalhaEstado.IdEstado, 15);
							else
								exames = _exameContext.GetByIdEstado(pessoaTrabalhaEstado.IdEstado, pesquisaExame.DataInicial, pesquisaExame.DataFinal);
						}
					}
				}
			}
			pesquisaExame.Exames = exames;
			/*
             * 2º Filtro - filtrando ViewModel por nome ou cpf e resultado
             */
			pesquisaExame.Pesquisa = pesquisaExame.Pesquisa ?? "";
			pesquisaExame.Resultado = pesquisaExame.Resultado ?? "";
			pesquisaExame.Cidade = pesquisaExame.Cidade ?? "";

			if (!pesquisaExame.Pesquisa.Equals(""))
				if (Methods.SoContemLetras(pesquisaExame.Pesquisa))
					pesquisaExame.Exames = pesquisaExame.Exames.Where(exameViewModel => exameViewModel.NomePaciente.ToUpper().Contains(pesquisaExame.Pesquisa.ToUpper())).ToList();
				else
					pesquisaExame.Exames = pesquisaExame.Exames.Where(exameViewModel => exameViewModel.Cpf.ToUpper().Contains(pesquisaExame.Pesquisa.ToUpper())).ToList();

			if (!pesquisaExame.Resultado.Equals("") && !pesquisaExame.Resultado.Equals("Todas as Opçoes"))
				pesquisaExame.Exames = pesquisaExame.Exames.Where(exameViewModel => exameViewModel.Exame.Resultado.ToUpper().Equals(pesquisaExame.Resultado.ToUpper())).ToList();

			if (!pesquisaExame.Cidade.Equals(""))
				pesquisaExame.Exames = pesquisaExame.Exames.Where(exameViewModel => exameViewModel.Cidade.ToUpper().Contains(pesquisaExame.Cidade.ToUpper())).ToList();

			/* 
             * Ordenando lista por data e pegando maior e menor datas... 
             */
			if (pesquisaExame.Exames.Count > 0)
			{
				pesquisaExame.Exames = pesquisaExame.Exames.OrderByDescending(ex => ex.Exame.DataExame).ToList();
				pesquisaExame.DataFinal = pesquisaExame.Exames[0].Exame.DataExame;
				pesquisaExame.DataInicial = pesquisaExame.Exames[pesquisaExame.Exames.Count - 1].Exame.DataExame;
			}

			pesquisaExame.Exames = pesquisaExame.Exames.OrderBy(ex => ex.Exame.CodigoColeta).ToList();
			return PreencheTotalizadores(pesquisaExame);
		}

		public PessoaModel CreatePessoaModelByExame(ExameViewModel exameViewModel)
		{
			var user = _pessoaContext.GetByCpf(exameViewModel.Paciente.Cpf.ToUpper());
			if (user == null)
			{
				exameViewModel.Paciente.Idpessoa = 0;
				exameViewModel.Paciente.IdAreaAtuacao = exameViewModel.Exame.IdAreaAtuacao;
				if (exameViewModel.Exame.AguardandoResultado == true || exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_POSITIVO) ||
					exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_IGMIGG))
					exameViewModel.Paciente.SituacaoSaude = PessoaModel.SITUACAO_ISOLAMENTO;
			}
			else
			{
				exameViewModel.Paciente.Idpessoa = user.Idpessoa;
				exameViewModel.Paciente.IdAreaAtuacao = exameViewModel.Exame.IdAreaAtuacao;
				if ((exameViewModel.Paciente.SituacaoSaude.Equals(PessoaModel.SITUACAO_SAUDAVEL) & !exameViewModel.Paciente.SituacaoSaude.Equals(PessoaModel.SITUACAO_SAUDAVEL) && exameViewModel.Exame.AguardandoResultado == false)
					|| (exameViewModel.Paciente.SituacaoSaude.Equals(PessoaModel.SITUACAO_SAUDAVEL) && exameViewModel.Exame.AguardandoResultado == true))
				{
					exameViewModel.Paciente.SituacaoSaude = PessoaModel.SITUACAO_ISOLAMENTO;
				}
				else if (exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_NEGATIVO) || exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_RECUPERADO)
				  || exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_INDETERMINADO))
				{
					DateTime dataMinima = DateTime.Now.AddDays(exameViewModel.VirusBacteria.DiasRecuperacao * (-1));
					var exames = _exameContext.GetByIdPaciente(user.Idpessoa).Where(e => e.Exame.DataExame >= dataMinima).ToList();
					if (exames.Count() <= 1 && exameViewModel.Paciente.SituacaoSaude.Equals(PessoaModel.SITUACAO_ISOLAMENTO))
					{
						exameViewModel.Paciente.SituacaoSaude = PessoaModel.SITUACAO_SAUDAVEL;
					}
				}
			}
			return exameViewModel.Paciente;
		}

		public PesquisaExameViewModel PreencheTotalizadores(PesquisaExameViewModel examesTotalizados)
		{

			foreach (var item in examesTotalizados.Exames)
			{
				switch (item.Exame.Resultado)
				{
					case ExameModel.RESULTADO_POSITIVO: examesTotalizados.Positivos++; break;
					case ExameModel.RESULTADO_NEGATIVO: examesTotalizados.Negativos++; break;
					case ExameModel.RESULTADO_INDETERMINADO: examesTotalizados.Indeterminados++; break;
					case ExameModel.RESULTADO_RECUPERADO: examesTotalizados.Recuperados++; break;
					case ExameModel.RESULTADO_AGUARDANDO: examesTotalizados.Aguardando++; break;
					case ExameModel.RESULTADO_IGMIGG: examesTotalizados.IgMIgGs++; break;
				}
			}
			return examesTotalizados;
		}
	}
}
