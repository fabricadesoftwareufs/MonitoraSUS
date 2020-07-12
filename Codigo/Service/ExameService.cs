using Model;
using Model.AuxModel;
using Model.ViewModel;
using Util;
using Newtonsoft.Json;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.IO;
using Repository.Interfaces;

namespace Service
{
    public class ExameService : IExameService
    {
        /**
         * Não retirei o contexto devido aos outros metodos.
         * Ideal usar apenas services, visto que o mesmo contexto será passado/obtido.
         */
        private readonly monitorasusContext _context;
        private readonly IPessoaService _pessoaService;
        private readonly ISituacaoVirusBacteriaService _situacaoPessoaService;

        // Repositorios
        private readonly IExameRepository _exameRepository;

        public ExameService(monitorasusContext context,
            IPessoaService pessoaService,
            ISituacaoVirusBacteriaService situacaoPessoaService,
            IExameRepository exameRepository)
        {
            _context = context;
            _pessoaService = pessoaService;
            _situacaoPessoaService = situacaoPessoaService;

            // Repositorio de Exame
            _exameRepository = exameRepository;
        }

        public bool Insert(ExameViewModel exameModel)
        {
            using (var transaction = _exameRepository.GetContext().Database.BeginTransaction())
            {
                try
                {
                    if (_exameRepository.GetExamesRelizadosData(exameModel.Paciente.Idpessoa, exameModel.Exame.IdVirusBacteria, exameModel.Exame.DataExame, exameModel.Exame.MetodoExame).Count > 0)
                        throw new ServiceException("Notificação DUPLICADA! Já existe um exame registrado desse paciente para esse Vírus/Bactéria na " +
                                                    "data informada e método aplicado. Por favor, verifique se os dados da notificação estão corretos.");
                    if (exameModel.Paciente.Idpessoa == 0)
                        exameModel.Paciente = _pessoaService.Insert(exameModel.Paciente);
                    else
                        exameModel.Paciente = _pessoaService.Update(exameModel.Paciente, false);

                    // inserindo o resultado do exame (situacao da pessoa)
                    var situacaoPessoa = _situacaoPessoaService.GetById(exameModel.Paciente.Idpessoa, exameModel.Exame.IdVirusBacteria);
                    if (situacaoPessoa == null)
                        _situacaoPessoaService.Insert(CreateSituacaoPessoaModelByExame(exameModel, situacaoPessoa, _pessoaService));
                    else
                        _situacaoPessoaService.Update(CreateSituacaoPessoaModelByExame(exameModel, situacaoPessoa, _pessoaService));

                    // repositorio
                    _exameRepository.Insert(exameModel);

                    // Caso dê tudo certo, commita.
                    transaction.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        // Metodos privados
        private SituacaoPessoaVirusBacteriaModel CreateSituacaoPessoaModelByExame(ExameViewModel exame, SituacaoPessoaVirusBacteriaModel situacao, IPessoaService pessoaService)
        {
            if (situacao != null)
                situacao.UltimaSituacaoSaude = exame.Exame.ResultadoStatus;
            else
            {
                situacao = new SituacaoPessoaVirusBacteriaModel();
                situacao.IdVirusBacteria = exame.Exame.IdVirusBacteria;
                situacao.Idpessoa = pessoaService.GetByCpf(Methods.RemoveSpecialsCaracts(exame.Paciente.Cpf)).Idpessoa;
                situacao.UltimaSituacaoSaude = exame.Exame.ResultadoStatus;
                situacao.DataUltimoMonitoramento = null;
            }

            return situacao;
        }


        public bool Delete(int id)
        {
            var exame = _context.Exame.Find(id);
            _context.Exame.Remove(exame);
            return _context.SaveChanges() == 1 ? true : false;
        }

        public bool Update(ExameViewModel exameModel)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    IPessoaService _pessoaService = new PessoaService(_context);
                    ISituacaoVirusBacteriaService _situacaoPessoaService = new SituacaoVirusBacteriaService(_context);
                    exameModel.Exame.IdAgenteSaude = exameModel.Usuario.IdPessoa;
                    var usuarioDuplicado = _pessoaService.GetByCpf(exameModel.Paciente.Cpf);
                    if (usuarioDuplicado != null)
                    {
                        if (!(usuarioDuplicado.Idpessoa == exameModel.Paciente.Idpessoa))
                            throw new ServiceException("Já existe um paciente com esse CPF/RG, tente novamente!");
                    }

                    var examesRealizados = GetExamesRelizadosData(exameModel.Paciente.Idpessoa, exameModel.Exame.IdVirusBacteria, exameModel.Exame.DataExame, exameModel.Exame.MetodoExame);
                    if (examesRealizados.Count > 0)
                    {
                        var exame = examesRealizados.FirstOrDefault();
                        if (exame.IdExame != exameModel.Exame.IdExame)
                            throw new ServiceException("Notificação DUPLICADA! Já existe um exame registrado desse paciente para esse Vírus/Bactéria na " +
                                                            "data informada. Por favor, verifique se os dados da notificação estão corretos.");
                    }

                    var situacao = _situacaoPessoaService.GetById(exameModel.Paciente.Idpessoa, exameModel.Exame.IdVirusBacteria);
                    if (situacao == null)
                        _situacaoPessoaService.Insert(CreateSituacaoPessoaModelByExame(exameModel, situacao, _pessoaService));
                    else
                        _situacaoPessoaService.Update(CreateSituacaoPessoaModelByExame(exameModel, situacao, _pessoaService));

                    _pessoaService.Update(CreatePessoaModelByExame(exameModel, _pessoaService), false);
                    _context.Update(ModelToEntity(exameModel));
                    _context.SaveChanges();
                    transaction.Commit();
                    return true;

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public ExameModel GetByIdColeta(string codigoColeta)
          => _context.Exame
                .Where(exameModel => exameModel.CodigoColeta.Equals(codigoColeta))
                .Select(exame => new ExameModel
                {
                    IdAreaAtuacao = exame.IdPacienteNavigation.IdAreaAtuacao,
                    AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                    CodigoColeta = exame.CodigoColeta,
                    Coriza = Convert.ToBoolean(exame.Coriza),
                    DataExame = exame.DataExame,
                    DataInicioSintomas = exame.DataInicioSintomas,
                    Diarreia = Convert.ToBoolean(exame.Diarreia),
                    DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                    DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                    DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                    DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                    Febre = Convert.ToBoolean(exame.Febre),
                    IdAgenteSaude = exame.IdAgenteSaude,
                    IdEmpresaSaude = exame.IdEmpresaSaude,
                    IdEstado = exame.IdEstado,
                    IdExame = exame.IdExame,
                    IdMunicipio = exame.IdMunicipio,
                    IdNotificacao = exame.IdNotificacao,
                    IdPaciente = exame.IdPaciente,
                    IdVirusBacteria = exame.IdVirusBacteria,
                    IgG = exame.IgG,
                    IgGIgM = exame.IgMigG,
                    IgM = exame.IgM,
                    MetodoExame = exame.MetodoExame,
                    Nausea = Convert.ToBoolean(exame.Nausea),
                    OutrosSintomas = exame.OutroSintomas,
                    Pcr = exame.Pcr,
                    PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                    RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                    StatusNotificacao = exame.StatusNotificacao,
                    Tosse = Convert.ToBoolean(exame.Tosse)
                }).FirstOrDefault();


        public ExameViewModel GetById(int id)
          => _context.Exame
                .Where(exameModel => exameModel.IdExame == id)
                .Select(exame => new ExameViewModel
                {
                    Exame = new ExameModel()
                    {
                        IdAreaAtuacao = exame.IdPacienteNavigation.IdAreaAtuacao,
                        AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                        CodigoColeta = exame.CodigoColeta,
                        Coriza = Convert.ToBoolean(exame.Coriza),
                        DataExame = exame.DataExame,
                        DataInicioSintomas = exame.DataInicioSintomas,
                        Diarreia = Convert.ToBoolean(exame.Diarreia),
                        DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                        DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                        DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                        DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                        Febre = Convert.ToBoolean(exame.Febre),
                        IdAgenteSaude = exame.IdAgenteSaude,
                        IdEmpresaSaude = exame.IdEmpresaSaude,
                        IdEstado = exame.IdEstado,
                        IdExame = exame.IdExame,
                        IdMunicipio = exame.IdMunicipio,
                        IdNotificacao = exame.IdNotificacao,
                        IdPaciente = exame.IdPaciente,
                        IdVirusBacteria = exame.IdVirusBacteria,
                        IgG = exame.IgG,
                        IgGIgM = exame.IgMigG,
                        IgM = exame.IgM,
                        MetodoExame = exame.MetodoExame,
                        Nausea = Convert.ToBoolean(exame.Nausea),
                        OutrosSintomas = exame.OutroSintomas,
                        Pcr = exame.Pcr,
                        PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                        RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                        StatusNotificacao = exame.StatusNotificacao,
                        Tosse = Convert.ToBoolean(exame.Tosse)
                    },
                    Paciente = new PessoaModel()
                    {
                        IdAreaAtuacao = exame.IdPacienteNavigation.IdAreaAtuacao,
                        Bairro = exame.IdPacienteNavigation.Bairro,
                        Cancer = Convert.ToBoolean(exame.IdPacienteNavigation.Cancer),
                        Cardiopatia = Convert.ToBoolean(exame.IdPacienteNavigation.Cardiopatia),
                        Cep = exame.IdPacienteNavigation.Cep,
                        Cidade = exame.IdPacienteNavigation.Cidade,
                        Cns = exame.IdPacienteNavigation.Cns,
                        Complemento = exame.IdPacienteNavigation.Complemento,
                        Coriza = Convert.ToBoolean(exame.IdPacienteNavigation.Coriza),
                        Cpf = exame.IdPacienteNavigation.Cpf,
                        DataNascimento = exame.IdPacienteNavigation.DataNascimento,
                        DataObito = exame.IdPacienteNavigation.DataObito,
                        Diabetes = Convert.ToBoolean(exame.IdPacienteNavigation.Diabetes),
                        Diarreia = Convert.ToBoolean(exame.IdPacienteNavigation.Diarreia),
                        DificuldadeRespiratoria = Convert.ToBoolean(exame.IdPacienteNavigation.DificuldadeRespiratoria),
                        DoencaRenal = Convert.ToBoolean(exame.IdPacienteNavigation.DoencaRenal),
                        DoencaRespiratoria = Convert.ToBoolean(exame.IdPacienteNavigation.DoencaRespiratoria),
                        DorAbdominal = Convert.ToBoolean(exame.IdPacienteNavigation.DorAbdominal),
                        DorGarganta = Convert.ToBoolean(exame.IdPacienteNavigation.DorGarganta),
                        DorOuvido = Convert.ToBoolean(exame.IdPacienteNavigation.DorOuvido),
                        Email = exame.IdPacienteNavigation.Email,
                        Epilepsia = Convert.ToBoolean(exame.IdPacienteNavigation.Epilepsia),
                        Estado = exame.IdPacienteNavigation.Estado,
                        Febre = Convert.ToBoolean(exame.IdPacienteNavigation.Febre),
                        FoneCelular = exame.IdPacienteNavigation.FoneCelular,
                        FoneFixo = exame.IdPacienteNavigation.FoneFixo,
                        Hipertenso = Convert.ToBoolean(exame.IdPacienteNavigation.Hipertenso),
                        Idpessoa = exame.IdPaciente,
                        Imunodeprimido = Convert.ToBoolean(exame.IdPacienteNavigation.Imunodeprimido),
                        Latitude = exame.IdPacienteNavigation.Latitude,
                        Longitude = exame.IdPacienteNavigation.Longitude,
                        Nausea = Convert.ToBoolean(exame.IdPacienteNavigation.Nausea),
                        Nome = exame.IdPacienteNavigation.Nome,
                        Numero = exame.IdPacienteNavigation.Numero,
                        Obeso = Convert.ToBoolean(exame.IdPacienteNavigation.Obeso),
                        OutrasComorbidades = exame.IdPacienteNavigation.OutrasComorbidades,
                        OutrosSintomas = exame.IdPacienteNavigation.OutrosSintomas,
                        PerdaOlfatoPaladar = Convert.ToBoolean(exame.IdPacienteNavigation.PerdaOlfatoPaladar),
                        Profissao = exame.IdPacienteNavigation.Profissao,
                        Rua = exame.IdPacienteNavigation.Rua,
                        Sexo = exame.IdPacienteNavigation.Sexo,
                        SituacaoSaude = exame.IdPacienteNavigation.SituacaoSaude,
                        Tosse = Convert.ToBoolean(exame.IdPacienteNavigation.Tosse)
                    }
                }).FirstOrDefault();

        private Exame ModelToEntity(ExameViewModel exameModel)
        {
            exameModel.Exame.CodigoColeta = (exameModel.Exame.CodigoColeta == null) ? "" : exameModel.Exame.CodigoColeta;
            exameModel.Exame.IdNotificacao = (exameModel.Exame.IdNotificacao == null) ? "" : exameModel.Exame.IdNotificacao;
            var secretarioMunicipio = _context.Pessoatrabalhamunicipio.Where(p => p.IdPessoa == exameModel.Exame.IdAgenteSaude).FirstOrDefault();

            if (secretarioMunicipio != null)
            {
                exameModel.Exame.IdMunicipio = secretarioMunicipio.IdMunicipio;
                exameModel.Exame.IdEstado = Convert.ToInt32(secretarioMunicipio.IdMunicipioNavigation.Uf);
                exameModel.Exame.IdEmpresaSaude = 1; // empresa padrão do banco 
            }
            else
            {
                var secretarioEstado = _context.Pessoatrabalhaestado.Where(p => p.Idpessoa == exameModel.Exame.IdAgenteSaude).FirstOrDefault();
                exameModel.Exame.IdEstado = secretarioEstado.IdEstado;
                exameModel.Exame.IdEmpresaSaude = secretarioEstado.IdEmpresaExame;
                exameModel.Exame.IdMunicipio = null;
            }
            return new Exame
            {
                IdAreaAtuacao = exameModel.Exame.IdAreaAtuacao,
                IdExame = exameModel.Exame.IdExame,
                IdAgenteSaude = exameModel.Usuario.IdPessoa,
                IdPaciente = exameModel.Paciente.Idpessoa,
                IdVirusBacteria = exameModel.Exame.IdVirusBacteria,
                IgG = exameModel.Exame.IgG,
                IgM = exameModel.Exame.IgM,
                Pcr = exameModel.Exame.Pcr,
                IgMigG = exameModel.Exame.IgGIgM,
                MetodoExame = exameModel.Exame.MetodoExame,
                IdEstado = exameModel.Exame.IdEstado,
                IdMunicipio = exameModel.Exame.IdMunicipio,
                DataInicioSintomas = exameModel.Exame.DataInicioSintomas,
                DataExame = exameModel.Exame.DataExame,
                IdEmpresaSaude = exameModel.Exame.IdEmpresaSaude,
                CodigoColeta = exameModel.Exame.CodigoColeta,
                StatusNotificacao = exameModel.Exame.StatusNotificacao,
                IdNotificacao = exameModel.Exame.IdNotificacao,
                DataNotificacao = DateTime.Now,
                AguardandoResultado = Convert.ToByte(exameModel.Exame.AguardandoResultado),
                Coriza = Convert.ToByte(exameModel.Exame.Coriza),
                Nausea = Convert.ToByte(exameModel.Exame.Nausea),
                Tosse = Convert.ToByte(exameModel.Exame.Tosse),
                PerdaOlfatoPaladar = Convert.ToByte(exameModel.Exame.PerdaOlfatoPaladar),
                RelatouSintomas = Convert.ToByte(exameModel.Exame.RelatouSintomas),
                Diarreia = Convert.ToByte(exameModel.Exame.Diarreia),
                DificuldadeRespiratoria = Convert.ToByte(exameModel.Exame.DificuldadeRespiratoria),
                DorAbdominal = Convert.ToByte(exameModel.Exame.DorAbdominal),
                DorGarganta = Convert.ToByte(exameModel.Exame.DorGarganta),
                DorOuvido = Convert.ToByte(exameModel.Exame.DorOuvido),
                Febre = Convert.ToByte(exameModel.Exame.Febre),
                OutroSintomas = exameModel.Exame.OutrosSintomas,
            };
        }

        public List<ExameBuscaModel> GetByIdAgente(int idAgente, DateTime dataInicio, DateTime dataFim)
         => _context.Exame
                .Where(exameModel => exameModel.IdAgenteSaude == idAgente
                    && exameModel.DataExame >= dataInicio && exameModel.DataExame <= dataFim)
                .Select(exame => new ExameBuscaModel
                {
                    Cns = exame.IdPacienteNavigation.Cns,
                    Cpf = exame.IdPacienteNavigation.Cpf,
                    NomePaciente = exame.IdPacienteNavigation.Nome,
                    NomeVirusBateria = exame.IdVirusBacteriaNavigation.Nome,
                    Cidade = exame.IdPacienteNavigation.Cidade,
                    Estado = exame.IdEstadoNavigation.Uf,
                    Exame = new ExameModel()
                    {
                        IdAreaAtuacao = exame.IdPacienteNavigation.IdAreaAtuacao,
                        AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                        CodigoColeta = exame.CodigoColeta,
                        Coriza = Convert.ToBoolean(exame.Coriza),
                        DataExame = exame.DataExame,
                        DataInicioSintomas = exame.DataInicioSintomas,
                        Diarreia = Convert.ToBoolean(exame.Diarreia),
                        DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                        DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                        DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                        DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                        Febre = Convert.ToBoolean(exame.Febre),
                        IdAgenteSaude = exame.IdAgenteSaude,
                        IdEmpresaSaude = exame.IdEmpresaSaude,
                        IdEstado = exame.IdEstado,
                        IdExame = exame.IdExame,
                        IdMunicipio = exame.IdMunicipio,
                        IdNotificacao = exame.IdNotificacao,
                        IdPaciente = exame.IdPaciente,
                        IdVirusBacteria = exame.IdVirusBacteria,
                        IgG = exame.IgG,
                        IgGIgM = exame.IgMigG,
                        IgM = exame.IgM,
                        MetodoExame = exame.MetodoExame,
                        Nausea = Convert.ToBoolean(exame.Nausea),
                        OutrosSintomas = exame.OutroSintomas,
                        Pcr = exame.Pcr,
                        PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                        RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                        StatusNotificacao = exame.StatusNotificacao,
                        Tosse = Convert.ToBoolean(exame.Tosse)
                    }
                }).ToList();
        public List<ExameBuscaModel> GetByIdAgente(int idAgente, int lastRecord)
         => _context.Exame
                .Where(exameModel => exameModel.IdAgenteSaude == idAgente).Take(lastRecord)
                .Select(exame => new ExameBuscaModel
                {
                    Cns = exame.IdPacienteNavigation.Cns,
                    Cpf = exame.IdPacienteNavigation.Cpf,
                    NomePaciente = exame.IdPacienteNavigation.Nome,
                    NomeVirusBateria = exame.IdVirusBacteriaNavigation.Nome,
                    Cidade = exame.IdPacienteNavigation.Cidade,
                    Estado = exame.IdEstadoNavigation.Uf,
                    Exame = new ExameModel()
                    {
                        IdAreaAtuacao = exame.IdPacienteNavigation.IdAreaAtuacao,
                        AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                        CodigoColeta = exame.CodigoColeta,
                        Coriza = Convert.ToBoolean(exame.Coriza),
                        DataExame = exame.DataExame,
                        DataInicioSintomas = exame.DataInicioSintomas,
                        Diarreia = Convert.ToBoolean(exame.Diarreia),
                        DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                        DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                        DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                        DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                        Febre = Convert.ToBoolean(exame.Febre),
                        IdAgenteSaude = exame.IdAgenteSaude,
                        IdEmpresaSaude = exame.IdEmpresaSaude,
                        IdEstado = exame.IdEstado,
                        IdExame = exame.IdExame,
                        IdMunicipio = exame.IdMunicipio,
                        IdNotificacao = exame.IdNotificacao,
                        IdPaciente = exame.IdPaciente,
                        IdVirusBacteria = exame.IdVirusBacteria,
                        IgG = exame.IgG,
                        IgGIgM = exame.IgMigG,
                        IgM = exame.IgM,
                        MetodoExame = exame.MetodoExame,
                        Nausea = Convert.ToBoolean(exame.Nausea),
                        OutrosSintomas = exame.OutroSintomas,
                        Pcr = exame.Pcr,
                        PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                        RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                        StatusNotificacao = exame.StatusNotificacao,
                        Tosse = Convert.ToBoolean(exame.Tosse)
                    }
                }).ToList();

        public List<ExameBuscaModel> GetByIdEmpresa(int idEempresa, DateTime dataInicio, DateTime dataFim)
        => _context.Exame
              .Where(exameModel => exameModel.IdEmpresaSaude == idEempresa
              && exameModel.DataExame >= dataInicio && exameModel.DataExame <= dataFim)
              .Select(exame => new ExameBuscaModel
              {
                  Cns = exame.IdPacienteNavigation.Cns,
                  Cpf = exame.IdPacienteNavigation.Cpf,
                  NomePaciente = exame.IdPacienteNavigation.Nome,
                  NomeVirusBateria = exame.IdVirusBacteriaNavigation.Nome,
                  Cidade = exame.IdPacienteNavigation.Cidade,
                  Estado = exame.IdEstadoNavigation.Uf,
                  Exame = new ExameModel()
                  {
                      AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                      CodigoColeta = exame.CodigoColeta,
                      Coriza = Convert.ToBoolean(exame.Coriza),
                      DataExame = exame.DataExame,
                      DataInicioSintomas = exame.DataInicioSintomas,
                      Diarreia = Convert.ToBoolean(exame.Diarreia),
                      DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                      DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                      DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                      DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                      Febre = Convert.ToBoolean(exame.Febre),
                      IdAgenteSaude = exame.IdAgenteSaude,
                      IdEmpresaSaude = exame.IdEmpresaSaude,
                      IdEstado = exame.IdEstado,
                      IdExame = exame.IdExame,
                      IdMunicipio = exame.IdMunicipio,
                      IdNotificacao = exame.IdNotificacao,
                      IdPaciente = exame.IdPaciente,
                      IdVirusBacteria = exame.IdVirusBacteria,
                      IgG = exame.IgG,
                      IgGIgM = exame.IgMigG,
                      IgM = exame.IgM,
                      MetodoExame = exame.MetodoExame,
                      Nausea = Convert.ToBoolean(exame.Nausea),
                      OutrosSintomas = exame.OutroSintomas,
                      Pcr = exame.Pcr,
                      PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                      RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                      StatusNotificacao = exame.StatusNotificacao,
                      Tosse = Convert.ToBoolean(exame.Tosse)
                  }
              }).ToList();
        public List<ExameBuscaModel> GetByIdEmpresa(int idEempresa, int lastRecord)
        => _context.Exame
              .Where(exameModel => exameModel.IdEmpresaSaude == idEempresa).Take(lastRecord)
              .Select(exame => new ExameBuscaModel
              {
                  Cns = exame.IdPacienteNavigation.Cns,
                  Cpf = exame.IdPacienteNavigation.Cpf,
                  NomePaciente = exame.IdPacienteNavigation.Nome,
                  NomeVirusBateria = exame.IdVirusBacteriaNavigation.Nome,
                  Cidade = exame.IdPacienteNavigation.Cidade,
                  Estado = exame.IdEstadoNavigation.Uf,
                  Exame = new ExameModel()
                  {
                      AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                      CodigoColeta = exame.CodigoColeta,
                      Coriza = Convert.ToBoolean(exame.Coriza),
                      DataExame = exame.DataExame,
                      DataInicioSintomas = exame.DataInicioSintomas,
                      Diarreia = Convert.ToBoolean(exame.Diarreia),
                      DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                      DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                      DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                      DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                      Febre = Convert.ToBoolean(exame.Febre),
                      IdAgenteSaude = exame.IdAgenteSaude,
                      IdEmpresaSaude = exame.IdEmpresaSaude,
                      IdEstado = exame.IdEstado,
                      IdExame = exame.IdExame,
                      IdMunicipio = exame.IdMunicipio,
                      IdNotificacao = exame.IdNotificacao,
                      IdPaciente = exame.IdPaciente,
                      IdVirusBacteria = exame.IdVirusBacteria,
                      IgG = exame.IgG,
                      IgGIgM = exame.IgMigG,
                      IgM = exame.IgM,
                      MetodoExame = exame.MetodoExame,
                      Nausea = Convert.ToBoolean(exame.Nausea),
                      OutrosSintomas = exame.OutroSintomas,
                      Pcr = exame.Pcr,
                      PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                      RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                      StatusNotificacao = exame.StatusNotificacao,
                      Tosse = Convert.ToBoolean(exame.Tosse)
                  }
              }).ToList();

        public List<ExameBuscaModel> GetByIdMunicipio(int idMunicicpio, DateTime dataInicio, DateTime dataFim)
         => _context.Exame
                .Where(exameModel => exameModel.IdMunicipio == idMunicicpio
                && exameModel.DataExame >= dataInicio && exameModel.DataExame <= dataFim)
                .Select(exame => new ExameBuscaModel
                {
                    Cns = exame.IdPacienteNavigation.Cns,
                    Cpf = exame.IdPacienteNavigation.Cpf,
                    NomePaciente = exame.IdPacienteNavigation.Nome,
                    NomeVirusBateria = exame.IdVirusBacteriaNavigation.Nome,
                    Cidade = exame.IdPacienteNavigation.Cidade,
                    Estado = exame.IdEstadoNavigation.Uf,
                    Exame = new ExameModel()
                    {
                        AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                        CodigoColeta = exame.CodigoColeta,
                        Coriza = Convert.ToBoolean(exame.Coriza),
                        DataExame = exame.DataExame,
                        DataInicioSintomas = exame.DataInicioSintomas,
                        Diarreia = Convert.ToBoolean(exame.Diarreia),
                        DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                        DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                        DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                        DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                        Febre = Convert.ToBoolean(exame.Febre),
                        IdAgenteSaude = exame.IdAgenteSaude,
                        IdEmpresaSaude = exame.IdEmpresaSaude,
                        IdEstado = exame.IdEstado,
                        IdExame = exame.IdExame,
                        IdMunicipio = exame.IdMunicipio,
                        IdNotificacao = exame.IdNotificacao,
                        IdPaciente = exame.IdPaciente,
                        IdVirusBacteria = exame.IdVirusBacteria,
                        IgG = exame.IgG,
                        IgGIgM = exame.IgMigG,
                        IgM = exame.IgM,
                        MetodoExame = exame.MetodoExame,
                        Nausea = Convert.ToBoolean(exame.Nausea),
                        OutrosSintomas = exame.OutroSintomas,
                        Pcr = exame.Pcr,
                        PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                        RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                        StatusNotificacao = exame.StatusNotificacao,
                        Tosse = Convert.ToBoolean(exame.Tosse)
                    }
                }).ToList();
        public List<ExameBuscaModel> GetByIdMunicipio(int idMunicicpio, int lastRecord)
         => _context.Exame
                .Where(exameModel => exameModel.IdMunicipio == idMunicicpio).Take(lastRecord)
                .Select(exame => new ExameBuscaModel
                {
                    Cns = exame.IdPacienteNavigation.Cns,
                    Cpf = exame.IdPacienteNavigation.Cpf,
                    NomePaciente = exame.IdPacienteNavigation.Nome,
                    NomeVirusBateria = exame.IdVirusBacteriaNavigation.Nome,
                    Cidade = exame.IdPacienteNavigation.Cidade,
                    Estado = exame.IdEstadoNavigation.Uf,
                    Exame = new ExameModel()
                    {
                        AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                        CodigoColeta = exame.CodigoColeta,
                        Coriza = Convert.ToBoolean(exame.Coriza),
                        DataExame = exame.DataExame,
                        DataInicioSintomas = exame.DataInicioSintomas,
                        Diarreia = Convert.ToBoolean(exame.Diarreia),
                        DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                        DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                        DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                        DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                        Febre = Convert.ToBoolean(exame.Febre),
                        IdAgenteSaude = exame.IdAgenteSaude,
                        IdEmpresaSaude = exame.IdEmpresaSaude,
                        IdEstado = exame.IdEstado,
                        IdExame = exame.IdExame,
                        IdMunicipio = exame.IdMunicipio,
                        IdNotificacao = exame.IdNotificacao,
                        IdPaciente = exame.IdPaciente,
                        IdVirusBacteria = exame.IdVirusBacteria,
                        IgG = exame.IgG,
                        IgGIgM = exame.IgMigG,
                        IgM = exame.IgM,
                        MetodoExame = exame.MetodoExame,
                        Nausea = Convert.ToBoolean(exame.Nausea),
                        OutrosSintomas = exame.OutroSintomas,
                        Pcr = exame.Pcr,
                        PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                        RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                        StatusNotificacao = exame.StatusNotificacao,
                        Tosse = Convert.ToBoolean(exame.Tosse)
                    }
                }).ToList();
        public List<ExameBuscaModel> GetByIdEstado(int idEstado, DateTime dataInicio, DateTime dataFim)
        => _context.Exame
               .Where(exameModel => (exameModel.IdEstado == idEstado)
               && exameModel.IdEmpresaSaude.Equals(EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
               && (exameModel.IdMunicipio == null)
               && exameModel.DataExame >= dataInicio && exameModel.DataExame <= dataFim)
               .Select(exame => new ExameBuscaModel
               {
                   Cns = exame.IdPacienteNavigation.Cns,
                   Cpf = exame.IdPacienteNavigation.Cpf,
                   NomePaciente = exame.IdPacienteNavigation.Nome,
                   NomeVirusBateria = exame.IdVirusBacteriaNavigation.Nome,
                   Cidade = exame.IdPacienteNavigation.Cidade,
                   Estado = exame.IdEstadoNavigation.Uf,
                   Exame = new ExameModel()
                   {
                       AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                       CodigoColeta = exame.CodigoColeta,
                       Coriza = Convert.ToBoolean(exame.Coriza),
                       DataExame = exame.DataExame,
                       DataInicioSintomas = exame.DataInicioSintomas,
                       Diarreia = Convert.ToBoolean(exame.Diarreia),
                       DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                       DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                       DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                       DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                       Febre = Convert.ToBoolean(exame.Febre),
                       IdAgenteSaude = exame.IdAgenteSaude,
                       IdEmpresaSaude = exame.IdEmpresaSaude,
                       IdEstado = exame.IdEstado,
                       IdExame = exame.IdExame,
                       IdMunicipio = exame.IdMunicipio,
                       IdNotificacao = exame.IdNotificacao,
                       IdPaciente = exame.IdPaciente,
                       IdVirusBacteria = exame.IdVirusBacteria,
                       IgG = exame.IgG,
                       IgGIgM = exame.IgMigG,
                       IgM = exame.IgM,
                       MetodoExame = exame.MetodoExame,
                       Nausea = Convert.ToBoolean(exame.Nausea),
                       OutrosSintomas = exame.OutroSintomas,
                       Pcr = exame.Pcr,
                       PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                       RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                       StatusNotificacao = exame.StatusNotificacao,
                       Tosse = Convert.ToBoolean(exame.Tosse)
                   }
               }).ToList();
        public List<ExameBuscaModel> GetByIdEstado(int idEstado, int lastRecord)
        => _context.Exame
               .Where(exameModel => (exameModel.IdEstado == idEstado)
               && exameModel.IdEmpresaSaude.Equals(EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
               && (exameModel.IdMunicipio == null)).Take(lastRecord)
               .Select(exame => new ExameBuscaModel
               {
                   Cns = exame.IdPacienteNavigation.Cns,
                   Cpf = exame.IdPacienteNavigation.Cpf,
                   NomePaciente = exame.IdPacienteNavigation.Nome,
                   NomeVirusBateria = exame.IdVirusBacteriaNavigation.Nome,
                   Cidade = exame.IdPacienteNavigation.Cidade,
                   Estado = exame.IdEstadoNavigation.Uf,
                   Exame = new ExameModel()
                   {
                       AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                       CodigoColeta = exame.CodigoColeta,
                       Coriza = Convert.ToBoolean(exame.Coriza),
                       DataExame = exame.DataExame,
                       DataInicioSintomas = exame.DataInicioSintomas,
                       Diarreia = Convert.ToBoolean(exame.Diarreia),
                       DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                       DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                       DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                       DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                       Febre = Convert.ToBoolean(exame.Febre),
                       IdAgenteSaude = exame.IdAgenteSaude,
                       IdEmpresaSaude = exame.IdEmpresaSaude,
                       IdEstado = exame.IdEstado,
                       IdExame = exame.IdExame,
                       IdMunicipio = exame.IdMunicipio,
                       IdNotificacao = exame.IdNotificacao,
                       IdPaciente = exame.IdPaciente,
                       IdVirusBacteria = exame.IdVirusBacteria,
                       IgG = exame.IgG,
                       IgGIgM = exame.IgMigG,
                       IgM = exame.IgM,
                       MetodoExame = exame.MetodoExame,
                       Nausea = Convert.ToBoolean(exame.Nausea),
                       OutrosSintomas = exame.OutroSintomas,
                       Pcr = exame.Pcr,
                       PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                       RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                       StatusNotificacao = exame.StatusNotificacao,
                       Tosse = Convert.ToBoolean(exame.Tosse)
                   }
               }).ToList();

        public List<ExameBuscaModel> GetByIdPaciente(int idPaciente)
         => _context.Exame
                .Where(exameModel => exameModel.IdPaciente == idPaciente)
                .Select(exame => new ExameBuscaModel
                {
                    Cns = exame.IdPacienteNavigation.Cns,
                    Cpf = exame.IdPacienteNavigation.Cpf,
                    NomePaciente = exame.IdPacienteNavigation.Nome,
                    NomeVirusBateria = exame.IdVirusBacteriaNavigation.Nome,
                    Cidade = exame.IdPacienteNavigation.Cidade,
                    Exame = new ExameModel()
                    {
                        AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                        CodigoColeta = exame.CodigoColeta,
                        Coriza = Convert.ToBoolean(exame.Coriza),
                        DataExame = exame.DataExame,
                        DataInicioSintomas = exame.DataInicioSintomas,
                        Diarreia = Convert.ToBoolean(exame.Diarreia),
                        DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                        DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                        DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                        DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                        Febre = Convert.ToBoolean(exame.Febre),
                        IdAgenteSaude = exame.IdAgenteSaude,
                        IdEmpresaSaude = exame.IdEmpresaSaude,
                        IdEstado = exame.IdEstado,
                        IdExame = exame.IdExame,
                        IdMunicipio = exame.IdMunicipio,
                        IdNotificacao = exame.IdNotificacao,
                        IdPaciente = exame.IdPaciente,
                        IdVirusBacteria = exame.IdVirusBacteria,
                        IgG = exame.IgG,
                        IgGIgM = exame.IgMigG,
                        IgM = exame.IgM,
                        MetodoExame = exame.MetodoExame,
                        Nausea = Convert.ToBoolean(exame.Nausea),
                        OutrosSintomas = exame.OutroSintomas,
                        Pcr = exame.Pcr,
                        PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                        RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                        StatusNotificacao = exame.StatusNotificacao,
                        Tosse = Convert.ToBoolean(exame.Tosse)
                    },
                    ResponsavelExame = ((exame.IdEmpresaSaude != null) && (exame.IdEmpresaSaude != EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)) ?
                    exame.IdEmpresaSaudeNavigation.Nome + " - " : (exame.IdMunicipio != null ? exame.IdMunicipioNavigation.Nome + " - " : "")
                    + exame.IdEstadoNavigation.Nome,
                }).ToList();

        public ConfiguracaoNotificarModel BuscarConfiguracaoNotificar(int IdEstado, int IdEmpresaExame)
        => _context.Configuracaonotificar
                .Where(c => c.IdEstado == IdEstado && c.IdEmpresaExame == IdEmpresaExame)
                .Select(conf => new ConfiguracaoNotificarModel
                {
                    HabilitadoSms = conf.HabilitadoSms,
                    HabilitadoWhatsapp = conf.HabilitadoWhatsapp,
                    IdConfiguracaoNotificar = conf.IdConfiguracaoNotificar,
                    IdEmpresaExame = conf.IdEmpresaExame,
                    IdEstado = conf.IdEstado,
                    IdMunicipio = conf.IdMunicipio,
                    MensagemCurado = conf.MensagemImunizado,
                    MensagemIndeterminado = conf.MensagemIndeterminado,
                    MensagemPositivo = conf.MensagemPositivo,
                    MensagemNegativo = conf.MensagemNegativo,
                    QuantidadeSmsdisponivel = conf.QuantidadeSmsdisponivel,
                    Sid = conf.Sid,
                    Token = conf.Token
                }).FirstOrDefault();

        public ConfiguracaoNotificarModel BuscarConfiguracaoNotificar(int IdMunicipio)
        => _context.Configuracaonotificar
                .Where(c => c.IdMunicipio == IdMunicipio)
                .Select(conf => new ConfiguracaoNotificarModel
                {
                    HabilitadoSms = conf.HabilitadoSms,
                    HabilitadoWhatsapp = conf.HabilitadoWhatsapp,
                    IdConfiguracaoNotificar = conf.IdConfiguracaoNotificar,
                    IdEmpresaExame = conf.IdEmpresaExame,
                    IdEstado = conf.IdEstado,
                    IdMunicipio = conf.IdMunicipio,
                    MensagemCurado = conf.MensagemImunizado,
                    MensagemIndeterminado = conf.MensagemIndeterminado,
                    MensagemPositivo = conf.MensagemPositivo,
                    MensagemNegativo = conf.MensagemNegativo,
                    QuantidadeSmsdisponivel = conf.QuantidadeSmsdisponivel,
                    Sid = conf.Sid,
                    Token = conf.Token
                }).FirstOrDefault();

        public async System.Threading.Tasks.Task<ExameModel> EnviarSMSResultadoExameAsync(ConfiguracaoNotificarModel configuracaoNotificar, ExameModel exame, PessoaModel pessoa)
        {
            try
            {
                string mensagem = "[MonitoraSUS]Olá, " + pessoa.Nome + ". ";
                if (exame.Resultado.Equals(ExameModel.RESULTADO_POSITIVO))
                    mensagem += configuracaoNotificar.MensagemPositivo;
                else if (exame.Resultado.Equals(ExameModel.RESULTADO_NEGATIVO))
                    mensagem += configuracaoNotificar.MensagemNegativo;
                else if (exame.Resultado.Equals(ExameModel.RESULTADO_RECUPERADO))
                    mensagem += configuracaoNotificar.MensagemCurado;
                else if (exame.Resultado.Equals(ExameModel.RESULTADO_INDETERMINADO))
                    mensagem += configuracaoNotificar.MensagemIndeterminado;
                else if (exame.Resultado.Equals(ExameModel.RESULTADO_AGUARDANDO))
                    return exame;
                else if (exame.Resultado.Equals(ExameModel.RESULTADO_IGMIGG))
                    return exame;

                var cliente = new HttpClient();
                string url = "https://api.smsdev.com.br/send?key=" + configuracaoNotificar.Token + "&type=9&";
                var uri = url + "number=" + pessoa.FoneCelular + "&msg=" + mensagem;
                var resultadoEnvio = await cliente.GetStringAsync(uri);
                ResponseSMSModel jsonResponse = JsonConvert.DeserializeObject<ResponseSMSModel>(resultadoEnvio);
                exame.IdNotificacao = jsonResponse.Id.ToString();
                exame.StatusNotificacao = ExameModel.NOTIFICADO_ENVIADO;

                _context.Update(exame);
                _context.SaveChanges();

                Configuracaonotificar configura = _context.Configuracaonotificar.Where(s => s.IdConfiguracaoNotificar == configuracaoNotificar.IdConfiguracaoNotificar).FirstOrDefault();
                if (configura != null)
                {
                    configura.QuantidadeSmsdisponivel -= 1;
                    _context.Update(configura);
                }
                return exame;
            }
            catch (HttpRequestException)
            {
                return exame;
            }
        }

        public async System.Threading.Tasks.Task<ExameModel> ConsultarSMSExameAsync(ConfiguracaoNotificarModel configuracaoNotificar, ExameModel exame)
        {
            try
            {
                var cliente = new HttpClient();
                string url = "https://api.smsdev.com.br/get?key=" + configuracaoNotificar.Token + "&action=status&";
                var uri = url + "id=" + exame.IdNotificacao;
                var resultadoEnvio = await cliente.GetStringAsync(uri);

                ConsultaSMSModel jsonResponse = JsonConvert.DeserializeObject<ConsultaSMSModel>(resultadoEnvio);
                if (jsonResponse.Descricao.Equals("RECEBIDA"))
                {
                    exame.StatusNotificacao = ExameModel.NOTIFICADO_SIM;
                    _context.Update(exame);
                    _context.SaveChanges();
                }
                else if (jsonResponse.Descricao.Equals("ERRO"))
                {
                    exame.StatusNotificacao = ExameModel.NOTIFICADO_PROBLEMAS;
                    _context.Update(exame);
                    _context.SaveChanges();
                }
            }
            catch (HttpRequestException)
            {
                return exame;
            }
            return exame;
        }

        public List<MonitoraPacienteViewModel> GetByHospital(int idEmpresa, int idVirusBacteria, DateTime dataInicio, DateTime dataFim)
        {
            var monitoraPacientes = _context.Exame
                         .Where(e => e.IdEmpresaSaude == idEmpresa &&
                                 e.IdVirusBacteria == idVirusBacteria &&
                                 e.DataExame >= dataInicio && e.DataExame <= dataFim &&
                                 (e.IdPacienteNavigation.SituacaoSaude.Equals(PessoaModel.SITUACAO_ESTABILIZACAO) ||
                                 e.IdPacienteNavigation.SituacaoSaude.Equals(PessoaModel.SITUACAO_HOSPITALIZADO_INTERNAMENTO) ||
                                 e.IdPacienteNavigation.SituacaoSaude.Equals(PessoaModel.SITUACAO_UTI) ||
                                 e.IdPacienteNavigation.SituacaoSaude.Equals(PessoaModel.SITUACAO_OBITO) ||
                                  e.AguardandoResultado == 1 ||
                                  (e.AguardandoResultado == 0 && (!e.IgM.Equals("N") || !e.Pcr.Equals("N") || !e.IgMigG.Equals("N")))))
                         .OrderByDescending(e => e.DataExame)
                         .Select(exame => new MonitoraPacienteViewModel
                         {
                             Paciente = new PessoaModel
                             {
                                 Cpf = exame.IdPacienteNavigation.Cpf,
                                 Idpessoa = exame.IdPacienteNavigation.Idpessoa,
                                 Nome = exame.IdPacienteNavigation.Nome,
                                 SituacaoSaude = exame.IdPacienteNavigation.SituacaoSaude
                             },
                             DataExame = exame.DataExame,
                             IdExame = exame.IdExame,
                             UltimoResultado = new ExameModel { AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado), IgG = exame.IgG, IgM = exame.IgM, IgGIgM = exame.IgMigG, Pcr = exame.Pcr, MetodoExame = exame.MetodoExame }.Resultado
                         }).ToList();
            List<MonitoraPacienteViewModel> listaMonitoramentoNaoNegativos = BuscarNaoNegativos(idVirusBacteria, monitoraPacientes);
            return listaMonitoramentoNaoNegativos;
        }

        public List<MonitoraPacienteViewModel> GetByCidadeResidenciaPaciente(string cidade, string siglaEstado,
            int idVirusBacteria, DateTime dataInicio, DateTime dataFim)
        {
            var monitoraPacientes = _context.Exame
                 .Where(e => e.IdPacienteNavigation.Cidade.ToUpper().Equals(cidade.ToUpper()) &&
                         e.IdPacienteNavigation.Estado.ToUpper().Equals(siglaEstado.ToUpper()) &&
                         e.IdVirusBacteria == idVirusBacteria &&
                         e.DataExame >= dataInicio && e.DataExame <= dataFim &&
                         (e.IdPacienteNavigation.SituacaoSaude.Equals(PessoaModel.SITUACAO_ESTABILIZACAO) ||
                         e.IdPacienteNavigation.SituacaoSaude.Equals(PessoaModel.SITUACAO_HOSPITALIZADO_INTERNAMENTO) ||
                         e.IdPacienteNavigation.SituacaoSaude.Equals(PessoaModel.SITUACAO_UTI) ||
                         e.IdPacienteNavigation.SituacaoSaude.Equals(PessoaModel.SITUACAO_OBITO) ||
                          e.AguardandoResultado == 1 ||
                          (e.AguardandoResultado == 0 && (!e.IgM.Equals("N") || !e.Pcr.Equals("N") || !e.IgMigG.Equals("N")))))
                 .OrderByDescending(e => e.DataExame)
                 .Select(exame => new MonitoraPacienteViewModel
                 {
                     Paciente = new PessoaModel
                     {
                         Cpf = exame.IdPacienteNavigation.Cpf,
                         Idpessoa = exame.IdPacienteNavigation.Idpessoa,
                         Nome = exame.IdPacienteNavigation.Nome,
                         SituacaoSaude = exame.IdPacienteNavigation.SituacaoSaude
                     },
                     DataExame = exame.DataExame,
                     IdExame = exame.IdExame,
                     UltimoResultado = new ExameModel { AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado), IgG = exame.IgG, IgM = exame.IgM, IgGIgM = exame.IgMigG, Pcr = exame.Pcr, MetodoExame = exame.MetodoExame }.Resultado
                 }).ToList();
            List<MonitoraPacienteViewModel> listaMonitoramentoNaoNegativos = BuscarNaoNegativos(idVirusBacteria, monitoraPacientes);
            return listaMonitoramentoNaoNegativos;
        }

        private List<MonitoraPacienteViewModel> BuscarNaoNegativos(int idVirusBacteria, List<MonitoraPacienteViewModel> monitoraPacientes)
        {
            var listaMonitoramentoNaoNegativos = new List<MonitoraPacienteViewModel>();
            if (monitoraPacientes.Count() > 0)
            {
                var virus = _context.Virusbacteria.Where(v => v.IdVirusBacteria == idVirusBacteria).First();
                var virusBacteria = new VirusBacteriaModel() { IdVirusBacteria = virus.IdVirusBacteria, DiasRecuperacao = virus.DiasRecuperacao, Nome = virus.Nome };
                HashSet<int> idPacientes = new HashSet<int>();
                foreach (MonitoraPacienteViewModel paciente in monitoraPacientes)
                {
                    var situacaoVirus = _context.Situacaopessoavirusbacteria.Where(s => s.Idpessoa == paciente.Paciente.Idpessoa && s.IdVirusBacteria == idVirusBacteria).FirstOrDefault();
                    if (situacaoVirus != null)
                    {
                        if (!idPacientes.Contains(paciente.Paciente.Idpessoa))
                        {
                            idPacientes.Add(paciente.Paciente.Idpessoa);
                            paciente.Descricao = situacaoVirus.Descricao;
                            paciente.DataUltimoMonitoramento = situacaoVirus.DataUltimoMonitoramento;
                            if (situacaoVirus.IdGestor == null)
                                paciente.Gestor = new PessoaModel() { Nome = "-" };
                            else
                                paciente.Gestor = new PessoaModel() { Nome = _context.Pessoa.Where(p => p.Idpessoa == situacaoVirus.IdGestor).First().Nome };
                            paciente.VirusBacteria = virusBacteria;
                            listaMonitoramentoNaoNegativos.Add(paciente);
                        }

                    }
                }
            }

            return listaMonitoramentoNaoNegativos;
        }

        public List<MonitoraPacienteViewModel> GetByEstadoResidenciaPaciente(string siglaEstado,
            int idVirusBacteria, DateTime dataInicio, DateTime dataFim)
        {
            var monitoraPacientes = _context.Exame
                 .Where(e => e.IdPacienteNavigation.Estado.ToUpper().Equals(siglaEstado.ToUpper()) &&
                         e.IdVirusBacteria == idVirusBacteria &&
                         e.DataExame >= dataInicio && e.DataExame <= dataFim &&
                         (e.IdPacienteNavigation.SituacaoSaude.Equals(PessoaModel.SITUACAO_ESTABILIZACAO) ||
                         e.IdPacienteNavigation.SituacaoSaude.Equals(PessoaModel.SITUACAO_HOSPITALIZADO_INTERNAMENTO) ||
                         e.IdPacienteNavigation.SituacaoSaude.Equals(PessoaModel.SITUACAO_UTI) ||
                         e.IdPacienteNavigation.SituacaoSaude.Equals(PessoaModel.SITUACAO_OBITO) ||
                          e.AguardandoResultado == 1 ||
                          (e.AguardandoResultado == 0 && (!e.IgM.Equals("N") || !e.Pcr.Equals("N") || !e.IgMigG.Equals("N"))))).OrderByDescending(e => e.DataExame)
                 .Select(exame => new MonitoraPacienteViewModel
                 {
                     Paciente = new PessoaModel
                     {
                         Cpf = exame.IdPacienteNavigation.Cpf,
                         Idpessoa = exame.IdPacienteNavigation.Idpessoa,
                         Nome = exame.IdPacienteNavigation.Nome,
                         SituacaoSaude = exame.IdPacienteNavigation.SituacaoSaude
                     },
                     DataExame = exame.DataExame,
                     IdExame = exame.IdExame,
                     UltimoResultado = new ExameModel { AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado), IgG = exame.IgG, IgM = exame.IgM, IgGIgM = exame.IgMigG, Pcr = exame.Pcr, MetodoExame = exame.MetodoExame }.Resultado
                 }).ToList();
            List<MonitoraPacienteViewModel> listaMonitoramentoNaoNegativos = BuscarNaoNegativos(idVirusBacteria, monitoraPacientes);
            return listaMonitoramentoNaoNegativos;
        }

        public List<ExameModel> GetExamesRelizadosData(int idPaciente, int idVirusBacteria, DateTime dateExame, string metodoExame)
        {
            var exames = _context.Exame.Where(exameModel => exameModel.IdVirusBacteria == idVirusBacteria &&
                         exameModel.IdPaciente == idPaciente && exameModel.MetodoExame.Equals(metodoExame) &&
                         dateExame.ToString("dd/MM/yyyy").Equals(exameModel.DataExame.ToString("dd/MM/yyyy")))
                         .Select(exame => new ExameModel
                         {
                             IdExame = exame.IdExame,
                         }).ToList();
            return exames;
        }

        public List<TotalEstadoMunicipioBairro> GetTotaisPopulacaoByEstado(string siglaEstado)
            => ConvertToEstadoMunicipioBairro(
            _context.Exame
                 .Where(exameModel => exameModel.IdPacienteNavigation.Estado.Equals(siglaEstado))
                 .Select(exame => new ExameCompletoModel
                 {
                     IdAreaAtuacao = exame.IdPacienteNavigation.IdAreaAtuacao,
                     IdVirusBacteria = exame.IdVirusBacteria,
                     IdExame = exame.IdExame,
                     IdPaciente = exame.IdPaciente,
                     IdAgenteSaude = exame.IdAgenteSaude,
                     DataExame = exame.DataExame,
                     DataInicioSintomas = exame.DataInicioSintomas,
                     IgG = exame.IgG,
                     IgM = exame.IgM,
                     Pcr = exame.Pcr,
                     IgGIgM = exame.IgMigG,
                     MetodoExame = exame.MetodoExame,
                     IdEstado = exame.IdEstado,
                     IdMunicipio = exame.IdMunicipio,
                     IdEmpresaSaude = exame.IdEmpresaSaude,
                     CodigoColeta = exame.CodigoColeta,
                     StatusNotificacao = exame.StatusNotificacao,
                     IdNotificacao = exame.IdNotificacao,
                     AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                     RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                     Coriza = Convert.ToBoolean(exame.Coriza),
                     Diarreia = Convert.ToBoolean(exame.Diarreia),
                     DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                     DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                     DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                     DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                     Febre = Convert.ToBoolean(exame.Febre),
                     Nausea = Convert.ToBoolean(exame.Nausea),
                     PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                     Tosse = Convert.ToBoolean(exame.Tosse),
                     UF = exame.IdPacienteNavigation.Estado,
                     Municipio = exame.IdPacienteNavigation.Cidade,
                     Bairro = "",
                     OutrosSintomas = exame.OutroSintomas,
                 }).ToList().GroupBy(e => new { Estado = e.UF, Municipio = e.Municipio, Resultado = e.Resultado })
                 .Select(g => new TotalPorResultadoExame
                 {
                     Estado = g.Key.Estado,
                     Municipio = g.Key.Municipio,
                     IdEmpresaSaude = EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO,
                     Resultado = g.Key.Resultado,
                     Bairro = "",
                     Total = g.Count()
                 }).ToList());
        public List<TotalEstadoMunicipioBairro> GetTotaisPopulacaoByMunicipio(string siglaEstado, string cidade)
            => ConvertToEstadoMunicipioBairro(
            _context.Exame
                 .Where(exameModel => exameModel.IdPacienteNavigation.Estado.Equals(siglaEstado) &&
                    exameModel.IdPacienteNavigation.Cidade.Equals(cidade))
                 .Select(exame => new ExameCompletoModel
                 {
                     IdAreaAtuacao = exame.IdPacienteNavigation.IdAreaAtuacao,
                     IdVirusBacteria = exame.IdVirusBacteria,
                     IdExame = exame.IdExame,
                     IdPaciente = exame.IdPaciente,
                     IdAgenteSaude = exame.IdAgenteSaude,
                     DataExame = exame.DataExame,
                     DataInicioSintomas = exame.DataInicioSintomas,
                     IgG = exame.IgG,
                     IgM = exame.IgM,
                     Pcr = exame.Pcr,
                     IgGIgM = exame.IgMigG,
                     MetodoExame = exame.MetodoExame,
                     IdEstado = exame.IdEstado,
                     IdMunicipio = exame.IdMunicipio,
                     IdEmpresaSaude = exame.IdEmpresaSaude,
                     CodigoColeta = exame.CodigoColeta,
                     StatusNotificacao = exame.StatusNotificacao,
                     IdNotificacao = exame.IdNotificacao,
                     AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                     RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                     Coriza = Convert.ToBoolean(exame.Coriza),
                     Diarreia = Convert.ToBoolean(exame.Diarreia),
                     DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                     DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                     DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                     DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                     Febre = Convert.ToBoolean(exame.Febre),
                     Nausea = Convert.ToBoolean(exame.Nausea),
                     PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                     Tosse = Convert.ToBoolean(exame.Tosse),
                     UF = exame.IdPacienteNavigation.Estado,
                     Municipio = exame.IdPacienteNavigation.Cidade,
                     Bairro = exame.IdPacienteNavigation.Bairro.ToUpper().Trim(),
                     OutrosSintomas = exame.OutroSintomas,
                 }).ToList().GroupBy(e => new { Estado = e.UF, Municipio = e.Municipio, Bairro = e.Bairro, Resultado = e.Resultado })
                 .Select(g => new TotalPorResultadoExame
                 {
                     Estado = g.Key.Estado,
                     Municipio = g.Key.Municipio,
                     IdEmpresaSaude = EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO,
                     Resultado = g.Key.Resultado,
                     Bairro = g.Key.Bairro,
                     Total = g.Count()
                 }).ToList());


        public List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByEstado(int idEstado)
            => ConvertToEstadoMunicipioBairro(
            _context.Exame
                 .Where(exameModel => exameModel.IdEstado == idEstado && exameModel.IdEmpresaSaude == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
                 .Select(exame => new ExameCompletoModel
                 {
                     IdAreaAtuacao = exame.IdPacienteNavigation.IdAreaAtuacao,
                     IdVirusBacteria = exame.IdVirusBacteria,
                     IdExame = exame.IdExame,
                     IdPaciente = exame.IdPaciente,
                     IdAgenteSaude = exame.IdAgenteSaude,
                     DataExame = exame.DataExame,
                     DataInicioSintomas = exame.DataInicioSintomas,
                     IgG = exame.IgG,
                     IgM = exame.IgM,
                     Pcr = exame.Pcr,
                     IgGIgM = exame.IgMigG,
                     MetodoExame = exame.MetodoExame,
                     IdEstado = exame.IdEstado,
                     IdMunicipio = exame.IdMunicipio,
                     IdEmpresaSaude = exame.IdEmpresaSaude,
                     CodigoColeta = exame.CodigoColeta,
                     StatusNotificacao = exame.StatusNotificacao,
                     IdNotificacao = exame.IdNotificacao,
                     AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                     RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                     Coriza = Convert.ToBoolean(exame.Coriza),
                     Diarreia = Convert.ToBoolean(exame.Diarreia),
                     DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                     DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                     DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                     DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                     Febre = Convert.ToBoolean(exame.Febre),
                     Nausea = Convert.ToBoolean(exame.Nausea),
                     PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                     Tosse = Convert.ToBoolean(exame.Tosse),
                     UF = exame.IdPacienteNavigation.Estado,
                     Municipio = exame.IdPacienteNavigation.Cidade,
                     Bairro = "",
                     OutrosSintomas = exame.OutroSintomas,
                 }).ToList().GroupBy(e => new { Estado = e.UF, Municipio = e.Municipio, Resultado = e.Resultado })
                 .Select(g => new TotalPorResultadoExame
                 {
                     Estado = g.Key.Estado,
                     Municipio = g.Key.Municipio,
                     IdEmpresaSaude = EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO,
                     Resultado = g.Key.Resultado,
                     Bairro = "",
                     Total = g.Count()
                 }).ToList());

        public List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByMunicipio(int idMunicipio)
            => ConvertToEstadoMunicipioBairro(
            _context.Exame
                 .Where(exameModel => exameModel.IdMunicipio == idMunicipio)
                 .Select(exame => new ExameCompletoModel
                 {
                     IdAreaAtuacao = exame.IdPacienteNavigation.IdAreaAtuacao,
                     IdVirusBacteria = exame.IdVirusBacteria,
                     IdExame = exame.IdExame,
                     IdPaciente = exame.IdPaciente,
                     IdAgenteSaude = exame.IdAgenteSaude,
                     DataExame = exame.DataExame,
                     DataInicioSintomas = exame.DataInicioSintomas,
                     IgG = exame.IgG,
                     IgM = exame.IgM,
                     Pcr = exame.Pcr,
                     IgGIgM = exame.IgMigG,
                     MetodoExame = exame.MetodoExame,
                     IdEstado = exame.IdEstado,
                     IdMunicipio = exame.IdMunicipio,
                     IdEmpresaSaude = exame.IdEmpresaSaude,
                     CodigoColeta = exame.CodigoColeta,
                     StatusNotificacao = exame.StatusNotificacao,
                     IdNotificacao = exame.IdNotificacao,
                     AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                     RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                     Coriza = Convert.ToBoolean(exame.Coriza),
                     Diarreia = Convert.ToBoolean(exame.Diarreia),
                     DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                     DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                     DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                     DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                     Febre = Convert.ToBoolean(exame.Febre),
                     Nausea = Convert.ToBoolean(exame.Nausea),
                     PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                     Tosse = Convert.ToBoolean(exame.Tosse),
                     UF = exame.IdPacienteNavigation.Estado,
                     Municipio = exame.IdPacienteNavigation.Cidade,
                     Bairro = exame.IdPacienteNavigation.Bairro,
                     OutrosSintomas = exame.OutroSintomas,
                 }).ToList().GroupBy(e => new { Estado = e.UF, Municipio = e.Municipio, Bairro = e.Bairro, Resultado = e.Resultado })
                 .Select(g => new TotalPorResultadoExame
                 {
                     Estado = g.Key.Estado,
                     Municipio = g.Key.Municipio,
                     IdEmpresaSaude = EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO,
                     Resultado = g.Key.Resultado,
                     Bairro = g.Key.Bairro,
                     Total = g.Count()
                 }).ToList());

        public List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByEmpresa(int idEempresa)
        => ConvertToEstadoMunicipioBairro(
            _context.Exame
                 .Where(exameModel => exameModel.IdEmpresaSaude == idEempresa)
                 .Select(exame => new ExameCompletoModel
                 {
                     IdAreaAtuacao = exame.IdPacienteNavigation.IdAreaAtuacao,
                     IdVirusBacteria = exame.IdVirusBacteria,
                     IdExame = exame.IdExame,
                     IdPaciente = exame.IdPaciente,
                     IdAgenteSaude = exame.IdAgenteSaude,
                     DataExame = exame.DataExame,
                     DataInicioSintomas = exame.DataInicioSintomas,
                     IgG = exame.IgG,
                     IgM = exame.IgM,
                     Pcr = exame.Pcr,
                     IgGIgM = exame.IgMigG,
                     MetodoExame = exame.MetodoExame,
                     IdEstado = exame.IdEstado,
                     IdMunicipio = exame.IdMunicipio,
                     IdEmpresaSaude = exame.IdEmpresaSaude,
                     CodigoColeta = exame.CodigoColeta,
                     StatusNotificacao = exame.StatusNotificacao,
                     IdNotificacao = exame.IdNotificacao,
                     AguardandoResultado = Convert.ToBoolean(exame.AguardandoResultado),
                     RelatouSintomas = Convert.ToBoolean(exame.RelatouSintomas),
                     Coriza = Convert.ToBoolean(exame.Coriza),
                     Diarreia = Convert.ToBoolean(exame.Diarreia),
                     DificuldadeRespiratoria = Convert.ToBoolean(exame.DificuldadeRespiratoria),
                     DorAbdominal = Convert.ToBoolean(exame.DorAbdominal),
                     DorGarganta = Convert.ToBoolean(exame.DorGarganta),
                     DorOuvido = Convert.ToBoolean(exame.DorOuvido),
                     Febre = Convert.ToBoolean(exame.Febre),
                     Nausea = Convert.ToBoolean(exame.Nausea),
                     PerdaOlfatoPaladar = Convert.ToBoolean(exame.PerdaOlfatoPaladar),
                     Tosse = Convert.ToBoolean(exame.Tosse),
                     UF = exame.IdPacienteNavigation.Estado,
                     Municipio = exame.IdPacienteNavigation.Cidade,
                     Bairro = "",
                     OutrosSintomas = exame.OutroSintomas,
                 }).ToList().GroupBy(e => new { Estado = e.UF, Municipio = e.Municipio, Resultado = e.Resultado })
                 .Select(g => new TotalPorResultadoExame
                 {
                     Estado = g.Key.Estado,
                     Municipio = g.Key.Municipio,
                     IdEmpresaSaude = idEempresa,
                     Resultado = g.Key.Resultado,
                     Bairro = "",
                     Total = g.Count()
                 }).ToList());

        private List<TotalEstadoMunicipioBairro> ConvertToEstadoMunicipioBairro(List<TotalPorResultadoExame> resultados)
        {
            List<TotalEstadoMunicipioBairro> totalEstadoMunicipioBairros = new List<TotalEstadoMunicipioBairro>();
            foreach (TotalPorResultadoExame totalPorResultado in resultados)
            {
                bool achou = false;
                foreach (TotalEstadoMunicipioBairro totalEMB in totalEstadoMunicipioBairros)
                {
                    if (totalEMB.Estado.Equals(totalPorResultado.Estado) && totalEMB.Municipio.Equals(totalPorResultado.Municipio) &&
                       (totalEMB.IdEmpresaSaude == totalPorResultado.IdEmpresaSaude) && totalEMB.Bairro.Equals(totalPorResultado.Bairro))
                    {
                        AtualizarTotais(totalPorResultado, totalEMB);
                        achou = true;
                        break;
                    }
                }
                if (!achou)
                {
                    TotalEstadoMunicipioBairro totalEMB = new TotalEstadoMunicipioBairro()
                    {
                        Bairro = totalPorResultado.Bairro,
                        Estado = totalPorResultado.Estado,
                        Municipio = totalPorResultado.Municipio,
                        IdEmpresaSaude = totalPorResultado.IdEmpresaSaude,
                        TotalRecuperados = 0,
                        TotalAguardando = 0,
                        TotalIgGIgM = 0,
                        TotalIndeterminados = 0,
                        TotalNegativos = 0,
                        TotalPositivos = 0
                    };
                    AtualizarTotais(totalPorResultado, totalEMB);
                    totalEstadoMunicipioBairros.Add(totalEMB);
                }
            }
            return totalEstadoMunicipioBairros;
        }


        private void AtualizarTotais(TotalPorResultadoExame totalPorResultado, TotalEstadoMunicipioBairro totalEMB)
        {
            if (totalPorResultado.Resultado.Equals(ExameModel.RESULTADO_POSITIVO))
                totalEMB.TotalPositivos += totalPorResultado.Total;
            if (totalPorResultado.Resultado.Equals(ExameModel.RESULTADO_NEGATIVO))
                totalEMB.TotalNegativos += totalPorResultado.Total;
            if (totalPorResultado.Resultado.Equals(ExameModel.RESULTADO_RECUPERADO))
                totalEMB.TotalRecuperados += totalPorResultado.Total;
            if (totalPorResultado.Resultado.Equals(ExameModel.RESULTADO_INDETERMINADO))
                totalEMB.TotalIndeterminados += totalPorResultado.Total;
            if (totalPorResultado.Resultado.Equals(ExameModel.RESULTADO_AGUARDANDO))
                totalEMB.TotalAguardando += totalPorResultado.Total;
            if (totalPorResultado.Resultado.Equals(ExameModel.RESULTADO_IGMIGG))
                totalEMB.TotalIgGIgM += totalPorResultado.Total;
        }

        private IndiceItemArquivoImportacao IndexaColunasArquivoImportacao(string cabecalho)
        {

            var itens = cabecalho.Split(';');

            var indices = new IndiceItemArquivoImportacao();
            for (int i = 0; i < itens.Length; i++)
            {
                if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.UNIDADE_SOLICITANTE)))
                    indices.IndiceNomeEmpresa = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CNES_UNIDADE_SOLCITANTE).ToUpper()))
                    indices.IndiceCnesEmpresa = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.MUNICIPIO_DO_SOLICITANTE).ToUpper()))
                    indices.IndiceCidadeEmpresa = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.ESTADO_DO_SOLICITANTE).ToUpper()))
                    indices.IndiceEstadoEmpresa = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CNS_DO_PACIENTE).ToUpper()))
                    indices.IndiceCnsPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.NOME_PACIENTE).ToUpper()))
                    indices.IndiceNomePaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.SEXO_PACIENTE).ToUpper()))
                    indices.IndiceSexoPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DATA_DE_NASCIMENTO_PACIENTE).ToUpper()))
                    indices.IndiceDataNascimentoPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.TIPO_DOCUMENTO_1).ToUpper()))
                    indices.IndiceTipoDocumento1Paciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DOCUMENTO_1).ToUpper()))
                    indices.IndiceDocumento1Paciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.TIPO_DOCUMENTO_2).ToUpper()))
                    indices.IndiceTipoDocumento2Paciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DOCUMENTO_2).ToUpper()))
                    indices.IndiceDocumento2Paciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.ENDERECO_PACIENTE).ToUpper()))
                    indices.IndiceRuaPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.BAIRRO_PACIENTE).ToUpper()))
                    indices.IndiceBairroPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CEP_PACIENTE).ToUpper()))
                    indices.IndiceCepPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.MUNICIPIO_PACIENTE).ToUpper()))
                    indices.IndiceCidadePaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.ESTADO_PACIENTE).ToUpper()))
                    indices.IndiceEstadoPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CELULAR_PACIENTE).ToUpper()))
                    indices.IndiceFoneCelularPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.TIPO_EXAME).ToUpper()))
                    indices.IndiceTipoExame = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.METODO_EXAME).ToUpper()))
                    indices.IndiceMetodoExame = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CODIGO_DA_AMOSTRA).ToUpper()))
                    indices.IndiceCodigoColeta = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DATA_DA_COLETA).ToUpper()))
                    indices.IndiceDataExame = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DATA_INICIO_SINTOMAS).ToUpper()))
                    indices.IndiceDataInicioSintomas = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.OBSERVACOES_RESULTADO).ToUpper()))
                    indices.IndiceObservacaoExame = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO).ToUpper()))
                    indices.IndiceResultadoExame = i;
            }

            var planilhaValida = false;
            if (indices.IndiceNomeEmpresa != -1 && indices.IndiceCnesEmpresa != -1 && indices.IndiceCidadeEmpresa != -1 && indices.IndiceEstadoEmpresa != -1 && indices.IndiceFoneCelularPaciente != -1 &&
                indices.IndiceCnsPaciente != -1 && indices.IndiceNomePaciente != -1 && indices.IndiceDataNascimentoPaciente != -1 && indices.IndiceTipoDocumento1Paciente != -1 && indices.IndiceDocumento1Paciente != -1 &&
                indices.IndiceTipoDocumento2Paciente != -1 && indices.IndiceDocumento2Paciente != -1 && indices.IndiceRuaPaciente != -1 && indices.IndiceBairroPaciente != -1 && indices.IndiceCepPaciente != -1 &&
                indices.IndiceCidadePaciente != -1 && indices.IndiceEstadoPaciente != -1 && indices.IndiceTipoExame != -1 && indices.IndiceMetodoExame != -1 && indices.IndiceCodigoColeta != -1 && indices.IndiceDataExame != -1 &&
                indices.IndiceDataInicioSintomas != -1 && indices.IndiceResultadoExame != -1 && indices.IndiceObservacaoExame != -1 && indices.IndiceSexoPaciente != -1)
                planilhaValida = true;

            return planilhaValida ? indices : null;
        }


        private string GetMetodoExame(string exame, string metodo, string resultado)
        {
            switch (metodo)
            {
                case "PCR":
                    if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_PCR))
                        return "S";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_NAO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_PCR))
                        return "N";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_SOLICITAR_NOVA_COLETA)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_PCR))
                        return "I";
                    else
                        return "I";

                case "IGG":
                    if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "S";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_NAO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "N";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_SOLICITAR_NOVA_COLETA)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "I";
                    else
                        return "I";

                case "IGM":
                    if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG))
                        return "S";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_NAO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG))
                        return "N";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_SOLICITAR_NOVA_COLETA)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG))
                        return "I";
                    else
                        return "I";

                case "IGG/IGM":
                    if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "S";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_NAO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "N";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_SOLICITAR_NOVA_COLETA)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "I";
                    else
                        return "I";

                default:
                    return "I";
            }
        }

        private int GetIdVirusBacteriaItemImportacao(string exame, List<VirusBacteriaModel> virus)
        {
            string[] e = exame.Split(',');

            foreach (var item in virus)
            {
                if (item.Nome.ToUpper().Contains(e[0]))
                {
                    return item.IdVirusBacteria;
                }
            }

            return virus[0].IdVirusBacteria;
        }

        private PessoaModel CreatePessoaModelByExame(ExameViewModel exameViewModel, IPessoaService pessoaService)
        {
            var user = pessoaService.GetByCpf(exameViewModel.Paciente.Cpf.ToUpper());
            if (user == null)
            {
                exameViewModel.Paciente.Idpessoa = 0;
                exameViewModel.Paciente.IdAreaAtuacao = exameViewModel.Exame.IdAreaAtuacao;
                if (exameViewModel.Exame.AguardandoResultado == true || exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_POSITIVO) ||
                    exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_IGMIGG))
                    exameViewModel.Paciente.SituacaoSaude = PessoaModel.SITUACAO_ISOLAMENTO;
            }
            else
            {
                exameViewModel.Paciente.Idpessoa = user.Idpessoa;
                exameViewModel.Paciente.IdAreaAtuacao = exameViewModel.Exame.IdAreaAtuacao;
                if ((exameViewModel.Paciente.SituacaoSaude.Equals(PessoaModel.SITUACAO_SAUDAVEL) & !exameViewModel.Paciente.SituacaoSaude.Equals(PessoaModel.SITUACAO_SAUDAVEL) && exameViewModel.Exame.AguardandoResultado == false)
                    || (exameViewModel.Paciente.SituacaoSaude.Equals(PessoaModel.SITUACAO_SAUDAVEL) && exameViewModel.Exame.AguardandoResultado == true))
                {
                    exameViewModel.Paciente.SituacaoSaude = PessoaModel.SITUACAO_ISOLAMENTO;
                }
                else if (exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_NEGATIVO) || exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_RECUPERADO)
                  || exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_INDETERMINADO))
                {
                    var virus = (new VirusBacteriaService(_context)).GetById(exameViewModel.Exame.IdVirusBacteria);
                    DateTime dataMinima = DateTime.Now.AddDays(virus.DiasRecuperacao * (-1));
                    var exames = GetByIdPaciente(user.Idpessoa).Where(e => e.Exame.DataExame >= dataMinima).ToList();
                    if (exames.Count() <= 1 && exameViewModel.Paciente.SituacaoSaude.Equals(PessoaModel.SITUACAO_ISOLAMENTO))
                    {
                        exameViewModel.Paciente.SituacaoSaude = PessoaModel.SITUACAO_SAUDAVEL;
                    }
                }
            }
            return exameViewModel.Paciente;
        }


        public void Import(IFormFile file, UsuarioViewModel agente)
        {
            var _pessoaTrabalhaMunicipioService = new PessoaTrabalhaMunicipioService(_context);
            var _pessoaTrabalhaEstadoContext = new PessoaTrabalhaEstadoService(_context);
            var _municipioGeoService = new MunicipioGeoService(_context);
            var _virusBacteriaService = new VirusBacteriaService(_context);
            var _pessoaService = new PessoaService(_context);
            var _empresaExameService = new EmpresaExameService(_context);
            var _situacaoPessoaService = new SituacaoVirusBacteriaService(_context);
            var _municipioService = new MunicipioService(_context);
            var secretarioMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(agente.UsuarioModel.IdPessoa);
            var secretarioEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(agente.UsuarioModel.IdPessoa);
            var exames = new List<ExameViewModel>();
            var indices = new IndiceItemArquivoImportacao();
            var listVirusBacteria = _virusBacteriaService.GetAll();
            MunicipioGeoModel cidadePaciente = new MunicipioGeoModel(),
                              cidadeEmpresa = new MunicipioGeoModel();


            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                indices = IndexaColunasArquivoImportacao(reader.ReadLine());

                if (indices == null)
                    throw new ServiceException("Essa planilha não possui as informações necessárias para fazer a importação, " +
                                                        "por favor verifique a planilha e tente novamente.");

                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLine().Split(';');

                    cidadePaciente = _municipioGeoService.GetByName(line[53]);
                    cidadeEmpresa = _municipioGeoService.GetByName(line[7]);

                    var exame = new ExameViewModel();
                    exame.Paciente = new PessoaModel
                    {
                        Nome = line[indices.IndiceNomePaciente],
                        Cidade = line[indices.IndiceCidadePaciente],
                        Cpf = line[indices.IndiceTipoDocumento1Paciente].Equals("CPF") && Methods.ValidarCpf(line[indices.IndiceDocumento1Paciente]) ?
                                Methods.RemoveSpecialsCaracts(line[indices.IndiceDocumento1Paciente]) : line[indices.IndiceTipoDocumento2Paciente].Equals("CPF") && Methods.ValidarCpf(line[indices.IndiceDocumento2Paciente]) ?
                                Methods.RemoveSpecialsCaracts(line[indices.IndiceDocumento2Paciente]) : "",

                        Sexo = line[indices.IndiceSexoPaciente].Equals("FEMININO") ? "Feminino" : "Masculino",
                        Cep = line[indices.IndiceCepPaciente].Length > 0 ? Methods.RemoveSpecialsCaracts(line[indices.IndiceCepPaciente]) : "00000000",
                        Rua = line[indices.IndiceRuaPaciente].Length > 0 ? line[indices.IndiceRuaPaciente].Split('-')[0] : "NÃO INFORMADO",
                        Bairro = line[indices.IndiceBairroPaciente].Length > 0 ? line[indices.IndiceBairroPaciente] : "NAO INFORMADO",
                        Estado = line[indices.IndiceEstadoPaciente],
                        Numero = line[indices.IndiceRuaPaciente].Length > 0 && line[indices.IndiceRuaPaciente].Split('-').Length >= 2 ? line[indices.IndiceRuaPaciente].Split('-')[1].Trim() : "",
                        Complemento = line[indices.IndiceRuaPaciente].Length > 0 && line[indices.IndiceRuaPaciente].Split('-').Length == 3 ? line[indices.IndiceRuaPaciente].Split('-')[2].Trim() : "",
                        FoneCelular = line[indices.IndiceFoneCelularPaciente],
                        DataNascimento = Convert.ToDateTime(line[indices.IndiceDataNascimentoPaciente]),
                        IdAreaAtuacao = 0,
                        Longitude = cidadePaciente != null ? cidadePaciente.Longitude.ToString() : "0",
                        Latitude = cidadePaciente != null ? cidadePaciente.Latitude.ToString() : "0",
                        Cns = line[indices.IndiceCnsPaciente],
                        Profissao = "NÃO INFORMADA",
                        OutrasComorbidades = "",
                        OutrosSintomas = ""
                    };

                    exame.Exame.IdVirusBacteria = GetIdVirusBacteriaItemImportacao(line[indices.IndiceTipoExame], listVirusBacteria);
                    exame.Exame.IdAgenteSaude = agente.UsuarioModel.IdPessoa;
                    exame.Exame.DataExame = Convert.ToDateTime(line[indices.IndiceDataExame]);
                    exame.Exame.DataInicioSintomas = line[indices.IndiceDataInicioSintomas].Equals("") ? Convert.ToDateTime(line[indices.IndiceDataExame]) : Convert.ToDateTime(line[indices.IndiceDataInicioSintomas]);
                    exame.Exame.IgG = line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && !line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) ? GetMetodoExame(line[indices.IndiceMetodoExame], IndiceItemArquivoImportacao.METODO_IGG, line[indices.IndiceResultadoExame].Length > 0 ? line[indices.IndiceResultadoExame] : line[indices.IndiceObservacaoExame]) : "N";
                    exame.Exame.IgM = line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) && !line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) ? GetMetodoExame(line[indices.IndiceMetodoExame], IndiceItemArquivoImportacao.METODO_IGM, line[indices.IndiceResultadoExame].Length > 0 ? line[indices.IndiceResultadoExame] : line[indices.IndiceObservacaoExame]) : "N";
                    exame.Exame.Pcr = line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_PCR) ? GetMetodoExame(line[indices.IndiceMetodoExame], IndiceItemArquivoImportacao.METODO_PCR, line[indices.IndiceResultadoExame].Length > 0 ? line[indices.IndiceResultadoExame] : line[indices.IndiceObservacaoExame]) : "N";
                    exame.Exame.IgGIgM = line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) ? GetMetodoExame(line[indices.IndiceMetodoExame], IndiceItemArquivoImportacao.METODO_IGG_IGM, line[indices.IndiceResultadoExame].Length > 0 ? line[indices.IndiceResultadoExame] : line[indices.IndiceObservacaoExame]) : "N";
                    if (secretarioMunicipio != null)
                        exame.Exame.IdMunicipio = secretarioMunicipio.IdMunicipio;
                    else
                        exame.Exame.IdMunicipio = null;
                    exame.Exame.IdEstado = secretarioMunicipio != null ? Convert.ToInt32(_municipioService.GetById(secretarioMunicipio.IdMunicipio).Uf) : secretarioEstado.IdEstado;
                    exame.EmpresaExame = new EmpresaExameModel
                    {
                        Cnpj = "NÃO INFORMADO",
                        Nome = line[indices.IndiceNomeEmpresa],
                        Cnes = line[indices.IndiceCnesEmpresa],
                        Cidade = line[indices.IndiceCidadeEmpresa],
                        Latitude = cidadeEmpresa != null ? cidadeEmpresa.Latitude.ToString() : "0",
                        Longitude = cidadeEmpresa != null ? cidadeEmpresa.Longitude.ToString() : "0",
                        Estado = line[indices.IndiceEstadoEmpresa],
                        Rua = "NÃO INFORMADO",
                        Bairro = "NÃO INFORMADO",
                        Cep = "00000000",
                        FoneCelular = "00000000000",
                    };
                    exame.Exame.IdAreaAtuacao = 0;
                    exame.Exame.CodigoColeta = line[indices.IndiceCodigoColeta];
                    exame.Paciente.Cns = line[indices.IndiceCnsPaciente];
                    exames.Add(exame);
                }
            }

            foreach (var item in exames)
            {
                var pessoa = !String.IsNullOrWhiteSpace(item.Paciente.Cpf) ?
                           _pessoaService.GetByCpf(item.Paciente.Cpf) : !String.IsNullOrWhiteSpace(item.Paciente.Cns) ?
                           _pessoaService.GetByCns(item.Paciente.Cns) : new PessoaModel { Idpessoa = -1 };

                if (pessoa == null || pessoa.Idpessoa == -1)
                {
                    pessoa = _pessoaService.Insert(item.Paciente);
                    item.Paciente.Idpessoa = pessoa.Idpessoa;
                    item.Paciente.Cpf = pessoa.Cpf;
                }
                else
                {
                    item.Paciente.Idpessoa = pessoa.Idpessoa;
                    item.Paciente.Cpf = pessoa.Cpf;
                    _pessoaService.Update(item.Paciente, true);
                }


                var empresa = _empresaExameService.GetByCNES(item.EmpresaExame.Cnes);

                if (empresa == null)
                    _empresaExameService.Insert(item.EmpresaExame);

                item.Exame.IdEmpresaSaude = _empresaExameService.GetByCNES(item.EmpresaExame.Cnes).Id;

                var situacaoPessoa = _situacaoPessoaService.GetById(item.Paciente.Idpessoa, item.Exame.IdVirusBacteria);

                if (situacaoPessoa == null)
                    _situacaoPessoaService.Insert(CreateSituacaoPessoaModelByExame(item, situacaoPessoa, _pessoaService));
                else
                    _situacaoPessoaService.Update(CreateSituacaoPessoaModelByExame(item, situacaoPessoa, _pessoaService));


                var exame = GetByIdColeta(item.Exame.CodigoColeta);

                var ex = new ExameModel
                {
                    IdExame = exame != null ? exame.IdExame : 0,
                    IdPaciente = item.Paciente.Idpessoa,
                    IdVirusBacteria = item.Exame.IdVirusBacteria,
                    IdAgenteSaude = item.Exame.IdAgenteSaude,
                    DataExame = item.Exame.DataExame,
                    DataInicioSintomas = item.Exame.DataInicioSintomas,
                    IgG = item.Exame.IgG,
                    IgM = item.Exame.IgM,
                    Pcr = item.Exame.Pcr,
                    IgGIgM = item.Exame.IgGIgM,
                    IdMunicipio = item.Exame.IdMunicipio,
                    IdEstado = item.Exame.IdEstado,
                    IdEmpresaSaude = item.Exame.IdEmpresaSaude,
                    IdAreaAtuacao = item.Exame.IdAreaAtuacao,
                    CodigoColeta = item.Exame.CodigoColeta,
                    IdNotificacao = "",
                    OutrosSintomas = "",
                    MetodoExame = "F",
                    StatusNotificacao = exame != null ? exame.StatusNotificacao : "N"
                };

                var check = GetExamesRelizadosData(ex.IdPaciente, item.Exame.IdVirusBacteria, item.Exame.DataExame, item.Exame.MetodoExame);
                if (exame != null)
                {
                    if (check.Count > 0)
                    {
                        var status = false;
                        foreach (var index in check)
                        {
                            if (index.IdExame == exame.IdExame)
                                status = true;
                        }

                        if (status)
                        {
                            _context.Update(ex);
                            _context.SaveChanges();
                        }
                    }
                }
                else
                {
                    if (check.Count == 0)
                    {
                        _context.Add(ex);
                        _context.SaveChanges();
                    }
                }
            }
        }
    }
}
