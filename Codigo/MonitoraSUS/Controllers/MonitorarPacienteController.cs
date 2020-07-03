using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Model;
using Model.ViewModel;
using Util;
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
		private readonly IAreaAtuacaoService _areaAtuacaoContext;
		private readonly IUsuarioService _usuarioContext;
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
							   IEmpresaExameService empresaExameContext,
							   IAreaAtuacaoService areaAtuacaoContext,
							   IUsuarioService usuarioContext)
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
			_areaAtuacaoContext = areaAtuacaoContext;
			_usuarioContext = usuarioContext;
		}

		/* O formulário só enviava os campos vazios.
         * Essa solução com a lista de parâmetros extensa é provisória.*/
		public IActionResult Index(DateTime DataInicial, DateTime DataFinal, string Pesquisa,
									   string Resultado, int VirusBacteria)
		{
			var virus = _virusBacteriaContext.GetAll();
			ViewBag.VirusBacteria = new SelectList(virus, "IdVirusBacteria", "Nome");
			ViewBag.AreaAtuacao = new SelectList(_areaAtuacaoContext.GetAll(), "IdAreaAtuacao", "Descricao");
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
			ViewBag.Empresas = _empresaExameContext.GetHospitais();

			return View(GetPacienteViewModel(idPaciente, IdVirusBacteria));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(MonitoraPacienteViewModel monitoraPaciente)
		{
			ViewBag.googleKey = _configuration["GOOGLE_KEY"];
			ViewBag.Empresas = _empresaExameContext.GetAll();

			/*
             * Fazendo validações no cpf
             */
			monitoraPaciente.Paciente.Cpf = monitoraPaciente.Paciente.Cpf ?? "";
			if (Methods.SoContemNumeros(monitoraPaciente.Paciente.Cpf) && !monitoraPaciente.Paciente.Cpf.Equals(""))
			{
				if (!Methods.ValidarCpf(monitoraPaciente.Paciente.Cpf))
				{
					TempData["resultadoPesquisa"] = "Esse esse cpf não é válido!";
					return View(GetPacienteViewModel(monitoraPaciente.Paciente.Idpessoa, monitoraPaciente.VirusBacteria.IdVirusBacteria));
				}
			}
			var usuarioDuplicado = _pessoaContext.GetByCpf(monitoraPaciente.Paciente.Cpf);
			if (usuarioDuplicado != null)
			{
				if (!(usuarioDuplicado.Idpessoa == monitoraPaciente.Paciente.Idpessoa))
				{
					TempData["resultadoPesquisa"] = "Já existe outro paciente com esse CPF/RG, tente novamente!";
					return View(GetPacienteViewModel(monitoraPaciente.Paciente.Idpessoa, monitoraPaciente.VirusBacteria.IdVirusBacteria));
				}
			}

			try
			{
				UpdateSituacaoPessoaVirusBacteria(monitoraPaciente);
				UpdatePaciente(monitoraPaciente);
			}
			catch
			{
				TempData["mensagemErro"] = "Houve um problema ao atualizar informações do paciente, por favor, tente novamente!";
				return View(GetPacienteViewModel(monitoraPaciente.Paciente.Idpessoa, monitoraPaciente.VirusBacteria.IdVirusBacteria));
			}

			TempData["mensagemSucesso"] = "Monitoramento realizado com sucesso!";
			return View(GetPacienteViewModel(monitoraPaciente.Paciente.Idpessoa, monitoraPaciente.VirusBacteria.IdVirusBacteria));
		}


		public bool UpdateSituacaoPessoaVirusBacteria(MonitoraPacienteViewModel monitoraPaciente)
		{
			var usuario = _usuarioContext.RetornLoggedUser((ClaimsIdentity)User.Identity);

			var situacaoModel = _situacaoPessoaContext.GetById(monitoraPaciente.Paciente.Idpessoa, monitoraPaciente.VirusBacteria.IdVirusBacteria);
			situacaoModel.IdGestor = usuario.UsuarioModel.IdPessoa;
			situacaoModel.DataUltimoMonitoramento = DateTime.Now;
			situacaoModel.Descricao = monitoraPaciente.Descricao;
			return _situacaoPessoaContext.Update(situacaoModel);
		}

		public bool UpdatePaciente(MonitoraPacienteViewModel monitoraPaciente)
		{
			return _pessoaContext.Update(monitoraPaciente.Paciente, true) != null ? true : false;
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
				Paciente = pessoa,
				Descricao = situacao.Descricao,
				VirusBacteria = _virusBacteriaContext.GetById(situacao.IdVirusBacteria),
				ExamesPaciente = _exameContext.GetByIdPaciente(pessoa.Idpessoa),
				Internacoes = internacoes
			};
			monitora.UltimoResultado = GetUltimoResultadoExame(monitora.ExamesPaciente);
			return monitora;
		}


		public PesquisaPacienteViewModel GetAllPacientesViewModel(PesquisaPacienteViewModel pesquisa)
		{
			var usuario = _usuarioContext.RetornLoggedUser((ClaimsIdentity)User.Identity);
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
						var empresa = _empresaExameContext.GetById(pessoaTrabalhaEstado.IdEmpresaExame);
						if (empresa.FazMonitoramento)
						{
							pesquisa.Exames = _exameContext.GetByHospital(pessoaTrabalhaEstado.IdEmpresaExame, pesquisa.VirusBacteria, pesquisa.DataInicial, pesquisa.DataFinal).ToList();
						}
						else
						{
							TempData["mensagemAviso"] = "Essa Funcionalidade Não está Disponível Para Organizações Privadas!";
							return new PesquisaPacienteViewModel();
						}
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
					pesquisa.Exames = pesquisa.Exames.Where(paciente => paciente.Paciente.Nome.ToUpper().Contains(pesquisa.Pesquisa.ToUpper())).ToList();
				else
					pesquisa.Exames = pesquisa.Exames.Where(paciente => paciente.Paciente.Cpf.ToUpper().Contains(pesquisa.Pesquisa.ToUpper())).ToList();

			if (!pesquisa.Resultado.Equals("") && !pesquisa.Resultado.Equals("Todas as Opçoes"))
				pesquisa.Exames = pesquisa.Exames.Where(paciente => paciente.UltimoResultado.ToUpper().Equals(pesquisa.Resultado.ToUpper())).ToList();

			pesquisa.Exames = pesquisa.Exames.OrderByDescending(ex => ex.DataExame).ToList();
			return PreencheTotalizadores(pesquisa);
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
				switch (item.Paciente.SituacaoSaude)
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

		public string GetUltimoResultadoExame(List<ExameBuscaModel> listaExames)
		{
			var exame = listaExames.OrderByDescending(e => e.Exame.DataExame).FirstOrDefault();
			return exame.Exame.Resultado;
		}
	}
}