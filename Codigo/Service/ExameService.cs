using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.AuxModel;
using Model.ViewModel;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Util;

namespace Service
{
    public class ExameService : IExameService
    {
        private readonly monitorasusContext _context;

        public ExameService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Insert(ExameViewModel exameModel)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    IPessoaService _pessoaService = new PessoaService(_context);
                    ISituacaoVirusBacteriaService _situacaoPessoaService = new SituacaoVirusBacteriaService(_context);
                    CreatePessoaModelByExame(exameModel, _pessoaService);
                    if (GetExamesRelizadosData(exameModel.Paciente.Idpessoa, exameModel.Exame.IdVirusBacteria, exameModel.Exame.DataExame, exameModel.Exame.MetodoExame).Count > 0)
                        return false;
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
                   
                    var exameEntity = ModelToEntity(exameModel);
                    _context.Add(exameEntity);
                    _context.SaveChanges();
                    transaction.Commit();
                    _context.Entry(exameEntity).State = EntityState.Detached;
                    return true;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
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
                        var exameBusca = examesRealizados.FirstOrDefault();
                        if (exameBusca.IdExame != exameModel.Exame.IdExame)
                            throw new ServiceException("Notificação DUPLICADA! Já existe um exame registrado desse paciente para esse Vírus/Bactéria na " +
                                                            "data informada. Por favor, verifique se os dados da notificação estão corretos.");
                    }

                    var situacao = _situacaoPessoaService.GetById(exameModel.Paciente.Idpessoa, exameModel.Exame.IdVirusBacteria);
                    if (situacao == null)
                        _situacaoPessoaService.Insert(CreateSituacaoPessoaModelByExame(exameModel, situacao, _pessoaService));
                    else
                        _situacaoPessoaService.Update(CreateSituacaoPessoaModelByExame(exameModel, situacao, _pessoaService));

                    _pessoaService.Update(CreatePessoaModelByExame(exameModel, _pessoaService), false);

                    var exameEntity = ModelToEntity(exameModel);
                    _context.Update(exameEntity);
                    _context.SaveChanges();
                    transaction.Commit();
                    _context.Entry(exameEntity).State = EntityState.Detached;

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
            exameModel.Exame.IdAgenteSaude = exameModel.Usuario.IdPessoa;
            exameModel.Exame.CodigoColeta = (exameModel.Exame.CodigoColeta == null) ? "" : exameModel.Exame.CodigoColeta;
            exameModel.Exame.IdNotificacao = (exameModel.Exame.IdNotificacao == null) ? "" : exameModel.Exame.IdNotificacao;
            if (exameModel.Exame.IdEstado == 0)
            {
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
                .Where(exameModel => exameModel.IdAgenteSaude == idAgente).OrderByDescending(e => e.DataNotificacao).Take(lastRecord)
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
              .Where(exameModel => exameModel.IdEmpresaSaude == idEempresa).OrderByDescending(e => e.DataNotificacao).Take(lastRecord)
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
                .Where(exameModel => exameModel.IdMunicipio == idMunicicpio).OrderByDescending(e => e.DataNotificacao).Take(lastRecord)
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
               && (exameModel.IdMunicipio == null)).OrderByDescending(e => e.DataNotificacao).Take(lastRecord)
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

        private PessoaModel CreatePessoaModelByExame(ExameViewModel exameViewModel, IPessoaService pessoaService)
        {
            PessoaModel pessoaModel = null;
            if (exameViewModel.Paciente.Idpessoa > 0)
                pessoaModel = pessoaService.GetById(exameViewModel.Paciente.Idpessoa);
            if (pessoaModel == null && !String.IsNullOrEmpty(exameViewModel.Paciente.Cpf))
                pessoaModel = pessoaService.GetByCpf(exameViewModel.Paciente.Cpf.ToUpper());
            if (pessoaModel == null && !String.IsNullOrEmpty(exameViewModel.Paciente.Cns))
                pessoaModel = pessoaService.GetByCns(exameViewModel.Paciente.Cns.ToUpper());
            if (pessoaModel == null)
            {
                exameViewModel.Paciente.Idpessoa = 0;
                exameViewModel.Paciente.IdAreaAtuacao = exameViewModel.Exame.IdAreaAtuacao;
                if (exameViewModel.Exame.AguardandoResultado == true || exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_POSITIVO) ||
                    exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_IGMIGG))
                    exameViewModel.Paciente.SituacaoSaude = PessoaModel.SITUACAO_ISOLAMENTO;
            }
            else
            {
                exameViewModel.Paciente.Idpessoa = pessoaModel.Idpessoa;
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
                    var exames = GetByIdPaciente(pessoaModel.Idpessoa).Where(e => e.Exame.DataExame >= dataMinima).ToList();
                    if (exames.Count() <= 1 && exameViewModel.Paciente.SituacaoSaude.Equals(PessoaModel.SITUACAO_ISOLAMENTO))
                    {
                        exameViewModel.Paciente.SituacaoSaude = PessoaModel.SITUACAO_SAUDAVEL;
                    }
                }
            }
            return exameViewModel.Paciente;
        }

        private SituacaoPessoaVirusBacteriaModel CreateSituacaoPessoaModelByExame(ExameViewModel exame, SituacaoPessoaVirusBacteriaModel situacao, IPessoaService pessoaService)
        {

            if (situacao != null)
            {
                situacao.UltimaSituacaoSaude = exame.Exame.ResultadoStatus;
            }
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

        public async void CorrigeLocalizacao(PessoaModel pessoa, string googleKey)
        {
            MunicipioGeoService geoService = new MunicipioGeoService(_context);
            MunicipioService municipioService = new MunicipioService(_context);

            var municipio = municipioService.GetByName(pessoa.Cidade);
            var geoLocation = geoService.GetByIBGECode(municipio.Codigo);
            double latPadrao = geoLocation.Latitude;
            double longPadrao = geoLocation.Longitude;

            // sequencia recomendada:  Number, Street Direction, Street Name, Street Suffix, City, State, Zip, Country

            string address = "";
            if (!pessoa.Numero.Equals(""))
                address += pessoa.Numero + "+";
            if (!pessoa.Bairro.Equals(""))
                address += pessoa.Bairro + "+";
            if (!pessoa.Rua.Equals(""))
                address += pessoa.Rua + "+";

            address += pessoa.Cidade + "+" + pessoa.Estado;
            if (!pessoa.Cep.Equals(""))
                address += "+" + pessoa.Cep;

            (string lat, string lng) = await Methods.BuscaLatLong(address, googleKey);
            double latitude = Convert.ToDouble(lat);
            double longitude = Convert.ToDouble(lng);

            if (Math.Abs(Convert.ToDouble(pessoa.Latitude) - latPadrao) > 10 || Math.Abs(Convert.ToDouble(pessoa.Longitude) - longPadrao) > 10 || latitude == 0 || longitude == 0)
            {
                pessoa.Latitude = Convert.ToString(latPadrao);
                pessoa.Longitude = Convert.ToString(longPadrao);
            }
            else
            {
                pessoa.Latitude = lat;
                pessoa.Longitude = lng;
            }
        }

    }
}
