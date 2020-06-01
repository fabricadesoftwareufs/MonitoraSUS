using Microsoft.AspNetCore.Authorization;
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

namespace MonitoraSUS.Controllers
{
	[Authorize(Roles = "GESTOR, SECRETARIO")]
	public class MonitorarPacienteController : Controller
	{
		private readonly IVirusBacteriaService _virusBacteriaContext;
		private readonly IPessoaService _pessoaContext;
		private readonly ISituacaoVirusBacteriaService _situacaoPessoaContext;
		private readonly IPessoaTrabalhaEstadoService _pessoaTrabalhaEstadoContext;
		private readonly IPessoaTrabalhaMunicipioService _pessoaTrabalhaMunicipioContext;
		private readonly IExameService _exameContext;
		private readonly IMunicipioService _municicpioContext;
		private readonly IEstadoService _estadoContext;
		private readonly IEmpresaExameService _empresaExameContext;
		private readonly IInternacaoService _internacaoContext;
		private readonly IConfiguration _configuration;


		public MonitorarPacienteController(IVirusBacteriaService virusBacteriaContext,
							   IPessoaService pessoaContext,
							   IExameService exameContext,
							   IConfiguration configuration,
							   ISituacaoVirusBacteriaService situacaoPessoaContext,
							   IPessoaTrabalhaEstadoService pessoaTrabalhaEstado,
							   IPessoaTrabalhaMunicipioService pessoaTrabalhaMunicipioContext,
							   IMunicipioService municicpioContext,
							   IEstadoService estadoContext,
							   IInternacaoService internacaoContext,
							   IEmpresaExameService empresaExameContext)
		{
			_virusBacteriaContext = virusBacteriaContext;
			_pessoaContext = pessoaContext;
			_situacaoPessoaContext = situacaoPessoaContext;
			_pessoaTrabalhaEstadoContext = pessoaTrabalhaEstado;
			_pessoaTrabalhaMunicipioContext = pessoaTrabalhaMunicipioContext;
			_exameContext = exameContext;
			_municicpioContext = municicpioContext;
			_estadoContext = estadoContext;
			_internacaoContext = internacaoContext;
			_configuration = configuration;
			_empresaExameContext = empresaExameContext;
		}

		/* O formulário só enviava os campos vazios.
         * Essa solução com a lista de parâmetros extensa é provisória.*/
		public IActionResult Index(DateTime DataInicial, DateTime DataFinal, string Pesquisa,
									   string Resultado, int VirusBacteria)
		{
			var virus = _virusBacteriaContext.GetAll();
			ViewBag.VirusBacteria = new SelectList(virus, "IdVirusBacteria", "Nome");
			if (VirusBacteria == 0)
				VirusBacteria = virus.First().IdVirusBacteria;
			int diasRecuperacao = virus.Where(v => v.IdVirusBacteria == VirusBacteria).First().DiasRecuperacao;

			var pesquisa = new PesquisaPacienteViewModel
			{
				Exames = new List<MonitoraPacienteViewModel>(),
				Resultado = Resultado,
				DataFinal = DataFinal.Equals(DateTime.MinValue) ? DateTime.Now : DataFinal,
				DataInicial = DataInicial.Equals(DateTime.MinValue) ? DateTime.Now.AddDays(-diasRecuperacao) : DataInicial,
				Pesquisa = Pesquisa,
				VirusBacteria = VirusBacteria,
			};

			return View(GetAllPacientesViewModel(pesquisa));
		}


		public IActionResult Edit(int idPaciente, int IdVirusBacteria)
		{
			ViewBag.googleKey = _configuration["GOOGLE_KEY"];
			ViewBag.Empresas = _empresaExameContext.GetAll();

			return View(GetPacienteViewModel(idPaciente, IdVirusBacteria));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(MonitoraPacienteViewModel paciente)
		{
			ViewBag.googleKey = _configuration["GOOGLE_KEY"];
			ViewBag.Empresas = _empresaExameContext.GetAll();

			/*
             * Fazendo validações no cpf
             */
			paciente.Cpf = paciente.Cpf ?? "";
			if (Methods.SoContemNumeros(paciente.Cpf) && !paciente.Cpf.Equals(""))
			{
				if (!Methods.ValidarCpf(paciente.Cpf))
				{
					TempData["resultadoPesquisa"] = "Esse esse cpf não é válido!";
					return View(GetPacienteViewModel(paciente.Idpessoa, paciente.VirusBacteria.IdVirusBacteria));
				}
			}
			var usuarioDuplicado = _pessoaContext.GetByCpf(paciente.Cpf);
			if (usuarioDuplicado != null)
			{
				if (!(usuarioDuplicado.Idpessoa == paciente.Idpessoa))
				{
					TempData["resultadoPesquisa"] = "Já existe outro paciente com esse CPF/RG, tente novamente!";
					return View(GetPacienteViewModel(paciente.Idpessoa, paciente.VirusBacteria.IdVirusBacteria));
				}
			}

			try
			{
				UpdateSituacaoPessoaVirusBacteria(paciente);
			}
			catch (Exception e)
			{
				TempData["mensagemErro"] = "Houve um problema ao atualizar informações do paciente, por favor, tente novamente!";
				return View(GetPacienteViewModel(paciente.Idpessoa, paciente.VirusBacteria.IdVirusBacteria));
			}

			try
			{
				UpdatePaciente(GetPacienteViewModel(paciente.Idpessoa, paciente.VirusBacteria.IdVirusBacteria));
			}
			catch
			{
				TempData["mensagemErro"] = "Houve um problema ao atualizar informações do paciente, por favor, tente novamente!";
				return View(GetPacienteViewModel(paciente.Idpessoa, paciente.VirusBacteria.IdVirusBacteria));
			}

			TempData["mensagemSucesso"] = "Monitoramento realizado com sucesso!";
			return View(GetPacienteViewModel(paciente.Idpessoa, paciente.VirusBacteria.IdVirusBacteria));
		}


		public bool UpdateSituacaoPessoaVirusBacteria(MonitoraPacienteViewModel paciente)
		{
			var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);

			var situacaoModel = _situacaoPessoaContext.GetById(paciente.Idpessoa, paciente.VirusBacteria.IdVirusBacteria);
			situacaoModel.IdGestor = usuario.UsuarioModel.IdPessoa;
			situacaoModel.DataUltimoMonitoramento = DateTime.Now;
			situacaoModel.Descricao = paciente.Descricao;
			return _situacaoPessoaContext.Update(situacaoModel);
		}

		public bool UpdatePaciente(MonitoraPacienteViewModel paciente)
		{
			var pacienteModel = new PessoaModel
			{
				Idpessoa = paciente.Idpessoa,
				Nome = paciente.Nome,
				Cpf = Methods.RemoveSpecialsCaracts(paciente.Cpf),
				DataNascimento = paciente.DataNascimento,
				Sexo = paciente.Sexo,
				Cep = Methods.RemoveSpecialsCaracts(paciente.Cep),
				Rua = paciente.Rua,
				Numero = paciente.Numero,
				Bairro = paciente.Bairro,
				Cidade = paciente.Cidade,
				Estado = paciente.Estado,
				Latitude = paciente.Latitude,
				Longitude = paciente.Longitude,
				Complemento = paciente.Complemento,
				FoneFixo = paciente.FoneFixo != null ? Methods.RemoveSpecialsCaracts(paciente.FoneFixo) : "",
				FoneCelular = Methods.RemoveSpecialsCaracts(paciente.FoneCelular),
				Email = paciente.Email,
				Cancer = paciente.Cancer,
				Diabetes = paciente.Diabetes,
				Hipertenso = paciente.Hipertenso,
				Imunodeprimido = paciente.Imunodeprimido,
				Cardiopatia = paciente.Cardiopatia,
				Obeso = paciente.Obeso,
				DoencaRespiratoria = paciente.DoencaRespiratoria,
				DoencaRenal = paciente.DoencaRenal,
				Epilepsia = paciente.Epilepsia,
				OutrasComorbidades = paciente.OutrasComorbidades,
				SituacaoSaude = paciente.SituacaoSaude,
				Coriza = paciente.Coriza,
				DataObito = paciente.DataObito,
				Diarreia = paciente.Diarreia,
				DificuldadeRespiratoria = paciente.DificuldadeRespiratoria,
				DorGarganta = paciente.DorGarganta,
				DorOuvido = paciente.DorOuvido,
				Febre = paciente.Febre,
				Nausea = paciente.Nausea,
				PerdaOlfatoPaladar = paciente.PerdaOlfatoPaladar,
				Tosse = paciente.Tosse,
				DorAbdominal = paciente.DorAbdominal
			};

			return _pessoaContext.Update(pacienteModel, true) != null ? true : false;
		}

		public MonitoraPacienteViewModel GetPacienteViewModel(int idPaciente, int IdVirusBacteria)
		{
			var situacao = _situacaoPessoaContext.GetById(idPaciente, IdVirusBacteria);
			var pessoa = _pessoaContext.GetById(idPaciente);
			
			var internacoes  = _internacaoContext.GetByIdPaciente(pessoa.Idpessoa);
			for (int i = 0; i < internacoes.Count; i++)
			{
				var empresa = _empresaExameContext.GetById(internacoes[i].IdEmpresa);
				internacoes[i].NomeEmpresa = empresa.Nome;
				internacoes[i].IdEmpresa = empresa.Id;
			}

			var monitora = new MonitoraPacienteViewModel
			{
				Idpessoa = pessoa.Idpessoa,
				Nome = pessoa.Nome,
				Cpf = pessoa.Cpf,
				DataNascimento = pessoa.DataNascimento,
				Sexo = pessoa.Sexo,
				Cep = pessoa.Cep,
				Rua = pessoa.Rua,
				Numero = pessoa.Numero,
				Bairro = pessoa.Bairro,
				Cidade = pessoa.Cidade,
				Estado = pessoa.Estado,
				Latitude = pessoa.Latitude,
				Longitude = pessoa.Longitude,
				Complemento = pessoa.Complemento,
				FoneCelular = pessoa.FoneCelular,
				FoneFixo = pessoa.FoneFixo,
				Email = pessoa.Email,
				Cancer = pessoa.Cancer,
				Diabetes = pessoa.Diabetes,
				Hipertenso = pessoa.Hipertenso,
				Imunodeprimido = pessoa.Imunodeprimido,
				Cardiopatia = pessoa.Cardiopatia,
				Obeso = pessoa.Obeso,
				DoencaRespiratoria = pessoa.DoencaRespiratoria,
				DoencaRenal = pessoa.DoencaRenal,
				Epilepsia = pessoa.Epilepsia,
				OutrasComorbidades = pessoa.OutrasComorbidades,
				Descricao = situacao.Descricao,
				SituacaoSaude = pessoa.SituacaoSaude,
				VirusBacteria = _virusBacteriaContext.GetById(situacao.IdVirusBacteria),
				ExamesPaciente = GetExamesPaciente(pessoa.Idpessoa),
				Internacoes = internacoes,
				Coriza = pessoa.Coriza,
				DataObito = pessoa.DataObito,
				Diarreia = pessoa.Diarreia,
				DificuldadeRespiratoria = pessoa.DificuldadeRespiratoria,
				DorGarganta = pessoa.DorGarganta,
				DorOuvido = pessoa.DorOuvido,
				Febre = pessoa.Febre,
				Nausea = pessoa.Nausea,
				PerdaOlfatoPaladar = pessoa.PerdaOlfatoPaladar,
				Tosse = pessoa.Tosse,
				DorAbdominal = pessoa.DorAbdominal		
			};
			monitora.UltimoResultado = GetUltimoResultadoExame(monitora.ExamesPaciente);
			return monitora;
		}


		public PesquisaPacienteViewModel GetAllPacientesViewModel(PesquisaPacienteViewModel pesquisa)
		{
			var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);
			var pessoaTrabalhaMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
			var pessoaTrabalhaEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);

			if (usuario.RoleUsuario.Equals("GESTOR") || usuario.RoleUsuario.Equals("SECRETARIO"))
			{
				if (pessoaTrabalhaMunicipio != null)
				{
					var municicpio = _municicpioContext.GetById(pessoaTrabalhaMunicipio.IdMunicipio);
					var estado = _estadoContext.GetByCodUf(Convert.ToInt32(municicpio.Uf));
					pesquisa.Exames = _exameContext.GetByCidadeResidenciaPaciente(municicpio.Nome, estado.Uf.ToUpper(), pesquisa.VirusBacteria, pesquisa.DataInicial, pesquisa.DataFinal).ToList();
				}
				if (pessoaTrabalhaEstado != null)
				{
					if (pessoaTrabalhaEstado.IdEmpresaExame != EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
					{
						TempData["mensagemAviso"] = "Essa Funcionalidade Não está Disponível Para Organizações Privadas!";
						return new PesquisaPacienteViewModel();
					}
					else
					{
						var estado = _estadoContext.GetById(pessoaTrabalhaEstado.IdEstado);
						pesquisa.Exames = _exameContext.GetByEstadoResidenciaPaciente(estado.Uf.ToUpper(), pesquisa.VirusBacteria, pesquisa.DataInicial, pesquisa.DataFinal).ToList();
					}
				}
			}

			/*
             * 2º Filtro - filtrando ViewModel por nome/cpf, resultado e exame
             */
			pesquisa.Pesquisa = pesquisa.Pesquisa ?? "";
			pesquisa.Resultado = pesquisa.Resultado ?? "";

			if (!pesquisa.Pesquisa.Equals(""))
				if (Methods.SoContemLetras(pesquisa.Pesquisa))
					pesquisa.Exames = pesquisa.Exames.Where(paciente => paciente.Nome.ToUpper().Contains(pesquisa.Pesquisa.ToUpper())).ToList();
				else
					pesquisa.Exames = pesquisa.Exames.Where(paciente => paciente.Cpf.ToUpper().Contains(pesquisa.Pesquisa.ToUpper())).ToList();

			if (!pesquisa.Resultado.Equals("") && !pesquisa.Resultado.Equals("Todas as Opçoes"))
				pesquisa.Exames = pesquisa.Exames.Where(paciente => paciente.UltimoResultado.ToUpper().Equals(pesquisa.Resultado.ToUpper())).ToList();

			pesquisa.Exames = pesquisa.Exames.OrderByDescending(ex => ex.DataExame).ToList();
			return PreencheTotalizadores(pesquisa);
		}

		public List<ExameViewModel> GetExamesPaciente(int idPaciente)
		{
			var examesViewModel = new List<ExameViewModel>();
			var exames = _exameContext.GetByIdPaciente(idPaciente);

			foreach (var exame in exames)
			{
				ExameViewModel ex = new ExameViewModel();
				ex.IdExame = exame.IdExame;
				ex.IdPaciente = _pessoaContext.GetById(exame.IdPaciente);
				ex.IdAgenteSaude = _pessoaContext.GetById(exame.IdAgenteSaude);
				ex.IdVirusBacteria = _virusBacteriaContext.GetById(exame.IdVirusBacteria);
				ex.IgG = exame.IgG;
				ex.IgM = exame.IgM;
				ex.Pcr = exame.Pcr;
				ex.IgGIgM = exame.IgGIgM;
				ex.MetodoExame = exame.MetodoExame;
				ex.RelatouSintomas = exame.RelatouSintomas;
				ex.AguardandoResultado = exame.AguardandoResultado;
				ex.IdEstado = exame.IdEstado;
				ex.MunicipioId = exame.IdMunicipio;
				ex.DataInicioSintomas = exame.DataInicioSintomas;
				ex.DataExame = exame.DataExame;
				ex.IdEstado = exame.IdEstado;
				ex.IdEmpresaSaude = exame.IdEmpresaSaude;
				ex.EhProfissionalSaude = exame.EhProfissionalSaude;
				ex.CodigoColeta = exame.CodigoColeta;
				ex.StatusNotificacao = exame.StatusNotificacao;
				ex.IdNotificacao = exame.IdNotificacao;
				if ((ex.IdEmpresaSaude != null) && (ex.IdEmpresaSaude != EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO))
					ex.ResponsavelRealizacaoExame = _empresaExameContext.GetById((int) ex.IdEmpresaSaude).Nome + " - ";
				else if ((ex.MunicipioId != null))
					ex.ResponsavelRealizacaoExame = _municicpioContext.GetById((int)ex.MunicipioId).Nome + " - ";
				ex.ResponsavelRealizacaoExame += _estadoContext.GetById(ex.IdEstado).Nome;
				examesViewModel.Add(ex);
			}
			return examesViewModel;
		}

		public PesquisaPacienteViewModel PreencheTotalizadores(PesquisaPacienteViewModel pacientesTotalizados)
		{

			foreach (var item in pacientesTotalizados.Exames)
			{
				switch (item.UltimoResultado)
				{
					case ExameModel.RESULTADO_POSITIVO: pacientesTotalizados.Positivos++; break;
					case ExameModel.RESULTADO_INDETERMINADO: pacientesTotalizados.Indeterminados++; break;
					case ExameModel.RESULTADO_RECUPERADO: pacientesTotalizados.Recuperados++; break;
					case ExameModel.RESULTADO_AGUARDANDO: pacientesTotalizados.Aguardando++; break;
					case ExameModel.RESULTADO_IGMIGG: pacientesTotalizados.IgGIgM++; break;
				}
				switch (item.SituacaoSaude)
				{
					case PessoaModel.SITUACAO_ISOLAMENTO: pacientesTotalizados.Isolamento++; break;
					case PessoaModel.SITUACAO_HOSPITALIZADO_INTERNAMENTO: pacientesTotalizados.Hospitalizado++; break;
					case PessoaModel.SITUACAO_UTI: pacientesTotalizados.UTI++; break;
					case PessoaModel.SITUACAO_SAUDAVEL: pacientesTotalizados.Saudavel++; break;
					case PessoaModel.SITUACAO_ESTABILIZACAO: pacientesTotalizados.Estabilizacao++; break;
					case PessoaModel.SITUACAO_OBITO: pacientesTotalizados.Obito++; break;
				}
			}
			return pacientesTotalizados;
		}

		public string GetUltimoResultadoExame(List<ExameViewModel> listaExames)
		{
			ExameViewModel exame = listaExames.OrderByDescending(e => e.DataExame).FirstOrDefault();
			return exame.Resultado;
		}
	}
}