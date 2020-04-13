using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Model;
using Service;
using Service.Interface;

namespace MonitoraSUS.Controllers
{
    public class ExameController : Controller
    {
        private readonly VirusBacteriaServiceService _virusBacteriaContext;
        private readonly ExameService _exameContext;
        private readonly PessoaService _pessoaContext;
        private readonly MunicipioService _municicpioContext;
        private readonly EstadoService _estadoContext;
        private readonly SituacaoVirusBacteriaService _situacaoPessoaContext;


        public ExameController(VirusBacteriaServiceService virusBacteriaContext,
                               ExameService exameContext,
                               PessoaService pessoaContext,
                               MunicipioService municicpioContext,
                               EstadoService estadoContext,
                               SituacaoVirusBacteriaService situacaoPessoaContext)
        {
            _virusBacteriaContext = virusBacteriaContext;
            _exameContext = exameContext;
            _pessoaContext = pessoaContext;
            _municicpioContext = municicpioContext;
            _estadoContext = estadoContext;
            _situacaoPessoaContext = situacaoPessoaContext;
        }

        public IActionResult Index()
        {
            return View(GetAllExamesViewModel());
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
                _exameContext.Update(CreateExameModel(exame));
                _situacaoPessoaContext.Update(CreateSituacaoPessoaModelByExame(exame, _situacaoPessoaContext.GetByIdPessoa(exame.IdPaciente.Idpessoa)));
                _pessoaContext.Update(CreatePessoaModelByExame(exame));

                TempData["mensagemSucesso"] = "Notificação realizada com SUCESSO!";

                return View();

            }
            catch
            {
                TempData["mensagemErro"] = "Houve um problema ao atualizar as informaçõess, tente novamente ou " +
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
                    var situacaoPessoa = _situacaoPessoaContext.GetByIdPessoa(exame.IdPaciente.Idpessoa);
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
                    _exameContext.Insert(CreateExameModel(exame));
                }
                catch
                {
                    TempData["mensagemErro"] = "Cadastro não pode ser concluido pois houve um problema ao inserir os dados do exame, " +
                                               "verifique as informações e tente novamente ou entre em contato com os desenvolvedores.";

                    return View(exame);
                }

                // codigo para realizar notificacao 

                TempData["mensagemSucesso"] = "Notificação realizada com SUCESSO!";

                return View();
            }
        }

        public SituacaoPessoaVirusBacteriaModel CreateSituacaoPessoaModelByExame(ExameViewModel exame, SituacaoPessoaVirusBacteriaModel situacao)
        {

            if (situacao != null)
            {
                situacao.UltimaSituacaoSaude = GetStatusExame(exame);
            }
            else
            {
                situacao = new SituacaoPessoaVirusBacteriaModel();

                situacao.IdVirusBacteria = exame.IdVirusBacteria.IdVirusBacteria;
                situacao.Idpessoa = exame.IdPaciente.Idpessoa;
                situacao.UltimaSituacaoSaude = GetStatusExame(exame);
            }

            return situacao;
        }

        public ExameModel CreateExameModel(ExameViewModel viewModel)
        {
            ExameModel exame = new ExameModel();

            exame.IdPaciente = viewModel.IdPaciente.Idpessoa;
            exame.IdVirusBacteria = viewModel.IdVirusBacteria.IdVirusBacteria;
            exame.IgG = viewModel.IgG;
            exame.IgM = viewModel.IgM;
            exame.Pcr = viewModel.Pcr;
            exame.EstadoRealizacao = viewModel.EstadoRealizacao;
            exame.MunicipioId = viewModel.MunicipioId;
            exame.DataInicioSintomas = viewModel.DataInicioSintomas;
            exame.DataExame = viewModel.DataExame;

            // pegando informações do agente de saúde logado no sistema
            var agente = _pessoaContext.GetById(1);
            exame.IdAgenteSaude = agente.Idpessoa;
            exame.EstadoRealizacao = _estadoContext.GetByName(agente.Estado).Id;
            exame.MunicipioId = _municicpioContext.GetByName(agente.Cidade).Id;

            return exame;
        }

        public ExameViewModel GetExameViewModelById(int id)
        {
            var exame = _exameContext.GetById(id);

            ExameViewModel ex = new ExameViewModel();

            ex.IdPaciente = _pessoaContext.GetById(exame.IdPaciente);
            ex.IdAgenteSaude = _pessoaContext.GetById(exame.IdAgenteSaude);
            ex.IdVirusBacteria = _virusBacteriaContext.GetById(exame.IdVirusBacteria);
            ex.Resultado = _situacaoPessoaContext.GetByIdPessoa(exame.IdPaciente).UltimaSituacaoSaude;
            ex.IgG = exame.IgG;
            ex.IgM = exame.IgM;
            ex.Pcr = exame.Pcr;
            ex.EstadoRealizacao = exame.EstadoRealizacao;
            ex.MunicipioId = exame.MunicipioId;
            ex.DataInicioSintomas = exame.DataInicioSintomas;
            ex.DataExame = exame.DataExame;
            ex.EstadoRealizacao = exame.EstadoRealizacao;
            ex.MunicipioId = exame.MunicipioId;

            return ex;
        }

        public List<ExameViewModel> GetAllExamesViewModel()
        {

            var examesViewModel = new List<ExameViewModel>();
            var exames = _exameContext.GetAll();

            foreach (var exame in exames)
            {
                ExameViewModel ex = new ExameViewModel();

                ex.IdPaciente = _pessoaContext.GetById(exame.IdPaciente);
                ex.IdAgenteSaude = _pessoaContext.GetById(exame.IdAgenteSaude);
                ex.IdVirusBacteria = _virusBacteriaContext.GetById(exame.IdVirusBacteria);
                ex.Resultado = _situacaoPessoaContext.GetByIdPessoa(exame.IdPaciente).UltimaSituacaoSaude;
                ex.IgG = exame.IgG;
                ex.IgM = exame.IgM;
                ex.Pcr = exame.Pcr;
                ex.EstadoRealizacao = exame.EstadoRealizacao;
                ex.MunicipioId = exame.MunicipioId;
                ex.DataInicioSintomas = exame.DataInicioSintomas;
                ex.DataExame = exame.DataExame;
                ex.EstadoRealizacao = exame.EstadoRealizacao;
                ex.MunicipioId = exame.MunicipioId;

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


            return exame.IdPaciente;
        }

        public static string RemoveSpecialsCaracts(string poluatedString)
            => Regex.Replace(poluatedString, "[^0-9a-zA-Z]+", "");

        public string GetStatusExame(ExameViewModel exame)
        {

            string resultado = "I";

            if (exame.Pcr.Equals('S') || exame.IgM.Equals('S'))
            {
                resultado = "P";
            }
            else if (exame.IgG.Equals('S'))
            {
                resultado = "C";
            }
            else if (exame.Pcr.Equals('N'))
            {
                resultado = "N";
            }
            else if (exame.Pcr.Equals('I'))
            {
                resultado = "I";
            }

            return resultado;
        }

    }
}
