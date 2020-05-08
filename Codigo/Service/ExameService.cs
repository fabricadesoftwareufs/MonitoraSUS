﻿using Model;
using Model.ViewModel;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class ExameService : IExameService
    {
        private readonly monitorasusContext _context;

        public ExameService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Insert(ExameModel exameModel)
        {
            _context.Add(ModelToEntity(exameModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

        public bool Delete(int id)
        {
            var exame = _context.Exame.Find(id);
            _context.Exame.Remove(exame);
            return _context.SaveChanges() == 1 ? true : false;
        }

        public bool Update(ExameModel exameModel)
        {
            _context.Update(ModelToEntity(exameModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

        public List<ExameModel> GetByIdAgente(int idAgente)
         => _context.Exame
                .Where(exameModel => exameModel.IdAgenteSaude == idAgente)
                .Select(exame => new ExameModel
                {
                    IdVirusBacteria = exame.IdVirusBacteria,
                    IdExame = exame.IdExame,
                    IdPaciente = exame.IdPaciente,
                    IdAgenteSaude = exame.IdAgenteSaude,
                    DataExame = exame.DataExame,
                    DataInicioSintomas = exame.DataInicioSintomas,
                    IgG = exame.IgG,
                    IgM = exame.IgM,
                    Pcr = exame.Pcr,
                    IdEstado = exame.IdEstado,
                    IdMunicipio = exame.IdMunicipio,
                    IdEmpresaSaude = exame.IdEmpresaSaude,
                    FoiNotificado = Convert.ToBoolean(exame.FoiNotificado),
                    DataNotificacao = exame.DataNotificacao,
                    EhProfissionalSaude = Convert.ToBoolean(exame.EhProfissionalSaude),
                    CodigoColeta = exame.CodigoColeta

                }).ToList();

        public List<ExameModel> GetAll()
             => _context.Exame
                .Select(exame => new ExameModel
                {
                    IdVirusBacteria = exame.IdVirusBacteria,
                    IdExame = exame.IdExame,
                    IdPaciente = exame.IdPaciente,
                    IdAgenteSaude = exame.IdAgenteSaude,
                    DataExame = exame.DataExame,
                    DataInicioSintomas = exame.DataInicioSintomas,
                    IgG = exame.IgG,
                    IgM = exame.IgM,
                    Pcr = exame.Pcr,
                    IdEstado = exame.IdEstado,
                    IdMunicipio = exame.IdMunicipio,
                    IdEmpresaSaude = exame.IdEmpresaSaude,
                    FoiNotificado = Convert.ToBoolean(exame.FoiNotificado),
                    DataNotificacao = exame.DataNotificacao,
                    EhProfissionalSaude = Convert.ToBoolean(exame.EhProfissionalSaude),
                    CodigoColeta = exame.CodigoColeta
                }).ToList();


        public ExameModel GetById(int id)
          => _context.Exame
                .Where(exameModel => exameModel.IdExame == id)
                .Select(exame => new ExameModel
                {
                    IdVirusBacteria = exame.IdVirusBacteria,
                    IdExame = exame.IdExame,
                    IdPaciente = exame.IdPaciente,
                    IdAgenteSaude = exame.IdAgenteSaude,
                    DataExame = exame.DataExame,
                    DataInicioSintomas = exame.DataInicioSintomas,
                    IgG = exame.IgG,
                    IgM = exame.IgM,
                    Pcr = exame.Pcr,
                    IdEstado = exame.IdEstado,
                    IdMunicipio = exame.IdMunicipio,
                    IdEmpresaSaude = exame.IdEmpresaSaude,
                    FoiNotificado = Convert.ToBoolean(exame.FoiNotificado),
                    DataNotificacao = exame.DataNotificacao,
                    EhProfissionalSaude = Convert.ToBoolean(exame.EhProfissionalSaude),
                    CodigoColeta = exame.CodigoColeta
                }).FirstOrDefault();

        private Exame ModelToEntity(ExameModel exameModel)
        {
            return new Exame
            {
                IdExame = exameModel.IdExame,
                IdAgenteSaude = exameModel.IdAgenteSaude,
                IdPaciente = exameModel.IdPaciente,
                IdVirusBacteria = exameModel.IdVirusBacteria,
                IgG = exameModel.IgG,
                IgM = exameModel.IgM,
                Pcr = exameModel.Pcr,
                IdEstado = exameModel.IdEstado,
                IdMunicipio = exameModel.IdMunicipio,
                DataInicioSintomas = exameModel.DataInicioSintomas,
                DataExame = exameModel.DataExame,
                IdEmpresaSaude = exameModel.IdEmpresaSaude,
                FoiNotificado = Convert.ToByte(exameModel.FoiNotificado),
                DataNotificacao = exameModel.DataNotificacao,
                EhProfissionalSaude = Convert.ToByte(exameModel.EhProfissionalSaude),
                CodigoColeta = exameModel.CodigoColeta
            };
        }

        public List<ExameModel> GetByIdEstado(int idEstado)
         => _context.Exame
                .Where(exameModel => exameModel.IdEstado == idEstado)
                .Select(exame => new ExameModel
                {
                    IdVirusBacteria = exame.IdVirusBacteria,
                    IdExame = exame.IdExame,
                    IdPaciente = exame.IdPaciente,
                    IdAgenteSaude = exame.IdAgenteSaude,
                    DataExame = exame.DataExame,
                    DataInicioSintomas = exame.DataInicioSintomas,
                    IgG = exame.IgG,
                    IgM = exame.IgM,
                    Pcr = exame.Pcr,
                    IdEstado = exame.IdEstado,
                    IdMunicipio = exame.IdMunicipio,
                    IdEmpresaSaude = exame.IdEmpresaSaude,
                    FoiNotificado = Convert.ToBoolean(exame.FoiNotificado),
                    DataNotificacao = exame.DataNotificacao,
                    EhProfissionalSaude = Convert.ToBoolean(exame.EhProfissionalSaude),
                    CodigoColeta = exame.CodigoColeta
                }).ToList();

        public List<ExameModel> GetByIdEmpresa(int idEempresa)
        => _context.Exame
               .Where(exameModel => exameModel.IdEmpresaSaude == idEempresa)
               .Select(exame => new ExameModel
               {
                   IdVirusBacteria = exame.IdVirusBacteria,
                   IdExame = exame.IdExame,
                   IdPaciente = exame.IdPaciente,
                   IdAgenteSaude = exame.IdAgenteSaude,
                   DataExame = exame.DataExame,
                   DataInicioSintomas = exame.DataInicioSintomas,
                   IgG = exame.IgG,
                   IgM = exame.IgM,
                   Pcr = exame.Pcr,
                   IdEstado = exame.IdEstado,
                   IdMunicipio = exame.IdMunicipio,
                   IdEmpresaSaude = exame.IdEmpresaSaude,
                   FoiNotificado = Convert.ToBoolean(exame.FoiNotificado),
                   DataNotificacao = exame.DataNotificacao,
                   EhProfissionalSaude = Convert.ToBoolean(exame.EhProfissionalSaude),
                   CodigoColeta = exame.CodigoColeta
               }).ToList();

        public List<ExameModel> GetByIdPaciente(int idPaciente)
         => _context.Exame
                .Where(exameModel => exameModel.IdPaciente == idPaciente)
                .Select(exame => new ExameModel
                {
                    IdVirusBacteria = exame.IdVirusBacteria,
                    IdExame = exame.IdExame,
                    IdPaciente = exame.IdPaciente,
                    IdAgenteSaude = exame.IdAgenteSaude,
                    DataExame = exame.DataExame,
                    DataInicioSintomas = exame.DataInicioSintomas,
                    IgG = exame.IgG,
                    IgM = exame.IgM,
                    Pcr = exame.Pcr,
                    IdEstado = exame.IdEstado,
                    IdMunicipio = exame.IdMunicipio,
                    IdEmpresaSaude = exame.IdEmpresaSaude,
                    FoiNotificado = Convert.ToBoolean(exame.FoiNotificado),
                    DataNotificacao = exame.DataNotificacao,
                    EhProfissionalSaude = Convert.ToBoolean(exame.EhProfissionalSaude),
                    CodigoColeta = exame.CodigoColeta
                }).ToList();

        public List<ExameModel> GetByIdMunicipio(int idMunicicpio)
         => _context.Exame
                .Where(exameModel => exameModel.IdMunicipio == idMunicicpio)
                .Select(exame => new ExameModel
                {
                    IdVirusBacteria = exame.IdVirusBacteria,
                    IdExame = exame.IdExame,
                    IdPaciente = exame.IdPaciente,
                    IdAgenteSaude = exame.IdAgenteSaude,
                    DataExame = exame.DataExame,
                    DataInicioSintomas = exame.DataInicioSintomas,
                    IgG = exame.IgG,
                    IgM = exame.IgM,
                    Pcr = exame.Pcr,
                    IdEstado = exame.IdEstado,
                    IdMunicipio = exame.IdMunicipio,
                    IdEmpresaSaude = exame.IdEmpresaSaude,
                    FoiNotificado = Convert.ToBoolean(exame.FoiNotificado),
                    DataNotificacao = exame.DataNotificacao,
                    EhProfissionalSaude = Convert.ToBoolean(exame.EhProfissionalSaude),
                    CodigoColeta = exame.CodigoColeta
                }).ToList();

        public List<ExameModel> CheckDuplicateExamToday(int idPaciente, int idVirusBacteria, DateTime dateExame)
        {
            var exames = _context.Exame.Where(exameModel => exameModel.IdVirusBacteria == idVirusBacteria &&
                         exameModel.IdPaciente == idPaciente && dateExame.ToString("dd/MM/yyyy").Equals(exameModel.DataExame.ToString("dd/MM/yyyy")))
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
                     IdVirusBacteria = exame.IdVirusBacteria,
                     IdExame = exame.IdExame,
                     IdPaciente = exame.IdPaciente,
                     IdAgenteSaude = exame.IdAgenteSaude,
                     DataExame = exame.DataExame,
                     DataInicioSintomas = exame.DataInicioSintomas,
                     IgG = exame.IgG,
                     IgM = exame.IgM,
                     Pcr = exame.Pcr,
                     IdEstado = exame.IdEstado,
                     IdMunicipio = exame.IdMunicipio,
                     IdEmpresaSaude = exame.IdEmpresaSaude,
                     UF = exame.IdEstadoNavigation.Uf,
                     Municipio = exame.IdMunicipioNavigation.Nome,
                     Bairro = ""
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


        public List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByEstado(int idEstado)
            => ConvertToEstadoMunicipioBairro(
            _context.Exame
                 .Where(exameModel => exameModel.IdEstado == idEstado && exameModel.IdEmpresaSaude == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
                 .Select(exame => new ExameCompletoModel
                 {
                     IdVirusBacteria = exame.IdVirusBacteria,
                     IdExame = exame.IdExame,
                     IdPaciente = exame.IdPaciente,
                     IdAgenteSaude = exame.IdAgenteSaude,
                     DataExame = exame.DataExame,
                     DataInicioSintomas = exame.DataInicioSintomas,
                     IgG = exame.IgG,
                     IgM = exame.IgM,
                     Pcr = exame.Pcr,
                     IdEstado = exame.IdEstado,
                     IdMunicipio = exame.IdMunicipio,
                     IdEmpresaSaude = exame.IdEmpresaSaude,
                     UF = exame.IdEstadoNavigation.Uf,
                     Municipio = exame.IdMunicipioNavigation.Nome,
                     Bairro = ""
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
                     IdVirusBacteria = exame.IdVirusBacteria,
                     IdExame = exame.IdExame,
                     IdPaciente = exame.IdPaciente,
                     IdAgenteSaude = exame.IdAgenteSaude,
                     DataExame = exame.DataExame,
                     DataInicioSintomas = exame.DataInicioSintomas,
                     IgG = exame.IgG,
                     IgM = exame.IgM,
                     Pcr = exame.Pcr,
                     IdEstado = exame.IdEstado,
                     IdMunicipio = exame.IdMunicipio,
                     IdEmpresaSaude = exame.IdEmpresaSaude,
                     UF = exame.IdEstadoNavigation.Uf,
                     Municipio = exame.IdMunicipioNavigation.Nome,
                     Bairro = ""
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
                     IdVirusBacteria = exame.IdVirusBacteria,
                     IdExame = exame.IdExame,
                     IdPaciente = exame.IdPaciente,
                     IdAgenteSaude = exame.IdAgenteSaude,
                     DataExame = exame.DataExame,
                     DataInicioSintomas = exame.DataInicioSintomas,
                     IgG = exame.IgG,
                     IgM = exame.IgM,
                     Pcr = exame.Pcr,
                     IdEstado = exame.IdEstado,
                     IdMunicipio = exame.IdMunicipio,
                     IdEmpresaSaude = exame.IdEmpresaSaude,
                     UF = exame.IdEstadoNavigation.Uf,
                     Municipio = exame.IdMunicipioNavigation.Nome,
                     Bairro = ""
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
                    if (totalEMB.Municipio == null)
                    {
                        if (totalEMB.Estado.Equals(totalPorResultado.Estado) && (totalPorResultado.Municipio == null) &&
                            (totalEMB.IdEmpresaSaude == totalPorResultado.IdEmpresaSaude) && totalEMB.Bairro.Equals(totalPorResultado.Bairro))
                        {
                            AtualizarTotais(totalPorResultado, totalEMB);
                            achou = true;
                            break;
                        }
                    }
                    else
                    {
                        if (totalPorResultado.Municipio != null)
                        {
                            if (totalEMB.Estado.Equals(totalPorResultado.Estado) && totalEMB.Municipio.Equals(totalPorResultado.Municipio) &&
                                (totalEMB.IdEmpresaSaude == totalPorResultado.IdEmpresaSaude) && totalEMB.Bairro.Equals(totalPorResultado.Bairro))
                            {
                                AtualizarTotais(totalPorResultado, totalEMB);
                                achou = true;
                                break;
                            }
                        }
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
                        TotalImunizados = 0,
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
            if (totalPorResultado.Resultado.Equals(ExameModel.RESULTADO_IMUNIZADO))
                totalEMB.TotalImunizados += totalPorResultado.Total;
            if (totalPorResultado.Resultado.Equals(ExameModel.RESULTADO_INDETERMINADO))
                totalEMB.TotalIndeterminados += totalPorResultado.Total;
        }
    }
}
