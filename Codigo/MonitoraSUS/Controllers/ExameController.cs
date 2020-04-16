using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Model;
using MonitoraSUS.Resources.Methods;
using Service;
using Service.Interface;

namespace MonitoraSUS.Controllers
{
    public class ExameController : Controller
    {
        private readonly IVirusBacteriaService _virusBacteriaContext;
        private readonly IExameService _exameContext;
        private readonly IPessoaService _pessoaContext;
        private readonly IMunicipioService _municicpioContext;
        private readonly IEstadoService _estadoContext;
        private readonly ISituacaoVirusBacteriaService _situacaoPessoaContext;
        private readonly IPessoaTrabalhaEstadoService _pessoaTrabalhaEstadoContext;
        private readonly IEmpresaExameService _empresaExameContext;


        public ExameController(IVirusBacteriaService virusBacteriaContext,
                               IExameService exameContext,
                               IPessoaService pessoaContext,
                               IMunicipioService municicpioContext,
                               IEstadoService estadoContext,
                               ISituacaoVirusBacteriaService situacaoPessoaContext,
                               IPessoaTrabalhaEstadoService pessoaTrabalhaEstado)
        {
            _virusBacteriaContext = virusBacteriaContext;
            _exameContext = exameContext;
            _pessoaContext = pessoaContext;
            _municicpioContext = municicpioContext;
            _estadoContext = estadoContext;
            _situacaoPessoaContext = situacaoPessoaContext;
            _pessoaTrabalhaEstadoContext = pessoaTrabalhaEstado;
        }

        public IActionResult Index()
        {
            return View(GetAllExamesViewModel());
        }

        public IActionResult Details(int id)
        {
            
            return View(GetExameViewModelById(id));
        }


        public IActionResult Edit(int id)
        {
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

            try
            {
                _exameContext.Update(CreateExameModel(exame,false));
                _situacaoPessoaContext.Update(CreateSituacaoPessoaModelByExame(exame, _situacaoPessoaContext.GetById(exame.IdPaciente.Idpessoa, exame.IdVirusBacteria.IdVirusBacteria)));
                _pessoaContext.Update(CreatePessoaModelByExame(exame));

                TempData["mensagemSucesso"] = "Notificação realizada com SUCESSO!";

                return View(new ExameViewModel());

            }
            catch
            {
                TempData["mensagemErro"] = "Houve um problema ao atualizar as informações, tente novamente ou " +
                  "entre em contato com os desenvolvedores";

                return View(exame);
            }
        }



        public IActionResult Create()
        {
            ViewBag.VirusBacteria = new SelectList(_virusBacteriaContext.GetAll(), "IdVirusBacteria", "Nome");
            return View(new ExameViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ExameViewModel exame)
        {
            ViewBag.VirusBacteria = new SelectList(_virusBacteriaContext.GetAll(), "IdVirusBacteria", "Nome");

            if (exame.PesquisarCpf == 1) // pesquisar usuario por cpf 
            {
                var cpf = RemoveSpecialsCaracts(exame.IdPaciente.Cpf); // cpf sem caracteres especiais

                var pessoa = _pessoaContext.GetByCpf(cpf);

                if (pessoa != null)
                    exame.IdPaciente = pessoa;
                else
                    TempData["resultadoPesquisa"] = "Paciente não cadastrado, preencha os campos para cadastra-lo!";


                return View(exame);
            }
            else
            {

                try
                {
                    var pessoa = CreatePessoaModelByExame(exame);
                    // inserindo ou atualizando o paciente
                    if (_pessoaContext.GetByCpf(pessoa.Cpf) == null)
                        _pessoaContext.Insert(pessoa);
                    else
                        _pessoaContext.Update(pessoa);
                }
                catch
                {
                    TempData["mensagemErro"] = "Cadastro não pode ser concluido pois houve um problema ao inserir/atualizar dados do paciente, " +
                                               "verifique as informações e tente novamente ou entre em contato com os desenvolvedores.";

                    return View(exame);
                }


                try
                {
                    // inserindo o resultado do exame (situacao da pessoa)                  
                    var cpf = RemoveSpecialsCaracts(exame.IdPaciente.Cpf);
                    var idPessoa = _pessoaContext.GetByCpf(cpf).Idpessoa;
                    var situacaoPessoa = _situacaoPessoaContext.GetById(idPessoa,exame.IdVirusBacteria.IdVirusBacteria);
                    
                    if (situacaoPessoa == null)
                        _situacaoPessoaContext.Insert(CreateSituacaoPessoaModelByExame(exame, situacaoPessoa)); 
                    else
                        _situacaoPessoaContext.Update(CreateSituacaoPessoaModelByExame(exame, situacaoPessoa));
                }
                catch
                {
                    TempData["mensagemErro"] = "Cadastro não pode ser concluido pois houve um problema ao inserir/atualizar o resultado do exame, " +
                                               "verifique as informações e tente novamente ou entre em contato com os desenvolvedores.";
                    return View(exame);
                }

                try
                {
                    // inserindo o exame
                    _exameContext.Insert(CreateExameModel(exame,true));
                }
                catch
                {
                    TempData["mensagemErro"] = "Cadastro não pode ser concluido pois houve um problema ao inserir os dados do exame, " +
                                               "verifique as informações e tente novamente ou entre em contato com os desenvolvedores.";

                    return View(exame);
                }

                // codigo para realizar notificacao 

                TempData["mensagemSucesso"] = "Notificação realizada com SUCESSO!";

                return View(new ExameViewModel());
            }
        }

        public SituacaoPessoaVirusBacteriaModel CreateSituacaoPessoaModelByExame(ExameViewModel exame, SituacaoPessoaVirusBacteriaModel situacao)
        {

            if (situacao != null)
            {
                situacao.UltimaSituacaoSaude = GetResultadoExame(exame);
            }
            else
            {
                situacao = new SituacaoPessoaVirusBacteriaModel();

                situacao.IdVirusBacteria = exame.IdVirusBacteria.IdVirusBacteria;
                situacao.Idpessoa = _pessoaContext.GetByCpf(RemoveSpecialsCaracts(exame.IdPaciente.Cpf)).Idpessoa;
                situacao.UltimaSituacaoSaude = GetResultadoExame(exame);
            }

            return situacao;
        }

        public ExameModel CreateExameModel(ExameViewModel viewModel, bool atualizarCidadeEstado)
        {
            ExameModel exame = new ExameModel();

            exame.IdExame = viewModel.IdExame;
            exame.IdPaciente = _pessoaContext.GetByCpf(RemoveSpecialsCaracts(viewModel.IdPaciente.Cpf)).Idpessoa;
            exame.IdVirusBacteria = viewModel.IdVirusBacteria.IdVirusBacteria;
            exame.IgG = viewModel.IgG;
            exame.IgM = viewModel.IgM;
            exame.Pcr = viewModel.Pcr;
            exame.IdEstado = viewModel.IdEstado;
            exame.IdMunicipio = viewModel.MunicipioId;
            exame.DataInicioSintomas = viewModel.DataInicioSintomas;
            exame.DataExame = viewModel.DataExame;
            exame.IdAgenteSaude = viewModel.IdAgenteSaude.Idpessoa;
            exame.IdEmpresaSaude = viewModel.IdEmpresaSaude;

            // pegando informações do agente de saúde logado no sistema
            // var agente = _pessoaContext.GetById(MethodsUtils.RetornLoggedUser((ClaimsIdentity)User.Identity).IdPessoa);
            var agente = _pessoaContext.GetById(5);
            
            if (atualizarCidadeEstado)
            {
                exame.IdEstado = _estadoContext.GetByName(agente.Estado).Id;
                exame.IdMunicipio = _municicpioContext.GetByName(agente.Cidade).Id;
                
                var pessoaEstado =  _pessoaTrabalhaEstadoContext.GetSecretarioAtivoByIdPessoa(agente.Idpessoa);
                if (pessoaEstado != null)
                    exame.IdEmpresaSaude = pessoaEstado.IdEmpresaExame;
            }

            exame.IdAgenteSaude = agente.Idpessoa;

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
            ex.Resultado = GetStatusExame(GetResultadoExame(new ExameViewModel { Pcr = exame.Pcr, IgG = exame.IgG, IgM = exame.IgM }));
            ex.IgG = exame.IgG;
            ex.IgM = exame.IgM;
            ex.Pcr = exame.Pcr;
            ex.IdEstado = exame.IdEstado;
            ex.MunicipioId = exame.IdMunicipio;
            ex.DataInicioSintomas = exame.DataInicioSintomas;
            ex.DataExame = exame.DataExame;
           


            return ex;
        }

        public List<ExameViewModel> GetAllExamesViewModel()
        {
            /*Pegando usuario logado*/
            // var usuario = MethodsUtils.RetornLoggedUser((ClaimsIdentity)User.Identity);
            var usuario = new UsuarioModel {IdUsuario = 0, IdPessoa = 5,TipoUsuario = 2};
            
            var exames = new List<ExameModel>();
            if (usuario.TipoUsuario == 1 || usuario.TipoUsuario == 3) 
            {
                exames = _exameContext.GetByIdAgente(usuario.IdPessoa);
            } 
            else if (usuario.TipoUsuario == 2) 
            {
                var secretario = _pessoaTrabalhaEstadoContext.GetSecretarioAtivoByIdPessoa(usuario.IdPessoa);
                
                if (secretario != null)
                    exames = _exameContext.GetByIdEstado(secretario.IdEstado);
            }

            var examesViewModel = new List<ExameViewModel>();

            foreach (var exame in exames)
            {
                ExameViewModel ex = new ExameViewModel();
                ex.IdExame = exame.IdExame;
                ex.IdPaciente = _pessoaContext.GetById(exame.IdPaciente);
                ex.IdAgenteSaude = _pessoaContext.GetById(exame.IdAgenteSaude);
                ex.IdVirusBacteria = _virusBacteriaContext.GetById(exame.IdVirusBacteria);
                ex.Resultado = GetStatusExame(GetResultadoExame(new ExameViewModel { Pcr = exame.Pcr, IgG = exame.IgG, IgM = exame.IgM}));
                ex.IgG = exame.IgG;
                ex.IgM = exame.IgM;
                ex.Pcr = exame.Pcr;
                ex.IdEstado = exame.IdEstado;
                ex.MunicipioId = exame.IdMunicipio;
                ex.DataInicioSintomas = exame.DataInicioSintomas;
                ex.DataExame = exame.DataExame;
                ex.IdEstado = exame.IdEstado;
                ex.MunicipioId = exame.IdMunicipio;

                examesViewModel.Add(ex);
            }

            return examesViewModel;
        }

        public PessoaModel CreatePessoaModelByExame(ExameViewModel exame)
        {


            exame.IdPaciente.Cpf = RemoveSpecialsCaracts(exame.IdPaciente.Cpf);
            exame.IdPaciente.Cep = RemoveSpecialsCaracts(exame.IdPaciente.Cep);
            exame.IdPaciente.FoneCelular = RemoveSpecialsCaracts(exame.IdPaciente.FoneCelular);

            if (exame.IdPaciente.FoneFixo != null)
                exame.IdPaciente.FoneFixo = RemoveSpecialsCaracts(exame.IdPaciente.FoneFixo);
            
            var paciente = _pessoaContext.GetByCpf(exame.IdPaciente.Cpf);
            if (paciente != null)
                exame.IdPaciente.Idpessoa = paciente.Idpessoa;
            
            return exame.IdPaciente;
        }

        public static string RemoveSpecialsCaracts(string poluatedString)
            => Regex.Replace(poluatedString, "[^0-9a-zA-Z]+", "");

        public string GetResultadoExame(ExameViewModel exame)
        {

            string resultado = "I";

            if (exame.Pcr.Equals("S") || exame.IgM.Equals("S"))
            {
                resultado = "P";
            }
            else if (exame.Pcr.Equals("I") || exame.IgM.Equals("I")) {
                resultado = "I";
            }
            else if (exame.IgG.Equals("S"))
            {
                resultado = "C";
            }
            else if (exame.Pcr.Equals("N") || exame.IgM.Equals("N"))
            {
                resultado = "N";
            }
            
            return resultado;
        }

        public string GetStatusExame(string status)
        {


            switch (status) { 
                case "I": return "IDETERMINADO";
                case "N": return "NEGATIVO";
                case "C": return "CURADO";
                case "P": return "POSITIVO";

                default: return "INDEFINIDO"; 
            }

            
        }

    }
}
