using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Model;
using Model.AuxModel;
using Model.ViewModel;
using MonitoraSUS.Utils;
using Newtonsoft.Json;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        /*
            * Lançamento de notificação 
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotificateByListAsync(List<ExameViewModel> exames)
        {
            //int qtdExames = exames.Count();
            //int qtdEnviadas = 0;

            List<SMSModel> smsModels = new List<SMSModel>();
            /*
            var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);

            var trabalhaMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
            var trabalhaEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
            */

            //  string comorbidade = "COVID19";

            /*
            ConfiguracaoNotificarModel configuracaoNotificar = null;

            if (trabalhaEstado != null && trabalhaEstado.IdEmpresaExame != -1)
                configuracaoNotificar = _configuracaoNotificarContext.GetByIdIdEmpresaExame(trabalhaEstado.IdEmpresaExame);
            else if (trabalhaMunicipio != null)
                configuracaoNotificar = _configuracaoNotificarContext.GetByIdMunicipio(trabalhaMunicipio.IdMunicipio);
                */

            // SMS
            // if (configuracaoNotificar != null && configuracaoNotificar.HabilitadoSms)
            // {
            // credenciais

            foreach (var item in exames)
            {
                var exame = _exameContext.GetById(item.IdExame);
                var pessoa = _pessoaContext.GetById(exame.IdPaciente);
                if (pessoa != null && exame != null)
                {
                    SMSModel sms = new SMSModel { To = "55" + pessoa.FoneCelular, Body = "sms!" };
                    smsModels.Add(sms);
                }
            }
            var resultado = await SendCompostSMS(smsModels);

            /*
                if (qtdEnviadas == qtdExames)
                    TempData["mensagemSucesso"] = "Todas as notificações foram enviadas com sucesso!";
                else
                    TempData["mensagemErro"] = "Erro ao enviar as todas notificações,  De " + qtdExames + "exames, foram enviadas " + qtdEnviadas + " notificações";
*/
            // }
            //  else
            //     TempData["mensagemErro"] = "Erro ao enviar notificação para o paciente, pois houve um problema com a configuração de SMS.";
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotificateById(int id, IFormCollection collection)
        {
            /*
            var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);

            var trabalhaMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
            var trabalhaEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
            */

            var exame = _exameContext.GetById(id);
            string comorbidade = "COVID19";
            var pessoa = _pessoaContext.GetById(exame.IdPaciente);

            /*
            ConfiguracaoNotificarModel configuracaoNotificar = null;

            if (trabalhaEstado != null && trabalhaEstado.IdEmpresaExame != -1)
                configuracaoNotificar = _configuracaoNotificarContext.GetByIdIdEmpresaExame(trabalhaEstado.IdEmpresaExame);
            else if (trabalhaMunicipio != null)
                configuracaoNotificar = _configuracaoNotificarContext.GetByIdMunicipio(trabalhaMunicipio.IdMunicipio);
                */

            // SMS
            // if (configuracaoNotificar != null && configuracaoNotificar.HabilitadoSms)
            //{
            if (pessoa != null && exame != null)
            {
                try
                {
                    SMSModel sms = new SMSModel { To = "55" + pessoa.FoneCelular, Body = "msg!" };
                    var resultado = await SendSimpleSMS(sms);
                    exame.FoiNotificado = true;
                    _exameContext.Update(exame);

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
            //  }
            //  else
            //     TempData["mensagemErro"] = "Erro ao enviar notificação para o paciente, pois houve um problema com a configuração de SMS.";

            return RedirectToAction(nameof(Notificate));
        }

        public IActionResult Export(List<ExameViewModel> exames)
        {
            var builder = new StringBuilder();
            builder.AppendLine("PACIENTE;INICIAIS;DATA NASC;NOME;CPF;RG;COMORBIDADES;ENDEREÇO;CONTATO;ESTADO;MUNICIPIO");

            string cpf = "", rg="", endereco="", contato="", 
                   estado="", municipio="", codigoColeta = "",
                   iniciais = "",dataNascimento="", nome="";
            
            foreach (var exame in exames)
            {
                cpf = (Methods.SoContemNumeros(exame.IdPaciente.Cpf) ? exame.IdPaciente.Cpf : " ");
                rg  = exame.IdPaciente.Cpf[0] != 'T' && !Methods.SoContemNumeros(exame.IdPaciente.Cpf) ? exame.IdPaciente.Cpf : " ";
               
                endereco =  exame.IdPaciente.Numero == "" ? "" : exame.IdPaciente.Rua + ", Nº " + exame.IdPaciente.Numero;
                endereco += exame.IdPaciente.Bairro == "" ? "" : ", " + exame.IdPaciente.Bairro;
                endereco += exame.IdPaciente.Complemento == ""? "" : ", "+exame.IdPaciente.Complemento;

                contato = exame.IdPaciente.FoneCelular;
                estado = exame.IdPaciente.Estado;
                municipio = exame.IdPaciente.Cidade;
                codigoColeta = exame.CodigoColeta;
                iniciais = RetornaIniciais(exame.IdPaciente.Nome);
                dataNascimento = exame.IdPaciente.DataNascimento.ToString("dd/MM/yyyy");
                nome = exame.IdPaciente.Nome;

                builder.AppendLine($"{codigoColeta};{iniciais};{dataNascimento};{nome};{cpf};{rg};{"-"};{endereco};{contato};{estado};{municipio}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "exames.csv");
        }

        public string RetornaIniciais(string nome) 
        {
            string[] s = nome.Split(' ');
            var inicias = "";
            for (int i = 0; i < s.Length; i++)
                inicias += s[i][0];

            return inicias.ToUpper();
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
                situacao.UltimaSituacaoSaude = exame.ResultadoStatus;
            }
            else
            {
                situacao = new SituacaoPessoaVirusBacteriaModel();

                situacao.IdVirusBacteria = exame.IdVirusBacteria.IdVirusBacteria;
                situacao.Idpessoa = _pessoaContext.GetByCpf(Methods.RemoveSpecialsCaracts(exame.IdPaciente.Cpf)).Idpessoa;
                situacao.UltimaSituacaoSaude = exame.ResultadoStatus;
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
            exame.IdEmpresaSaude = viewModel.IdEmpresaSaude;
            exame.FoiNotificado = viewModel.FoiNotificado;
            exame.DataNotificacao = viewModel.DataNotificacao;
            exame.EhProfissionalSaude = viewModel.EhProfissionalSaude;
            exame.CodigoColeta = viewModel.CodigoColeta;

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
            ex.IdEstado = exame.IdEstado;
            ex.MunicipioId = exame.IdMunicipio;
            ex.DataInicioSintomas = exame.DataInicioSintomas;
            ex.DataExame = exame.DataExame;
            ex.IdEmpresaSaude = exame.IdEmpresaSaude;
            ex.FoiNotificado = exame.FoiNotificado;
            ex.DataNotificacao = exame.DataNotificacao;
            ex.EhProfissionalSaude = exame.EhProfissionalSaude;
            ex.CodigoColeta = exame.CodigoColeta;

            return ex;
        }


        public PesquisaExameViewModel GetAllExamesViewModel(PesquisaExameViewModel pesquisaExame)
        {
            /*
             * Pegando usuario logado e carregando 
             * os exames que ele pode ver
             */
            var usuario = Methods.RetornLoggedUser((ClaimsIdentity)User.Identity);
            var secretarioMunicipio = _pessoaTrabalhaMunicipioContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);
            var secretarioEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(usuario.UsuarioModel.IdPessoa);


            var exames = new List<ExameModel>();
            if (usuario.RoleUsuario.Equals("AGENTE") || usuario.RoleUsuario.Equals("ADM"))
            {
                exames = _exameContext.GetByIdAgente(usuario.UsuarioModel.IdPessoa);
            }
            else if (usuario.RoleUsuario.Equals("GESTOR") || usuario.RoleUsuario.Equals("SECRETARIO"))
            {
                if (secretarioMunicipio != null)
                    exames = _exameContext.GetByIdMunicipio(secretarioMunicipio.IdMunicipio);
                if (secretarioEstado != null)
                {
                    if (secretarioEstado.IdEmpresaExame != EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
                        exames = _exameContext.GetByIdEmpresa(secretarioEstado.IdEmpresaExame);
                    else
                        exames = _exameContext.GetByIdEstado(secretarioEstado.IdEstado);
                }
            }

            /* 
             * 1º Filto - por datas 
             */
            if (pesquisaExame.DataInicial == DateTime.MinValue && pesquisaExame.DataFinal == DateTime.MinValue && !pesquisaExame.RealizouPesquisa)
            {
                pesquisaExame.DataInicial = DateTime.Now.AddDays(-7);
                pesquisaExame.DataFinal = DateTime.Now;
                exames = exames.Where(exameModel => exameModel.DataExame >= pesquisaExame.DataInicial && exameModel.DataExame <= DateTime.Now).OrderBy(ex => ex.DataExame).ToList();
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
                ex.IdEstado = exame.IdEstado;
                ex.MunicipioId = exame.IdMunicipio;
                ex.DataInicioSintomas = exame.DataInicioSintomas;
                ex.DataExame = exame.DataExame;
                ex.IdEstado = exame.IdEstado;
                ex.MunicipioId = exame.IdMunicipio;
                ex.IdEmpresaSaude = exame.IdEmpresaSaude;
                ex.FoiNotificado = exame.FoiNotificado;
                ex.DataNotificacao = exame.DataNotificacao;
                ex.EhProfissionalSaude = exame.EhProfissionalSaude;
                ex.CodigoColeta = exame.CodigoColeta;

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
                pesquisaExame.Exames = pesquisaExame.Exames.OrderBy(ex => ex.DataExame).ToList();
                pesquisaExame.DataInicial = pesquisaExame.Exames[0].DataExame;
                pesquisaExame.DataFinal = pesquisaExame.Exames[pesquisaExame.Exames.Count - 1].DataExame;
            }


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
            if (exame.IdPaciente.Cpf.Equals(""))
                exame.IdPaciente.Idpessoa = 0;

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
                    case ExameModel.RESULTADO_IMUNIZADO: examesTotalizados.Imunizados++; break;
                }
            }


            return examesTotalizados;
        }

        // pegar msg padrao do banco
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

        //SMS API DEV

        public static async Task<ResponseSMSModel> SendSimpleSMS(SMSModel sms)
        {
            try
            {
                const string url = "https://api.smsdev.com.br/send?key=FHHMEV35L21PWU2CG1QQIFWR&type=9&";
                var cliente = new HttpClient();
                var uri = url + "number=" + sms.To + "&msg=" + sms.Body;
                var resultado = await cliente.GetStringAsync(uri);
                var jsonResponse = JsonConvert.DeserializeObject<ResponseSMSModel>(resultado);
                // Console.WriteLine("\nOpa : " + jsonResponse.Descricao + "cod : " + jsonResponse.Codigo);
                return jsonResponse;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                throw e.InnerException;
            }

        }

        public static async Task<List<ResponseMultipleSMSModel>> SendCompostSMS(List<SMSModel> smsModels)
        {
            try
            {
                const string url = "https://api.smsdev.com.br/multiple?key=FHHMEV35L21PWU2CG1QQIFWR&type=9";
                var cliente = new HttpClient();
                var uri = url;
                // se por i+1 na string ele junta
                int contador = 0;
                for (int i = 0; i < smsModels.Count; i++)
                {
                    contador++;
                    uri = uri + "&number" + contador + "=" + smsModels[i].To + "&msg" + contador + "=" + smsModels[i].Body;
                }

                var resultado = await cliente.GetStringAsync(uri);

                Console.WriteLine("\nResposta: " + resultado);
                var jsonResponse = JsonConvert.DeserializeObject<List<ResponseMultipleSMSModel>>(resultado);

                // Console.WriteLine("\nOpa : " + jsonResponse);
                return jsonResponse;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                throw e.InnerException;
            }

        }
    }
}
