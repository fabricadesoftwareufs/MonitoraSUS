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
        private readonly IConfiguration _configuration;


        public MonitorarPacienteController(IVirusBacteriaService virusBacteriaContext,
                               IPessoaService pessoaContext,
                               IExameService exameContext,
                               IConfiguration configuration,
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
            _configuration = configuration;
        }

        /* O formulário só enviava os campos vazios.
         * Essa solução com a lista de parâmetros extensa é provisória.*/
        public IActionResult Index(DateTime DataInicial, DateTime DataFinal, string Pesquisa,
                                       string Resultado, int VirusBacteria, bool RealizouPesquisa)
        {
            var virus = _virusBacteriaContext.GetAll();
            virus.Insert(0, new VirusBacteriaModel { Nome = "Todas as Opções", IdVirusBacteria = 0 });
            ViewBag.VirusBacteria = new SelectList(virus, "IdVirusBacteria", "Nome");

            var pesquisa = new PesquisaPacienteViewModel
            {
                Pacientes = new List<MonitoraPacienteViewModel>(),
                Resultado = Resultado,
                DataFinal = DataFinal,
                DataInicial = DataInicial,
                Pesquisa = Pesquisa,
                VirusBacteria = VirusBacteria,
                RealizouPesquisa = RealizouPesquisa
            };

            return View(GetAllPacientesViewModel(pesquisa));
        }


        public IActionResult Edit(int idPaciente, int IdVirusBacteria)
        {
            ViewBag.googleKey = _configuration["GOOGLE_KEY"];
            return View(GetPacienteViewModel(idPaciente, IdVirusBacteria));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MonitoraPacienteViewModel paciente)
        {
            ViewBag.googleKey = _configuration["GOOGLE_KEY"];
            /*
             * Fazendo validações no cpf
             */
            paciente.Cpf = paciente.Cpf ?? "";
            if (Methods.SoContemNumeros(paciente.Cpf) && !paciente.Cpf.Equals(""))
            {
                if (!Methods.ValidarCpf(paciente.Cpf))
                {
                    TempData["resultadoPesquisa"] = "Esse esse cpf não é válido!";
                    return View(paciente);
                }
            }
            var usuarioDuplicado = _pessoaContext.GetByCpf(paciente.Cpf);
            if (usuarioDuplicado != null)
            {
                if (!(usuarioDuplicado.Idpessoa == paciente.Idpessoa))
                {
                    TempData["resultadoPesquisa"] = "Já existe outro paciente com esse CPF/RG, tente novamente!";
                    return View(paciente);
                }
            }

            try
            {
                UpdateSituacaoPessoaVirusBacteria(paciente);
            }
            catch
            {
                TempData["mensagemErro"] = "Houve um problema ao atualizar informações do paciente, por favor, tente novamente!";
                return View(paciente);
            }

            try
            {
                UpdatePaciente(paciente);
            }
            catch
            {
                TempData["mensagemErro"] = "Houve um problema ao atualizar informações do paciente, por favor, tente novamente!";
                return View(paciente);
            }

            TempData["mensagemSucesso"] = "Monitoramento realizado com sucesso!";
            return View(paciente);
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
                OutrasComorbidades = paciente.OutrasComorbidades,
            };

            return _pessoaContext.Update(pacienteModel) != null ? true : false;
        }

        public MonitoraPacienteViewModel GetPacienteViewModel(int idPaciente, int IdVirusBacteria)
        {
            var situacao = _situacaoPessoaContext.GetById(idPaciente, IdVirusBacteria);
            var pessoa = _pessoaContext.GetById(idPaciente);

            return new MonitoraPacienteViewModel
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
                OutrasComorbidades = pessoa.OutrasComorbidades,
                Descricao = situacao.Descricao,
                VirusBacteria = _virusBacteriaContext.GetById(situacao.IdVirusBacteria),
                ExamesPaciente = GetExamesPaciente(pessoa.Idpessoa),
                UltimaSituacao = GetUltimaSituacaoSaude(_situacaoPessoaContext.GetById(idPaciente, IdVirusBacteria).UltimaSituacaoSaude)
            };
        }


        public PesquisaPacienteViewModel GetAllPacientesViewModel(PesquisaPacienteViewModel pesquisa)
        {
            var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);
            var pessoaTrabalhaMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
            var pessoaTrabalhaEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);

            var pacientes = new List<PessoaModel>();
            if (usuario.RoleUsuario.Equals("GESTOR") || usuario.RoleUsuario.Equals("SECRETARIO"))
            {
                if (pessoaTrabalhaMunicipio != null)
                {
                    /*
                     * Filtrando por municipio depois pelo estado do municipio, 
                     * pois cidades com mesmo nome existem em diversos estados...
                     */
                    var municicpio = _municicpioContext.GetById(pessoaTrabalhaMunicipio.IdMunicipio);
                    var estado = _estadoContext.GetByCodUf(Convert.ToInt32(municicpio.Uf));
                    pacientes = _pessoaContext.GetByCidade(municicpio.Nome).Where(p => p.Estado.ToUpper().Equals(estado.Uf.ToUpper())).ToList();
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
                        pacientes = _pessoaContext.GetByEstado(_estadoContext.GetById(pessoaTrabalhaEstado.IdEstado).Uf);
                    }
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

                examesViewModel.Add(ex);
            }

            return examesViewModel;
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