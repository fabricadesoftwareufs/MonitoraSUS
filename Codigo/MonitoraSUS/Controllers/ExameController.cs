using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Model;
using MonitoraSUS.Utils;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace MonitoraSUS.Controllers
{
    [Authorize]
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

        public ExameController(IVirusBacteriaService virusBacteriaContext,
                               IExameService exameContext,
                               IPessoaService pessoaContext,
                               IMunicipioService municicpioContext,
                               IEstadoService estadoContext,
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
        }

        public IActionResult Index(DateTime filtro)
        {
            /*
             * O tratamento da variavel filtro é feito dentro 
             * do método GetAllExamesViewModel()
             */
            return View(GetAllExamesViewModel(filtro));
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
            var situacao = _situacaoPessoaContext.GetById(exame.IdPaciente, exame.IdVirusBacteria);

            /* 
             * Removendo situação do paciente 
             */
            try
            {
                if (situacao != null)
                    _situacaoPessoaContext.Delete(situacao.Idpessoa, situacao.IdVirusBacteria);
            }
            catch
            {
                TempData["mensagemErro"] = "Não foi possível excluir esse exame, tente novamente." +
                                           " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";

                return RedirectToAction(nameof(Index));
            }

            /* 
             * Removendo exame do paciente 
             */
            try
            {
                _exameContext.Delete(id);
            }
            catch
            {
                /*
                 * Se o exame não puder ser removido, adicionar 
                 * novamente a ultima situacao do paciente pra 
                 * manter a consistência do banco de dados
                 */
                try { _situacaoPessoaContext.Insert(situacao); }
                catch { }

                TempData["mensagemErro"] = " Não foi possível excluir esse exame, tente novamente." +
                                           " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";

                return RedirectToAction(nameof(Index));
            }

            TempData["mensagemSucesso"] = "O Exame foi removido com sucesso!";
            return RedirectToAction(nameof(Index));
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
                /*
                 * Atualizando Exame
                 */
                _exameContext.Update(CreateExameModel(exame, false));

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
                _pessoaContext.Update(CreatePessoaModelByExame(exame));

                TempData["mensagemSucesso"] = "Notificação realizada com SUCESSO!";

                return View(new ExameViewModel());

            }
            catch
            {
                TempData["mensagemErro"] = "Houve um problema ao atualizar as informações, tente novamente." +
                  " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";

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

            if (Methods.ValidarCpf(exame.IdPaciente.Cpf))
            {
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
                        TempData["resultadoPesquisa"] = "Paciente não cadastrado, preencha os campos para cadastra-lo!";

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
                        TempData["mensagemErro"] = "Cadastro não pode ser concluido pois houve um problema ao inserir/atualizar dados do paciente, tente novamente. " +
                                                    " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";
                        return View(exame);
                    }


                    try
                    {
                        // inserindo o resultado do exame (situacao da pessoa)                  
                        var cpf = Methods.RemoveSpecialsCaracts(exame.IdPaciente.Cpf);
                        var idPessoa = _pessoaContext.GetByCpf(cpf).Idpessoa;
                        var situacaoPessoa = _situacaoPessoaContext.GetById(idPessoa, exame.IdVirusBacteria.IdVirusBacteria);

                        if (situacaoPessoa == null)
                            _situacaoPessoaContext.Insert(CreateSituacaoPessoaModelByExame(exame, situacaoPessoa));
                        else
                            _situacaoPessoaContext.Update(CreateSituacaoPessoaModelByExame(exame, situacaoPessoa));
                    }
                    catch
                    {
                        TempData["mensagemErro"] = "Cadastro não pode ser concluido pois houve um problema ao inserir/atualizar o resultado do exame, tente novamente" +
                                                    " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";
                        return View(exame);
                    }

                    try
                    {
                        // inserindo o exame
                        _exameContext.Insert(CreateExameModel(exame, true));
                    }
                    catch
                    {
                        TempData["mensagemErro"] = "Cadastro não pode ser concluido pois houve um problema ao inserir os dados do exame, tente novamente." +
                                                   " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";


                        return View(exame);
                    }

                    // codigo para realizar notificacao 

                    TempData["mensagemSucesso"] = "Notificação realizada com SUCESSO!";

                    return View(new ExameViewModel());
                }
            }
            else
            {
                TempData["resultadoPesquisa"] = "Esse esse cpf não é válido!";
                return View(exame);
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
                situacao.Idpessoa = _pessoaContext.GetByCpf(Methods.RemoveSpecialsCaracts(exame.IdPaciente.Cpf)).Idpessoa;
                situacao.UltimaSituacaoSaude = GetResultadoExame(exame);
            }

            return situacao;
        }

        public ExameModel CreateExameModel(ExameViewModel viewModel, bool atualizarCidadeEstado)
        {
            ExameModel exame = new ExameModel();

            exame.IdExame = viewModel.IdExame;
            exame.IdPaciente = _pessoaContext.GetByCpf(Methods.RemoveSpecialsCaracts(viewModel.IdPaciente.Cpf)).Idpessoa;
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

            /*
             *  pegando informações do agente de saúde logado no sistema 
             */
            var agente = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);


            var secretarioMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(agente.usuarioModel.IdPessoa);
            var secretarioEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(agente.usuarioModel.IdPessoa);

            // verificando se o funcionario trabalha no municipio ou no estado
            if (secretarioMunicipio != null)
            {
                exame.IdMunicipio = secretarioMunicipio.IdMunicipio;
                exame.IdEstado = _estadoContext.GetByUf(_municicpioContext.GetById(secretarioMunicipio.IdMunicipio).Uf).Id;

                if (secretarioEstado != null)
                    exame.IdEmpresaSaude = secretarioEstado.IdEmpresaExame;
            }
            else
            {
                if (secretarioEstado != null)
                {
                    exame.IdEstado = _estadoContext.GetByUf(_municicpioContext.GetById(secretarioMunicipio.IdMunicipio).Uf).Id;
                    exame.IdEmpresaSaude = secretarioEstado.IdEmpresaExame;
                }
            }

            exame.IdAgenteSaude = agente.usuarioModel.IdPessoa;

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

        public List<ExameViewModel> GetAllExamesViewModel(DateTime filtro)
        {
            /*
             * Pegando usuario logado e carregando 
             * os exames que ele pode ver
             */
            var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);

            var exames = new List<ExameModel>();
            if (usuario.RoleUsuario.Equals("AGENTE"))
            {
                exames = _exameContext.GetByIdAgente(usuario.usuarioModel.IdPessoa);
            }
            else if (usuario.RoleUsuario.Equals("COORDENADOR") || usuario.RoleUsuario.Equals("SECRETARIO"))
            {
                var secretarioMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.usuarioModel.IdPessoa);

                // verificando se o funcionario trabalha no municipio ou no estado
                if (secretarioMunicipio != null)
                {
                    var uf = _municicpioContext.GetById(secretarioMunicipio.IdMunicipio).Uf;
                    var idEstado = _estadoContext.GetByUf(uf).Id;
                    exames = _exameContext.GetByIdEstado(idEstado);
                }
                else
                {
                    var secretarioEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.usuarioModel.IdPessoa);
                    if (secretarioEstado != null)
                        exames = _exameContext.GetByIdEstado(secretarioEstado.IdEstado);
                }
            }

            /* 
             * Se o filtro for uma data válida, 
             * ele faz a seleção
             */
            if (filtro != DateTime.MinValue && filtro != null)
            {
                exames = exames.Where(exameModel => DateTime.Compare(exameModel.DataExame, filtro) == 0).ToList();
            }
            else
            {
                exames = exames.Where(exameModel => DateTime.Compare(exameModel.DataExame, DateTime.Today) == 0).ToList();
            }


            var examesViewModel = new List<ExameViewModel>();

            foreach (var exame in exames)
            {
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
                ex.IdEstado = exame.IdEstado;
                ex.MunicipioId = exame.IdMunicipio;

                examesViewModel.Add(ex);
            }

            return examesViewModel;
        }

        public PessoaModel CreatePessoaModelByExame(ExameViewModel exame)
        {


            exame.IdPaciente.Cpf = Methods.RemoveSpecialsCaracts(exame.IdPaciente.Cpf);
            exame.IdPaciente.Cep = Methods.RemoveSpecialsCaracts(exame.IdPaciente.Cep);
            exame.IdPaciente.FoneCelular = Methods.RemoveSpecialsCaracts(exame.IdPaciente.FoneCelular);
            exame.IdPaciente.Sexo = exame.IdPaciente.Sexo.Equals("M") ? "Masculino" : "Feminino";

            if (exame.IdPaciente.FoneFixo != null)
                exame.IdPaciente.FoneFixo = Methods.RemoveSpecialsCaracts(exame.IdPaciente.FoneFixo);

            var paciente = _pessoaContext.GetByCpf(exame.IdPaciente.Cpf);
            if (paciente != null)
                exame.IdPaciente.Idpessoa = paciente.Idpessoa;

            return exame.IdPaciente;
        }

        public string GetResultadoExame(ExameViewModel exame)
        {

            string resultado = "I";

            if (exame.Pcr.Equals("S") || exame.IgM.Equals("S"))
            {
                resultado = "P";
            }
            else if (exame.Pcr.Equals("I") || exame.IgM.Equals("I"))
            {
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


            switch (status)
            {
                case "I": return "INDETERMINADO";
                case "N": return "NEGATIVO";
                case "C": return "CURADO";
                case "P": return "POSITIVO";

                default: return "IDETERMINADO";
            }


        }

    }
}