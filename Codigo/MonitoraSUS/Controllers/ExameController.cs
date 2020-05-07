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
using System.Text.RegularExpressions;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

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
        private readonly IConfiguracaoNotificarService _configuracaoNotificarContext;
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
                               IConfiguracaoNotificarService configuracaoNotificarContext)
        {
            _virusBacteriaContext = virusBacteriaContext;
            _exameContext = exameContext;
            _pessoaContext = pessoaContext;
            _municicpioContext = municicpioContext;
            _estadoContext = estadoContext;
            _situacaoPessoaContext = situacaoPessoaContext;
            _pessoaTrabalhaEstadoContext = pessoaTrabalhaEstado;
            _pessoaTrabalhaMunicipioContext = pessoaTrabalhaMunicipioContext;
            _configuracaoNotificarContext = configuracaoNotificarContext;
            _configuration = configuration;
        }

        public IActionResult Index(string pesquisa, DateTime DataInicial, DateTime DataFinal)
        {
            return View(GetAllExamesViewModel(pesquisa, DataInicial, DataFinal));
        }


        public IActionResult Notificate(string pesquisa, DateTime DataInicial, DateTime DataFinal)
        {
            return View(GetAllExamesViewModel(pesquisa, DataInicial, DataFinal));
        }

        /*
            * Lançamento de notificação 
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NotificateByList(List<ExameViewModel> exames)
        {
            int qtdExames = exames.Count();
            int qtdEnviadas = 0;

            var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);

            var trabalhaMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
            var trabalhaEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);


            string comorbidade = "COVID19";
            ConfiguracaoNotificarModel configuracaoNotificar = null;

            if (trabalhaEstado != null && trabalhaEstado.IdEmpresaExame != -1)
                configuracaoNotificar = _configuracaoNotificarContext.GetByIdIdEmpresaExame(trabalhaEstado.IdEmpresaExame);
            else if (trabalhaMunicipio != null)
                configuracaoNotificar = _configuracaoNotificarContext.GetByIdMunicipio(trabalhaMunicipio.IdMunicipio);

            // SMS
            if (configuracaoNotificar != null && configuracaoNotificar.HabilitadoSms)
            {
                // credenciais
                string accountSid = configuracaoNotificar.Sid;
                string authToken = configuracaoNotificar.Token;
                TwilioClient.Init(accountSid, authToken);

                foreach (var item in exames)
                {
                    var exame = _exameContext.GetById(item.IdExame);
                    var pessoa = _pessoaContext.GetById(exame.IdPaciente);
                    if (pessoa != null && exame != null)
                    {
                        try
                        {
                            // msg

                            var to = new PhoneNumber(pessoa.FoneCelular);
                            var message = MessageResource.Create(
                                    to,
                                    from: new PhoneNumber(configuracaoNotificar.NumeroSms),
                                    body:
                                     "Resultado do exame para " +
                                     comorbidade + ": *" +
                                     ResultadoDefinido(exame.Resultado, configuracaoNotificar) + "*."
                                );

                            qtdEnviadas++;

                        }
                        catch (Exception ex)
                        {
                            TempData["mensagemErro"] = "Erro ao enviar notificação para o paciente.";
                            Console.WriteLine
                                (
                                    $" Registration Failure : {ex.Message} " +
                                    $" responseTrace : {ex.StackTrace}"
                                );
                        }
                    }
                }

                if (qtdEnviadas == qtdExames)
                    TempData["mensagemSucesso"] = "Todas as notificações foram enviadas com sucesso!";
                else
                    TempData["mensagemErro"] = "Erro ao enviar as todas notificações,  De " + qtdExames + "exames, foram enviadas " + qtdEnviadas + " notificações";

            }
            else
                TempData["mensagemErro"] = "Erro ao enviar notificação para o paciente, pois houve um problema com a configuração de SMS.";

            foreach (var item in exames)
            {

                // TODO lançar notificacao
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
            return View(totaisRealizado);
        }

        /// <summary>
        /// Envia uma msg para o paciente 
        /// </summary>
        /// <param name="id">Id do exame</param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NotificateById(int id, IFormCollection collection)
        {
            var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);

            var trabalhaMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
            var trabalhaEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);

            var exame = _exameContext.GetById(id);
            string comorbidade = "COVID19";
            var pessoa = _pessoaContext.GetById(exame.IdPaciente);
            ConfiguracaoNotificarModel configuracaoNotificar = null;

            if (trabalhaEstado != null && trabalhaEstado.IdEmpresaExame != -1)
                configuracaoNotificar = _configuracaoNotificarContext.GetByIdIdEmpresaExame(trabalhaEstado.IdEmpresaExame);
            else if (trabalhaMunicipio != null)
                configuracaoNotificar = _configuracaoNotificarContext.GetByIdMunicipio(trabalhaMunicipio.IdMunicipio);

            // SMS
            if (configuracaoNotificar != null && configuracaoNotificar.HabilitadoSms)
            {
                if (pessoa != null && exame != null)
                {
                    try
                    {
                        // credenciais
                        string accountSid = configuracaoNotificar.Sid;
                        string authToken = configuracaoNotificar.Token;
                        TwilioClient.Init(accountSid, authToken);

                        // msg

                        var to = new PhoneNumber(pessoa.FoneCelular);
                         var message = MessageResource.Create(
                                 to,
                                 from: new PhoneNumber(configuracaoNotificar.NumeroSms),  
                                 body: 
                                  "Resultado do exame para " +
                                  comorbidade + ": *" +
                                  ResultadoDefinido(exame.Resultado, configuracaoNotificar) + "*."
                             );

                        TempData["mensagemSucesso"] = "Notificação enviado ao paciente com sucesso!";

                    }
                    catch (Exception ex)
                    {
                        TempData["mensagemErro"] = "Erro ao enviar notificação para o paciente.";
                        Console.WriteLine
                            (
                                $" Registration Failure : {ex.Message} " +
                                $" responseTrace : {ex.StackTrace}"
                            );
                    }
                }

                else
                    TempData["mensagemErro"] = "Erro ao enviar notificação em virtude de um problema com dados do exame ou paciente.";

                catch (Exception ex)
                {
                    ViewBag.responseNotificate = "Erro ao enviar notificação para o paciente.";
                    ViewBag.successN = "error";
                    Console.WriteLine
                        (
                            $" Registration Failure : {ex.Message} " +
                            $" responseTrace : {ex.StackTrace}"
                        );
                }
            }
            else
                TempData["mensagemErro"] = "Erro ao enviar notificação para o paciente, pois houve um problema com a configuração de SMS.";

            return RedirectToAction(nameof(Notificate));
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
                        pessoa = _pessoaContext.Insert(pessoa);
                    else
                        pessoa = _pessoaContext.Update(pessoa);
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
                catch
                {
                    TempData["mensagemErro"] = "Cadastro não pode ser concluido pois houve um problema ao inserir/atualizar o resultado do exame, tente novamente" +
                                                " Se o erro persistir, entre em contato com a Fábrica de Software da UFS pelo email fabricadesoftware@ufs.br";
                    return View(exame);
                }

                try
                {
                    // inserindo o exame
                    _exameContext.Insert(CreateExameModel(exame, pessoa.Idpessoa, true));
                }
                catch
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
                situacao.UltimaSituacaoSaude = Methods.GetResultadoExame(exame);
            }
            else
            {
                situacao = new SituacaoPessoaVirusBacteriaModel();

                situacao.IdVirusBacteria = exame.IdVirusBacteria.IdVirusBacteria;
                situacao.Idpessoa = _pessoaContext.GetByCpf(Methods.RemoveSpecialsCaracts(exame.IdPaciente.Cpf)).Idpessoa;
                situacao.UltimaSituacaoSaude = Methods.GetResultadoExame(exame);
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
            exame.IdEstado = viewModel.IdEstado;
            exame.IdMunicipio = viewModel.MunicipioId;
            exame.DataInicioSintomas = viewModel.DataInicioSintomas;
            exame.DataExame = viewModel.DataExame;
            exame.IdAgenteSaude = viewModel.IdAgenteSaude.Idpessoa;
            exame.IdEmpresaSaude = viewModel.IdEmpresaSaude;
            exame.FoiNotificado = viewModel.FoiNotificado;
            exame.DataNotificacao = viewModel.DataNotificacao;

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
            ex.Resultado = Methods.GetStatusExame(Methods.GetResultadoExame(new ExameViewModel { Pcr = exame.Pcr, IgG = exame.IgG, IgM = exame.IgM }));
            ex.IgG = exame.IgG;
            ex.IgM = exame.IgM;
            ex.Pcr = exame.Pcr;
            ex.IdEstado = exame.IdEstado;
            ex.MunicipioId = exame.IdMunicipio;
            ex.DataInicioSintomas = exame.DataInicioSintomas;
            ex.DataExame = exame.DataExame;
            ex.IdEmpresaSaude = exame.IdEmpresaSaude;
            ex.FoiNotificado = exame.FoiNotificado;
            ex.DataNotificacao = exame.DataNotificacao;

            return ex;
        }

        public TotalizadoresExameViewModel GetAllExamesViewModel(string pesquisa, DateTime DataInicial, DateTime DataFinal)
        {
            // indica se o usuário fez um filtro nos exames
            var foiFiltrado = false;

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
            else if (usuario.RoleUsuario.Equals("GESTOR"))
            {
                var secretarioMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);

                if (secretarioMunicipio != null)
                    exames = _exameContext.GetByIdMunicipio(secretarioMunicipio.IdMunicipio);
            }
            else if (usuario.RoleUsuario.Equals("SECRETARIO"))
            {
                var secretarioEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);

                if (secretarioEstado.IdEmpresaExame != 1)
                    exames = _exameContext.GetByIdEmpresa(secretarioEstado.IdEmpresaExame);
                else
                    exames = _exameContext.GetByIdEstado(secretarioEstado.IdEstado);
            }


            /* 
             * 1º Filto - por datas 
             */
            if (DataInicial > DateTime.MinValue && DataFinal > DateTime.MinValue)
            {
                exames = exames.Where(exameModel => exameModel.DataExame >= DataInicial && exameModel.DataExame <= DataFinal).ToList();
                foiFiltrado = true;
            }
            else if (DataInicial == DateTime.MinValue && DataFinal > DateTime.MinValue)
            {
                exames = exames.Where(exameModel => exameModel.DataExame <= DataFinal).ToList();
                foiFiltrado = true;
            }
            else if (DataFinal == DateTime.MinValue && DataInicial > DateTime.MinValue)
            {
                exames = exames.Where(exameModel => exameModel.DataExame >= DataInicial).ToList();
                foiFiltrado = true;
            }

            /* 
             * montando view model com o primeiro filtro
             */
            var examesViewModel = new List<ExameViewModel>();
            foreach (var exame in exames)
            {
                ExameViewModel ex = new ExameViewModel();
                ex.IdExame = exame.IdExame;
                ex.IdPaciente = _pessoaContext.GetById(exame.IdPaciente);
                ex.IdAgenteSaude = _pessoaContext.GetById(exame.IdAgenteSaude);
                ex.IdVirusBacteria = _virusBacteriaContext.GetById(exame.IdVirusBacteria);
                ex.Resultado = Methods.GetStatusExame(Methods.GetResultadoExame(new ExameViewModel { Pcr = exame.Pcr, IgG = exame.IgG, IgM = exame.IgM }));
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
                ex.FoiNotificado = exame.FoiNotificado;
                ex.DataNotificacao = exame.DataNotificacao;

                examesViewModel.Add(ex);
            }

            /*
             * 2º Filtro - filtrando ViewModel por nome ou cpf
             */
            pesquisa = pesquisa ?? "";
            if (!pesquisa.Equals(""))
            {
                if (Methods.SoContemLetras(pesquisa))
                {
                    examesViewModel = examesViewModel.Where(exameViewModel => exameViewModel.IdPaciente.Nome.ToUpper().Contains(pesquisa.ToUpper())).ToList();
                    foiFiltrado = true;
                }
                else
                {
                    examesViewModel = examesViewModel.Where(exameViewModel => exameViewModel.IdPaciente.Cpf.ToUpper().Contains(pesquisa.ToUpper())).ToList();
                    foiFiltrado = true;
                }
            }

            return (foiFiltrado ? PreencheTotalizadores(examesViewModel) : new TotalizadoresExameViewModel { Exames = examesViewModel });
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
            if (exame.IdPaciente.Cpf.Equals(""))
                exame.IdPaciente.Idpessoa = 0;

            return exame.IdPaciente;
        }

        public TotalizadoresExameViewModel PreencheTotalizadores(List<ExameViewModel> listaExames)
        {
            var examesTotalizados = new TotalizadoresExameViewModel { Exames = listaExames };

            foreach (var item in listaExames)
            {
                switch (item.Resultado.ToUpper())
                {
                    case "POSITIVO": examesTotalizados.Positivos++; break;
                    case "NEGATIVO": examesTotalizados.Negativos++; break;
                    case "INDETERMINADO": examesTotalizados.Indeterminados++; break;
                    case "CURADO": examesTotalizados.Imunizados++; break;
                }
            }

            return examesTotalizados;
        }

        private string ResultadoDefinido(string resultadoExame, ConfiguracaoNotificarModel config)
        {

            switch (resultadoExame.ToUpper())
            {
                case "POSITIVO":
                    return config.MensagemPositivo;

                case "NEGATIVO":
                    return config.MensagemPositivo;

                case "IMUNIZADO":
                    return config.MensagemImunizado;

                case "INDETERMINIDO":
                    return config.MensagemIndeterminado;

                default:
                    return config.MensagemIndeterminado;
            }
        }

    }
}