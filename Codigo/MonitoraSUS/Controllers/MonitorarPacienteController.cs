using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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



        public MonitorarPacienteController(IVirusBacteriaService virusBacteriaContext,
                               IPessoaService pessoaContext,
                               IExameService exameContext,
                               ISituacaoVirusBacteriaService situacaoPessoaContext,
                               IPessoaTrabalhaEstadoService pessoaTrabalhaEstado,
                               IPessoaTrabalhaMunicipioService pessoaTrabalhaMunicipioContext,
                               IMunicipioService municicpioContext,
                               IEstadoService estadoContext)
        {
            _virusBacteriaContext = virusBacteriaContext;
            _pessoaContext = pessoaContext;
            _situacaoPessoaContext = situacaoPessoaContext;
            _pessoaTrabalhaEstadoContext = pessoaTrabalhaEstado;
            _pessoaTrabalhaMunicipioContext = pessoaTrabalhaMunicipioContext;
            _exameContext = exameContext;
            _municicpioContext = municicpioContext;
            _estadoContext = estadoContext;
        }

        public IActionResult Index(PesquisaPacienteViewModel pesquisa)
        {
            var virus = _virusBacteriaContext.GetAll();
            virus.Add(new VirusBacteriaModel { Nome = "Todas as Opções", IdVirusBacteria = 0 });
            ViewBag.VirusBacteria = new SelectList(virus, "IdVirusBacteria", "Nome");

            return View(GetAllPacientesViewModel(pesquisa));
        }



        public PesquisaPacienteViewModel GetAllPacientesViewModel(PesquisaPacienteViewModel pesquisa)
        {
            var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);
            var pessoaTrabalhaMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
            var pessoaTrabalhaEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);

            var pacientes = new List<PessoaModel>();
            if (usuario.RoleUsuario.Equals("GESTOR"))
            {
                if (pessoaTrabalhaMunicipio != null)
                    pacientes = _pessoaContext.GetByCidade(_municicpioContext.GetById(pessoaTrabalhaMunicipio.IdMunicipio).Nome);

                if (pessoaTrabalhaEstado != null)
                {
                    if (pessoaTrabalhaEstado.IdEmpresaExame != EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
                        TempData["mensagemErro"] = "Essa funcionalidade está disponível apenas para Estados e Municípios.";
                    else
                        pacientes = _pessoaContext.GetByEstado(_estadoContext.GetById(pessoaTrabalhaEstado.IdEstado).Uf);
                }
            }

            pesquisa.Pacientes = new List<MonitoraPacienteViewModel>();
            foreach (var item in pacientes)
            {
                List<SituacaoPessoaVirusBacteriaModel> situacao = _situacaoPessoaContext.GetByIdPaciente(item.Idpessoa);
                if (situacao.Count > 0)
                {
                    foreach (var sit in situacao)
                    {
                        if (!sit.UltimaSituacaoSaude.Equals("N"))
                            pesquisa.Pacientes.Add(GetPaciente(item, sit));
                    }
                }
            }

            /*
             * Filrando por datas
             */
            if (pesquisa.DataInicial == DateTime.MinValue && pesquisa.DataFinal == DateTime.MinValue && !pesquisa.RealizouPesquisa)
            {
                pesquisa.DataInicial = DateTime.Now.AddDays(-7);
                pesquisa.DataFinal = DateTime.Now;
                pesquisa.Pacientes = pesquisa.Pacientes.Where(paciente => paciente.DataUltimoMonitoramento >= pesquisa.DataInicial && paciente.DataUltimoMonitoramento <= pesquisa.DataFinal).ToList();
            }
            else if (pesquisa.DataInicial > DateTime.MinValue && pesquisa.DataFinal > DateTime.MinValue)
            {
                pesquisa.Pacientes = pesquisa.Pacientes.Where(paciente => paciente.DataUltimoMonitoramento >= pesquisa.DataInicial && paciente.DataUltimoMonitoramento <= pesquisa.DataFinal).ToList();
            }
            else if (pesquisa.DataInicial == DateTime.MinValue && pesquisa.DataFinal > DateTime.MinValue)
            {
                pesquisa.Pacientes = pesquisa.Pacientes.Where(paciente => paciente.DataUltimoMonitoramento <= pesquisa.DataFinal).ToList();
            }
            else if (pesquisa.DataFinal == DateTime.MinValue && pesquisa.DataInicial > DateTime.MinValue)
            {
                pesquisa.Pacientes = pesquisa.Pacientes.Where(paciente => paciente.DataUltimoMonitoramento >= pesquisa.DataInicial).ToList();
            }

            /*
             * 2º Filtro - filtrando ViewModel por nome/cpf, resultado e exame
             */
            pesquisa.Pesquisa = pesquisa.Pesquisa ?? "";
            pesquisa.Resultado = pesquisa.Resultado ?? "";

            if (!pesquisa.Pesquisa.Equals(""))
                if (Methods.SoContemLetras(pesquisa.Pesquisa))
                    pesquisa.Pacientes = pesquisa.Pacientes.Where(paciente => paciente.Nome.ToUpper().Contains(pesquisa.Pesquisa.ToUpper())).ToList();
                else
                    pesquisa.Pacientes = pesquisa.Pacientes.Where(paciente => paciente.Cpf.ToUpper().Contains(pesquisa.Pesquisa.ToUpper())).ToList();

            if (!pesquisa.Resultado.Equals("") && !pesquisa.Resultado.Equals("Todas as Opçoes"))
                pesquisa.Pacientes = pesquisa.Pacientes.Where(paciente => paciente.UltimaSituacao.ToUpper().Equals(pesquisa.Resultado.ToUpper())).ToList();

            if (pesquisa.VirusBacteria != 0)
                pesquisa.Pacientes = pesquisa.Pacientes.Where(paciente => paciente.VirusBacteria.IdVirusBacteria == pesquisa.VirusBacteria).ToList();
            /* 
             * Ordenando lista por data e pegando maior e menor datas... 
             */
            if (pesquisa.Pacientes.Count > 0)
            {
                pesquisa.Pacientes = pesquisa.Pacientes.OrderByDescending(ex => ex.DataUltimoMonitoramento).ToList();
                pesquisa.DataFinal = pesquisa.Pacientes[0].DataUltimoMonitoramento.Value;
                pesquisa.DataInicial = pesquisa.Pacientes[pesquisa.Pacientes.Count - 1].DataUltimoMonitoramento.Value;
            }

            return PreencheTotalizadores(pesquisa);
        }

        public MonitoraPacienteViewModel GetPaciente(PessoaModel pessoa, SituacaoPessoaVirusBacteriaModel situacao)
        {
            MonitoraPacienteViewModel pc = new MonitoraPacienteViewModel();

            pc.Idpessoa = pessoa.Idpessoa;
            pc.Nome = pessoa.Nome;
            pc.Cpf = pessoa.Cpf;
            pc.UltimaSituacao = GetUltimaSituacaoSaude(situacao.UltimaSituacaoSaude);
            pc.DataUltimoMonitoramento = situacao.DataUltimoMonitoramento;
            pc.VirusBacteria = _virusBacteriaContext.GetById(situacao.IdVirusBacteria);

            if (situacao.IdGestor.HasValue && situacao.IdGestor != 0)
                pc.Gestor = _pessoaContext.GetById(situacao.IdGestor.Value);
            else
                pc.Gestor = new PessoaModel { Nome = " - " };

            return pc;
        }

        public PesquisaPacienteViewModel PreencheTotalizadores(PesquisaPacienteViewModel pacientesTotalizados)
        {

            foreach (var item in pacientesTotalizados.Pacientes)
            {
                switch (item.UltimaSituacao)
                {
                    case ExameModel.RESULTADO_POSITIVO: pacientesTotalizados.Positivos++; break;
                    case ExameModel.RESULTADO_INDETERMINADO: pacientesTotalizados.Indeterminados++; break;
                    case ExameModel.RESULTADO_IMUNIZADO: pacientesTotalizados.Imunizados++; break;
                }
            }


            return pacientesTotalizados;
        }

        public string GetUltimaSituacaoSaude(string situacao)
        {
            switch (situacao)
            {
                case "P":
                    return "Positivo";
                case "C":
                    return "Imunizado";
                case "I":
                    return "Indeterminado";

                default: return "Indeterminado";
            }
        }
    }
}