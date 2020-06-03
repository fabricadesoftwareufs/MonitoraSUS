using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Model;
using Model.ViewModel;
using MonitoraSUS.Utils;
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
		private readonly IConfiguration _configuration;

		public ExameController(IVirusBacteriaService virusBacteriaContext,
							   IExameService exameContext,
							   IPessoaService pessoaContext,
							   IMunicipioService municicpioContext,
							   IEstadoService estadoContext,
							   IConfiguration configuration,
							   ISituacaoVirusBacteriaService situacaoPessoaContext,
							   IPessoaTrabalhaEstadoService pessoaTrabalhaEstado,
							   IPessoaTrabalhaMunicipioService pessoaTrabalhaMunicipioContext)
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
		}

		public IActionResult Index(PesquisaExameViewModel pesquisaExame)
		{
			return View(GetAllExamesViewModel(pesquisaExame));
		}


		public IActionResult Notificate(PesquisaExameViewModel pesquisaExame)
		{
			return View(GetAllExamesViewModel(pesquisaExame));
		}

		[Authorize(Roles = "GESTOR, SECRETARIO")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EnviarSMS(int id, IFormCollection collection)
		{
			var exame = _exameContext.GetById(id);
			var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);
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
				if (configuracaoNotificar.QuantidadeSmsdisponivel == 0 && exame.StatusNotificacao.Equals(ExameModel.NOTIFICADO_NAO))
				{
					TempData["mensagemErro"] = "Não possui créditos para enviar SMS. " +
						"Por favor entre em contato pelo email fabricadesoftware@ufs.br se precisar novos créditos.";
				}
				else
				{
					var pacienteModel = _pessoaContext.GetById(exame.IdPaciente);
					string statusAnteriorSMS = exame.StatusNotificacao;
					if (pacienteModel.TemFoneCelularValido)
					{
						if (exame.StatusNotificacao.Equals(ExameModel.NOTIFICADO_ENVIADO))
						{
							exame = await _exameContext.ConsultarSMSExameAsync(configuracaoNotificar, exame);
						}
						else if (exame.StatusNotificacao.Equals(ExameModel.NOTIFICADO_NAO) || exame.StatusNotificacao.Equals(ExameModel.NOTIFICADO_PROBLEMAS))
						{
							exame = await _exameContext.EnviarSMSResultadoExameAsync(configuracaoNotificar, exame, pacienteModel);
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
			return RedirectToAction(nameof(Notificate));
		}

		[Authorize(Roles = "GESTOR, SECRETARIO")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ConsultarSMSEnviados(List<ExameViewModel> exames)
		{
			var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);
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
					var exameModel = _exameContext.GetById(exame.IdExame);
					var pacienteModel = _pessoaContext.GetById(exame.IdPaciente.Idpessoa);
					if (pacienteModel.TemFoneCelularValido)
					{
						if (exameModel.StatusNotificacao.Equals(ExameModel.NOTIFICADO_ENVIADO))
						{
							var exameNotificado = await _exameContext.ConsultarSMSExameAsync(configuracaoNotificar, exameModel);
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
			return RedirectToAction(nameof(Notificate));
		}

		[Authorize(Roles = "GESTOR, SECRETARIO")]
		public IActionResult TotaisExames()
		{
			var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);

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
				var exame = GetExameViewModelById(e.IdExame);
				string coleta = exame.CodigoColeta;
				string cpfRgTemp = exame.IdPaciente.Cpf;
				string nome = exame.IdPaciente.Nome;
				string situacaoSaude = exame.IdPaciente.SituacaoSaudeDescricao;
				string dataNascimento = exame.IdPaciente.DataNascimento.ToString("dd/MM/yyyy");
				string estado = exame.IdPaciente.Estado;
				string municipio = exame.IdPaciente.Cidade;
				string bairro = exame.IdPaciente.Bairro;
				string rua = exame.IdPaciente.Rua;
				string numero = exame.IdPaciente.Numero;
				string complemento = exame.IdPaciente.Complemento;
				string foneCelular = exame.IdPaciente.FoneCelular;
				string diabetes = exame.IdPaciente.Diabetes ? "Sim" : "Não";
				string cardiopatia = exame.IdPaciente.Cardiopatia ? "Sim" : "Não";
				string hipertenso = exame.IdPaciente.Hipertenso ? "Sim" : "Não";
				string imunodeprimido = exame.IdPaciente.Imunodeprimido ? "Sim" : "Não";
				string obeso = exame.IdPaciente.Obeso ? "Sim" : "Não";
				string cancer = exame.IdPaciente.Cancer ? "Sim" : "Não";
				string doencaRespiratoria = exame.IdPaciente.DoencaRespiratoria ? "Sim" : "Não";
				string outrasComorbidades = exame.IdPaciente.OutrasComorbidades;
				string dataExame = exame.DataExame.ToString("dd/MM/yyyy");
				string resultadoExame = exame.Resultado;
				string virus = exame.IdVirusBacteria.Nome;
				builder.AppendLine($"{coleta};{virus};{dataExame};{resultadoExame};{situacaoSaude};{cpfRgTemp};{nome};{dataNascimento};{estado};{municipio};{bairro};{rua};{numero};{complemento};{foneCelular};{diabetes};{cardiopatia};{hipertenso};{imunodeprimido};{obeso};{cancer};{doencaRespiratoria};{outrasComorbidades};");
			}

			return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "exames.csv");
		}

		
		public IActionResult Details(int id)
		{
			return View(GetExameViewModelById(id));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Delete(int id, IFormCollection collection)
		{

			var exame = _exameContext.GetById(id);
			
			try
			{
				_exameContext.Delete(id);

				var situacoes = _situacaoPessoaContext.GetByIdPaciente(exame.IdPaciente);
				var exames = _exameContext.GetByIdPaciente(exame.IdPaciente);
				var pessoaTrabalhaEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(exame.IdPaciente);
				var pessoaTrabalhaMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(exame.IdPaciente);
				var examesPaciente = _exameContext.GetByIdPaciente(exame.IdPaciente);
				var examesNotificados = _exameContext.GetByIdAgente(exame.IdPaciente);
				if (situacoes.Count == 1 && pessoaTrabalhaEstado == null && pessoaTrabalhaMunicipio == null &&
					examesPaciente.Count == 0 && examesNotificados.Count == 0)
				{
					var situacao = situacoes.First();
					_situacaoPessoaContext.Delete(situacao.Idpessoa, situacao.IdVirusBacteria);
					_pessoaContext.Delete(exame.IdPaciente);
				}
			}
			catch
			{
				TempData["mensagemErro"] = "Houve problemas na exclusão do exame. Tente novamente em alguns minutos." +
										   " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";
				return RedirectToAction(nameof(Index));
			}

			TempData["mensagemSucesso"] = "O Exame foi removido com sucesso!";
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Edit(int id)
		{
			ViewBag.googleKey = _configuration["GOOGLE_KEY"];
			ViewBag.VirusBacteria = new SelectList(_virusBacteriaContext.GetAll(), "IdVirusBacteria", "Nome");
			return View(GetExameViewModelById(id));
		}

		/// <summary>
		/// Edita um exame existente da base de dados
		/// </summary>
		/// <param name="exame"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(ExameViewModel exame)
		{
			ViewBag.VirusBacteria = new SelectList(_virusBacteriaContext.GetAll(), "IdVirusBacteria", "Nome");
			ViewBag.googleKey = _configuration["GOOGLE_KEY"];

			exame.IdPaciente.Cpf = exame.IdPaciente.Cpf ?? "";
			if (Methods.SoContemNumeros(exame.IdPaciente.Cpf) && !exame.IdPaciente.Cpf.Equals(""))
			{
				if (!Methods.ValidarCpf(exame.IdPaciente.Cpf))
				{
					TempData["resultadoPesquisa"] = "Esse esse cpf não é válido!";
					/*  
                     * Limpando o objeto para enviar  
                     * somente o cpf pesquisado
                     */
					var exameVazio = new ExameViewModel();
					exameVazio.IdPaciente.Cpf = exame.IdPaciente.Cpf;
					return View(exameVazio);
				}
			}

			try
			{
				/* 
                 * Verificando se o usuario está atualizando 
                 * cpf/rg duplicado duplicado 
                 */
				var usuarioDuplicado = _pessoaContext.GetByCpf(exame.IdPaciente.Cpf);
				if (usuarioDuplicado != null)
				{
					if (!(usuarioDuplicado.Idpessoa == exame.IdPaciente.Idpessoa))
					{
						TempData["mensagemErro"] = "Já existe um paciente com esse CPF/RG, tente novamente!";
						return View(exame);
					}
				}

				/* 
                 * Verificando duplicidade de exames no mesmo dia
                 * na hora de atulizar um registro
                 */
				var check = _exameContext.CheckDuplicateExamToday(exame.IdPaciente.Idpessoa, exame.IdVirusBacteria.IdVirusBacteria, exame.DataExame, exame.MetodoExame);
				if (check.Count > 0)
				{
					var status = false;
					foreach (var item in check)
					{
						if (item.IdExame == exame.IdExame)
							status = true;
					}

					if (!status)
					{
						TempData["mensagemErro"] = "Notificação DUPLICADA! Já existe um exame registrado desse paciente para esse Vírus/Bactéria na " +
													"data informada. Por favor, verifique se os dados da notificação estão corretos.";
						return View(exame);
					}
				}

				/*
                 * Atualizando Exame
                 */
				_exameContext.Update(CreateExameModel(exame, 0, false));

				/*
                 * Atualizando ou Inserindo situacao do usuario 
                 */
				var situacao = _situacaoPessoaContext.GetById(exame.IdPaciente.Idpessoa, exame.IdVirusBacteria.IdVirusBacteria);
				if (situacao == null)
					_situacaoPessoaContext.Insert(CreateSituacaoPessoaModelByExame(exame, situacao));
				else
					_situacaoPessoaContext.Update(CreateSituacaoPessoaModelByExame(exame, situacao));

				/*
                 * Atualizando as informações do paciente
                 */
				_pessoaContext.Update(CreatePessoaModelByExame(exame), false);

				TempData["mensagemSucesso"] = "Edição realizada com SUCESSO!";

				return View(new ExameViewModel());

			}
			catch (Exception e)
			{
				TempData["mensagemErro"] = "Houve um problema ao atualizar as informações, tente novamente." +
				  " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";

				return View(exame);
			}
		}



		public IActionResult Create()
		{
			ViewBag.googleKey = _configuration["GOOGLE_KEY"];
			ViewBag.VirusBacteria = new SelectList(_virusBacteriaContext.GetAll(), "IdVirusBacteria", "Nome");
			return View(new ExameViewModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(ExameViewModel exame)
		{
			ViewBag.googleKey = _configuration["GOOGLE_KEY"];
			ViewBag.VirusBacteria = new SelectList(_virusBacteriaContext.GetAll(), "IdVirusBacteria", "Nome");

			exame.IdPaciente.Cpf = exame.IdPaciente.Cpf ?? "";
			if (Methods.SoContemNumeros(exame.IdPaciente.Cpf) && !exame.IdPaciente.Cpf.Equals(""))
			{
				if (!Methods.ValidarCpf(exame.IdPaciente.Cpf))
				{
					TempData["resultadoPesquisa"] = "Esse esse cpf não é válido!";
					/*  
                     * Limpando o objeto para enviar  
                     * somente o cpf pesquisado
                     */
					var exameVazio = new ExameViewModel();
					exameVazio.IdPaciente.Cpf = exame.IdPaciente.Cpf;
					return View(exameVazio);
				}
			}

			/* 
             * verificando se é pra pesquisar ou inserir um novo exame 
             */
			if (exame.PesquisarCpf == 1)
			{
				var cpf = Methods.RemoveSpecialsCaracts(exame.IdPaciente.Cpf); // cpf sem caracteres especiais

				var pessoa = _pessoaContext.GetByCpf(cpf);

				if (pessoa != null)
				{
					exame.IdPaciente = pessoa;
					return View(exame);
				}
				else
				{
					/*
                     * Limpando o objeto para enviar  
                     * somente o cpf pesquisado
                     */
					var exameVazio = new ExameViewModel();
					exameVazio.IdPaciente.Cpf = exame.IdPaciente.Cpf;

					return View(exameVazio);
				}
			}
			else
			{
				var pessoa = CreatePessoaModelByExame(exame);
				if (_exameContext.CheckDuplicateExamToday(pessoa.Idpessoa, exame.IdVirusBacteria.IdVirusBacteria, exame.DataExame, exame.MetodoExame).Count > 0)
				{
					TempData["mensagemErro"] = "Notificação DUPLICADA! Já existe um exame registrado desse paciente para esse Vírus/Bactéria na " +
												"data informada e método aplicado. Por favor, verifique se os dados da notificação estão corretos.";
					return View(exame);
				}

				try
				{
					var pessoaBusca = _pessoaContext.GetByCpf(pessoa.Cpf);
					if (pessoaBusca == null)
					{
						if (exame.Resultado.Equals(ExameModel.RESULTADO_POSITIVO) && exame.AguardandoResultado == false)
							pessoa.SituacaoSaude = PessoaModel.SITUACAO_ISOLAMENTO;
						pessoa = _pessoaContext.Insert(pessoa);
					}
					else
					{
						if (pessoaBusca.SituacaoSaude.Equals(PessoaModel.SITUACAO_SAUDAVEL) 
							&& exame.Resultado.Equals(ExameModel.RESULTADO_POSITIVO)
							&& exame.AguardandoResultado == false)
							pessoa.SituacaoSaude = PessoaModel.SITUACAO_ISOLAMENTO;
						pessoa = _pessoaContext.Update(pessoa, false);
					}
				}
				catch
				{
					TempData["mensagemErro"] = "Cadastro não pode ser concluido pois houve um problema ao inserir/atualizar dados do paciente, tente novamente. " +
												" Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";
					return View(exame);
				}


				try
				{
					// inserindo o resultado do exame (situacao da pessoa)                  
					var situacaoPessoa = _situacaoPessoaContext.GetById(pessoa.Idpessoa, exame.IdVirusBacteria.IdVirusBacteria);

					if (situacaoPessoa == null)
						_situacaoPessoaContext.Insert(CreateSituacaoPessoaModelByExame(exame, situacaoPessoa));
					else
						_situacaoPessoaContext.Update(CreateSituacaoPessoaModelByExame(exame, situacaoPessoa));
				}
				catch (Exception e)
				{
					TempData["mensagemErro"] = "Cadastro não pode ser concluido pois houve um problema ao inserir/atualizar o resultado do exame, tente novamente" +
												" Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";
					return View(exame);
				}

				try
				{
					var exameModel = CreateExameModel(exame, pessoa.Idpessoa, true);
					// inserindo o exame
					_exameContext.Insert(exameModel);
				}
				catch (Exception e)
				{
					TempData["mensagemErro"] = "Cadastro não pode ser concluido pois houve um problema ao inserir os dados do exame, tente novamente." +
											   " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";
					return View(exame);
				}

				// codigo para realizar notificacao 

				TempData["mensagemSucesso"] = "Notificação realizada com SUCESSO!";

				return RedirectToAction(nameof(Create));
			}
		}

		public SituacaoPessoaVirusBacteriaModel CreateSituacaoPessoaModelByExame(ExameViewModel exame, SituacaoPessoaVirusBacteriaModel situacao)
		{

			if (situacao != null)
			{
				situacao.UltimaSituacaoSaude = exame.ResultadoStatus;
			}
			else
			{
				situacao = new SituacaoPessoaVirusBacteriaModel();
				situacao.IdVirusBacteria = exame.IdVirusBacteria.IdVirusBacteria;
				situacao.Idpessoa = _pessoaContext.GetByCpf(Methods.RemoveSpecialsCaracts(exame.IdPaciente.Cpf)).Idpessoa;
				situacao.UltimaSituacaoSaude = exame.ResultadoStatus;
				situacao.DataUltimoMonitoramento = null;
			}

			return situacao;
		}

		public ExameModel CreateExameModel(ExameViewModel viewModel, int idPaciente, bool create)
		{
			ExameModel exame = new ExameModel();

			exame.IdExame = viewModel.IdExame;
			if (create)
				exame.IdPaciente = idPaciente;
			else
				exame.IdPaciente = viewModel.IdPaciente.Idpessoa;

			exame.IdVirusBacteria = viewModel.IdVirusBacteria.IdVirusBacteria;
			exame.IgG = viewModel.IgG;
			exame.IgM = viewModel.IgM;
			exame.Pcr = viewModel.Pcr;
			exame.IgGIgM = viewModel.IgGIgM;
			exame.MetodoExame = viewModel.MetodoExame;
			exame.IdEstado = viewModel.IdEstado;
			exame.IdMunicipio = viewModel.MunicipioId;
			exame.DataInicioSintomas = viewModel.DataInicioSintomas;
			exame.DataExame = viewModel.DataExame;
			exame.IdEmpresaSaude = viewModel.IdEmpresaSaude;
			exame.EhProfissionalSaude = viewModel.EhProfissionalSaude;
			exame.CodigoColeta = (viewModel.CodigoColeta == null) ? "" : viewModel.CodigoColeta;
			exame.StatusNotificacao = viewModel.StatusNotificacao;
			exame.CodigoColeta = viewModel.CodigoColeta == null ? "" : viewModel.CodigoColeta;
			exame.IdNotificacao = viewModel.IdNotificacao;
			exame.AguardandoResultado = viewModel.AguardandoResultado;
			exame.Coriza = viewModel.Coriza;
			exame.Diarreia = viewModel.Diarreia;
			exame.DificuldadeRespiratoria = viewModel.DificuldadeRespiratoria;
			exame.DorAbdominal = viewModel.DorAbdominal;
			exame.DorGarganta = viewModel.DorGarganta;
			exame.DorOuvido = viewModel.DorOuvido;
			exame.Febre = viewModel.Febre;
			exame.Nausea = viewModel.Nausea;
			exame.PerdaOlfatoPaladar = viewModel.PerdaOlfatoPaladar;
			exame.RelatouSintomas = viewModel.RelatouSintomas;
			exame.Tosse = viewModel.Tosse;
			/*
             *  pegando informações do agente de saúde logado no sistema 
             */
			var agente = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);

			var secretarioMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(agente.UsuarioModel.IdPessoa);
			var secretarioEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(agente.UsuarioModel.IdPessoa);

			// verificando se o funcionario trabalha no municipio ou no estado
			if (secretarioMunicipio != null)
			{
				exame.IdMunicipio = secretarioMunicipio.IdMunicipio;
				exame.IdEstado = Convert.ToInt32(_municicpioContext.GetById(secretarioMunicipio.IdMunicipio).Uf);
				exame.IdEmpresaSaude = 1; // empresa padrão do banco 
			}
			else
			{
				exame.IdEstado = secretarioEstado.IdEstado;
				exame.IdEmpresaSaude = secretarioEstado.IdEmpresaExame;
				exame.IdMunicipio = null;
			}

			exame.IdAgenteSaude = agente.UsuarioModel.IdPessoa;

			return exame;
		}

		public ExameViewModel GetExameViewModelById(int id)
		{
			var exame = _exameContext.GetById(id);

			ExameViewModel ex = new ExameViewModel();

			ex.IdExame = exame.IdExame;
			ex.IdPaciente = _pessoaContext.GetById(exame.IdPaciente);
			ex.IdAgenteSaude = _pessoaContext.GetById(exame.IdAgenteSaude);
			ex.IdVirusBacteria = _virusBacteriaContext.GetById(exame.IdVirusBacteria);
			//ex.Resultado = Methods.GetStatusExame(Methods.GetResultadoExame(new ExameViewModel { Pcr = exame.Pcr, IgG = exame.IgG, IgM = exame.IgM }));
			ex.IgG = exame.IgG;
			ex.IgM = exame.IgM;
			ex.Pcr = exame.Pcr;
			ex.IgGIgM = exame.IgGIgM;
			ex.MetodoExame = exame.MetodoExame;
			ex.IdEstado = exame.IdEstado;
			ex.MunicipioId = exame.IdMunicipio;
			ex.DataInicioSintomas = exame.DataInicioSintomas;
			ex.DataExame = exame.DataExame;
			ex.IdEmpresaSaude = exame.IdEmpresaSaude;
			ex.EhProfissionalSaude = exame.EhProfissionalSaude;
			ex.CodigoColeta = exame.CodigoColeta;
			ex.StatusNotificacao = exame.StatusNotificacao;
			ex.IdNotificacao = exame.IdNotificacao;
			ex.AguardandoResultado = exame.AguardandoResultado;
			ex.Coriza = exame.Coriza;
			ex.Diarreia = exame.Diarreia;
			ex.DificuldadeRespiratoria = exame.DificuldadeRespiratoria;
			ex.DorAbdominal = exame.DorAbdominal;
			ex.DorGarganta = exame.DorGarganta;
			ex.DorOuvido = exame.DorOuvido;
			ex.Febre = exame.Febre;
			ex.Nausea = exame.Nausea;
			ex.PerdaOlfatoPaladar = exame.PerdaOlfatoPaladar;
			ex.RelatouSintomas = exame.RelatouSintomas;
			ex.Tosse = exame.Tosse;
			return ex;
		}


		public PesquisaExameViewModel GetAllExamesViewModel(PesquisaExameViewModel pesquisaExame)
		{
			/*
             * Pegando usuario logado e carregando 
             * os exames que ele pode ver
             */
			var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);
			var pessoaTrabalhaMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
			var pessoaTrabalhaEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);

			var exames = new List<ExameModel>();
			if (usuario.RoleUsuario.Equals("AGENTE") || usuario.RoleUsuario.Equals("ADM"))
			{
				exames = _exameContext.GetByIdAgente(usuario.UsuarioModel.IdPessoa);
			}
			else if (usuario.RoleUsuario.Equals("GESTOR") || usuario.RoleUsuario.Equals("SECRETARIO"))
			{
				if (pessoaTrabalhaMunicipio != null)
					exames = _exameContext.GetByIdMunicipio(pessoaTrabalhaMunicipio.IdMunicipio);
				if (pessoaTrabalhaEstado != null)
				{
					if (pessoaTrabalhaEstado.IdEmpresaExame != EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
						exames = _exameContext.GetByIdEmpresa(pessoaTrabalhaEstado.IdEmpresaExame);
					else
						exames = _exameContext.GetByIdEstado(pessoaTrabalhaEstado.IdEstado);
				}
			}

			/* 
             * 1º Filto - por datas 
             */
			if (pesquisaExame.DataInicial == DateTime.MinValue && pesquisaExame.DataFinal == DateTime.MinValue && !pesquisaExame.RealizouPesquisa)
			{
				//pesquisaExame.DataInicial = DateTime.Now.AddDays(-7);
				//pesquisaExame.DataFinal = DateTime.Now;
				//exames = exames.Where(exameModel => exameModel.DataExame >= pesquisaExame.DataInicial && exameModel.DataExame <= DateTime.Now).OrderBy(ex => ex.DataExame).ToList();
				exames = exames.TakeLast(10).ToList();
			}
			else if (pesquisaExame.DataInicial > DateTime.MinValue && pesquisaExame.DataFinal > DateTime.MinValue)
			{
				exames = exames.Where(exameModel => exameModel.DataExame >= pesquisaExame.DataInicial && exameModel.DataExame <= pesquisaExame.DataFinal).ToList();
			}
			else if (pesquisaExame.DataInicial == DateTime.MinValue && pesquisaExame.DataFinal > DateTime.MinValue)
			{
				exames = exames.Where(exameModel => exameModel.DataExame <= pesquisaExame.DataFinal).ToList();
			}
			else if (pesquisaExame.DataFinal == DateTime.MinValue && pesquisaExame.DataInicial > DateTime.MinValue)
			{
				exames = exames.Where(exameModel => exameModel.DataExame >= pesquisaExame.DataInicial).ToList();
			}

			/* 
             * montando view model com o primeiro filtro
             */
			pesquisaExame.Exames = new List<ExameViewModel>();
			foreach (var exame in exames)
			{
				ExameViewModel ex = new ExameViewModel();
				ex.IdExame = exame.IdExame;
				ex.IdPaciente = _pessoaContext.GetById(exame.IdPaciente);
				ex.IdAgenteSaude = _pessoaContext.GetById(exame.IdAgenteSaude);
				ex.IdVirusBacteria = _virusBacteriaContext.GetById(exame.IdVirusBacteria);
				//ex.Resultado = Methods.GetStatusExame(Methods.GetResultadoExame(new ExameViewModel { Pcr = exame.Pcr, IgG = exame.IgG, IgM = exame.IgM }));
				ex.IgG = exame.IgG;
				ex.IgM = exame.IgM;
				ex.Pcr = exame.Pcr;
				ex.IgGIgM = exame.IgGIgM;
				ex.MetodoExame = exame.MetodoExame;
				ex.IdEstado = exame.IdEstado;
				ex.MunicipioId = exame.IdMunicipio;
				ex.DataInicioSintomas = exame.DataInicioSintomas;
				ex.DataExame = exame.DataExame;
				ex.IdEstado = exame.IdEstado;
				ex.MunicipioId = exame.IdMunicipio;
				ex.IdEmpresaSaude = exame.IdEmpresaSaude;
				ex.EhProfissionalSaude = exame.EhProfissionalSaude;
				ex.CodigoColeta = exame.CodigoColeta;
				ex.StatusNotificacao = exame.StatusNotificacao;
				ex.IdNotificacao = exame.IdNotificacao;
				ex.AguardandoResultado = exame.AguardandoResultado;
				ex.Coriza = exame.Coriza;
				ex.Diarreia = exame.Diarreia;
				ex.DificuldadeRespiratoria = exame.DificuldadeRespiratoria;
				ex.DorAbdominal = exame.DorAbdominal;
				ex.DorGarganta = exame.DorGarganta;
				ex.DorOuvido = exame.DorOuvido;
				ex.Febre = exame.Febre;
				ex.Nausea = exame.Nausea;
				ex.PerdaOlfatoPaladar = exame.PerdaOlfatoPaladar;
				ex.RelatouSintomas = exame.RelatouSintomas;
				ex.Tosse = exame.Tosse;

				pesquisaExame.Exames.Add(ex);
			}

			/*
             * 2º Filtro - filtrando ViewModel por nome ou cpf e resultado
             */
			pesquisaExame.Pesquisa = pesquisaExame.Pesquisa ?? "";
			pesquisaExame.Resultado = pesquisaExame.Resultado ?? "";
			pesquisaExame.Cidade = pesquisaExame.Cidade ?? "";

			if (!pesquisaExame.Pesquisa.Equals(""))
				if (Methods.SoContemLetras(pesquisaExame.Pesquisa))
					pesquisaExame.Exames = pesquisaExame.Exames.Where(exameViewModel => exameViewModel.IdPaciente.Nome.ToUpper().Contains(pesquisaExame.Pesquisa.ToUpper())).ToList();
				else
					pesquisaExame.Exames = pesquisaExame.Exames.Where(exameViewModel => exameViewModel.IdPaciente.Cpf.ToUpper().Contains(pesquisaExame.Pesquisa.ToUpper())).ToList();

			if (!pesquisaExame.Resultado.Equals("") && !pesquisaExame.Resultado.Equals("Todas as Opçoes"))
				pesquisaExame.Exames = pesquisaExame.Exames.Where(exameViewModel => exameViewModel.Resultado.ToUpper().Equals(pesquisaExame.Resultado.ToUpper())).ToList();

			if (!pesquisaExame.Cidade.Equals(""))
				pesquisaExame.Exames = pesquisaExame.Exames.Where(exameViewModel => exameViewModel.IdPaciente.Cidade.ToUpper().Contains(pesquisaExame.Cidade.ToUpper())).ToList();

			/* 
             * Ordenando lista por data e pegando maior e menor datas... 
             */
			if (pesquisaExame.Exames.Count > 0)
			{
				pesquisaExame.Exames = pesquisaExame.Exames.OrderByDescending(ex => ex.DataExame).ToList();
				pesquisaExame.DataFinal = pesquisaExame.Exames[0].DataExame;
				pesquisaExame.DataInicial = pesquisaExame.Exames[pesquisaExame.Exames.Count - 1].DataExame;
			}

			pesquisaExame.Exames = pesquisaExame.Exames.OrderBy(ex => ex.CodigoColeta).ToList();

			return PreencheTotalizadores(pesquisaExame);
		}

		public PessoaModel CreatePessoaModelByExame(ExameViewModel exame)
		{
			exame.IdPaciente.Cpf = Methods.RemoveSpecialsCaracts(exame.IdPaciente.Cpf.ToUpper());
			exame.IdPaciente.Cep = Methods.RemoveSpecialsCaracts(exame.IdPaciente.Cep);
			exame.IdPaciente.FoneCelular = Methods.RemoveSpecialsCaracts(exame.IdPaciente.FoneCelular);
			exame.IdPaciente.Sexo = exame.IdPaciente.Sexo.Equals("M") ? "Masculino" : "Feminino";

			if (exame.IdPaciente.FoneFixo != null)
				exame.IdPaciente.FoneFixo = Methods.RemoveSpecialsCaracts(exame.IdPaciente.FoneFixo);

			/* 
             * Só para garantir que a aplicação não irá quebrar
             * caso view retorne um id que ficou em cache... 
             */
			var user = _pessoaContext.GetByCpf(exame.IdPaciente.Cpf.ToUpper());
			if (user == null)
				exame.IdPaciente.Idpessoa = 0;
			else
			{
				exame.IdPaciente.Idpessoa = user.Idpessoa;

				if (exame.IdPaciente.SituacaoSaude.Equals(PessoaModel.SITUACAO_SAUDAVEL) && exame.Resultado.Equals(ExameModel.RESULTADO_POSITIVO))
				{
					exame.IdPaciente.SituacaoSaude = PessoaModel.SITUACAO_ISOLAMENTO;
					_pessoaContext.Update(exame.IdPaciente, false);
				}
			}

			return exame.IdPaciente;
		}

		public PesquisaExameViewModel PreencheTotalizadores(PesquisaExameViewModel examesTotalizados)
		{

			foreach (var item in examesTotalizados.Exames)
			{
				switch (item.Resultado)
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
