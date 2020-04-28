using Model;
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
               }).ToList();

<<<<<<< HEAD
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
                }).ToList();

=======
                
>>>>>>> 9c8df4baf41ba7459f1a3cd8fed596a17c95511f
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
    }
}
