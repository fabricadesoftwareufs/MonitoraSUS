using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Model;
using MonitoraSUS.Utils;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

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

        public IActionResult Index(string cpf, DateTime DataInicial, DateTime DataFinal)
        {
            /*
             * O tratamento da variavel filtro é feito dentro 
             * do método GetAllExamesViewModel()
             */

            return View(GetAllExamesViewModel(cpf, DataInicial, DataFinal));
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

            exame.IdPaciente.Cpf = Methods.RemoveSpecialsCaracts(exame.IdPaciente.Cpf);

            if (SoContemNumeros(exame.IdPaciente.Cpf))
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
                var check = _exameContext.CheckDuplicateExamToday(exame.IdPaciente.Idpessoa, exame.IdVirusBacteria.IdVirusBacteria, exame.DataExame);
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
                _exameContext.Update(CreateExameModel(exame));

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

                TempData["mensagemSucesso"] = "Edição realizada com SUCESSO!";

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

            if (SoContemNumeros(exame.IdPaciente.Cpf))
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
                var pessoa = CreatePessoaModelByExame(exame);
                if (_exameContext.CheckDuplicateExamToday(pessoa.Idpessoa, exame.IdVirusBacteria.IdVirusBacteria, exame.DataExame).Count > 0)
                {
                    TempData["mensagemErro"] = "Notificação DUPLICADA! Já existe um exame registrado desse paciente para esse Vírus/Bactéria na " +
                                                "data informada. Por favor, verifique se os dados da notificação estão corretos.";
                    return View(exame);
                }

                try
                {
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
                    var idPessoa = _pessoaContext.GetByCpf(exame.IdPaciente.Cpf).Idpessoa;
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
                    _exameContext.Insert(CreateExameModel(exame));
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

        public ExameModel CreateExameModel(ExameViewModel viewModel)
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

        public List<ExameViewModel> GetAllExamesViewModel(string cpf, DateTime DataInicial, DateTime DataFinal)
        {
            /*
             * Pegando usuario logado e carregando 
             * os exames que ele pode ver
             */
            var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);


            var exames = new List<ExameModel>();
            if (usuario.RoleUsuario.Equals("AGENTE") || usuario.RoleUsuario.Equals("ADM"))
            {
                exames = _exameContext.GetByIdAgente(usuario.UsuarioModel.IdPessoa);
            }
            else if (usuario.RoleUsuario.Equals("GESTOR") || usuario.RoleUsuario.Equals("SECRETARIO"))
            {
                var secretarioMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);

                // verificando se o funcionario trabalha no municipio ou no estado
                if (secretarioMunicipio != null)
                {
                    var idEstado = Convert.ToInt32(_municicpioContext.GetById(secretarioMunicipio.IdMunicipio).Uf);
                    exames = _exameContext.GetByIdEstado(idEstado);
                }
                else
                {
                    var secretarioEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);

                    if (secretarioEstado.IdEmpresaExame != 1)
                        exames = _exameContext.GetByIdEmpresa(secretarioEstado.IdEmpresaExame);
                    else
                        exames = _exameContext.GetByIdEstado(secretarioEstado.IdEstado);
                }
            }

            /* 
             * Se o filtro for uma data válida, 
             * ele faz a seleção
             */
            cpf = cpf ?? "";

            if ((DataInicial == DateTime.MinValue) && (DataFinal == DateTime.MinValue) && cpf.Equals(""))
            {
                exames = exames.Where(exameModel => DateTime.Compare(exameModel.DataExame, DateTime.Today) == 0).ToList();
            }
            else if (!cpf.Equals(""))
            {
                cpf = Methods.RemoveSpecialsCaracts(cpf);
                var paciente = _pessoaContext.GetByCpf(cpf);

                if (paciente == null)
                {
                    exames = new List<ExameModel>();
                }
                else if (DataInicial > DateTime.MinValue && DataFinal > DateTime.MinValue)
                {
                    exames = exames.Where(exameModel => exameModel.DataExame >= DataInicial &&
                                        exameModel.DataExame <= DataFinal && exameModel.IdPaciente == paciente.Idpessoa).ToList();
                }
                else if (DataInicial == DateTime.MinValue && DataFinal > DateTime.MinValue)
                {
                    exames = exames.Where(exameModel => exameModel.DataExame <= DataFinal && exameModel.IdPaciente == paciente.Idpessoa).ToList();
                }
                else if (DataFinal == DateTime.MinValue && DataInicial > DateTime.MinValue)
                {
                    exames = exames.Where(exameModel => exameModel.DataExame >= DataInicial && exameModel.IdPaciente == paciente.Idpessoa).ToList();
                }
                else
                {
                    exames = exames.Where(exameModel => exameModel.IdPaciente == paciente.Idpessoa).ToList();
                }
            }
            else if (DataInicial > DateTime.MinValue && DataFinal > DateTime.MinValue)
            {
                exames = exames.Where(exameModel => exameModel.DataExame >= DataInicial && exameModel.DataExame <= DataFinal).ToList();
            }
            else if (DataInicial == DateTime.MinValue && DataFinal > DateTime.MinValue)
            {
                exames = exames.Where(exameModel => exameModel.DataExame <= DataFinal).ToList();
            }
            else if (DataFinal == DateTime.MinValue && DataInicial > DateTime.MinValue)
            {
                exames = exames.Where(exameModel => exameModel.DataExame >= DataInicial).ToList();
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
            exame.IdPaciente.Cpf = Methods.RemoveSpecialsCaracts(exame.IdPaciente.Cpf.ToUpper());
            exame.IdPaciente.Cep = Methods.RemoveSpecialsCaracts(exame.IdPaciente.Cep);
            exame.IdPaciente.FoneCelular = Methods.RemoveSpecialsCaracts(exame.IdPaciente.FoneCelular);
            exame.IdPaciente.Sexo = exame.IdPaciente.Sexo.Equals("M") ? "Masculino" : "Feminino";

            if (exame.IdPaciente.FoneFixo != null)
                exame.IdPaciente.FoneFixo = Methods.RemoveSpecialsCaracts(exame.IdPaciente.FoneFixo);

            return exame.IdPaciente;
        }

        public static bool SoContemNumeros(String texto)
        {
            texto = texto.Replace(".", "").Replace("-", "");
            var value = Regex.IsMatch(texto, "^[0-9]*$");
            return value;
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