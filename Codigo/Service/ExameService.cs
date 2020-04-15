using Model;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class ExameService : IExameService
    {
        private readonly monitorasusContext _context;

        public ExameService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Delete(int id)
        {
            var exame = _context.Exame.Find(id);
            _context.Exame.Remove(exame);
            return _context.SaveChanges() == 1 ? true : false;
        }

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
                    EstadoRealizacao = exame.EstadoRealizacao,
                    MunicipioId = exame.MunicipioId,
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
                    EstadoRealizacao = exame.EstadoRealizacao,
                    MunicipioId = exame.MunicipioId,
                }).FirstOrDefault();

        public bool Insert(ExameModel exameModel)
        {
            _context.Add(ModelToEntity(exameModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

        private Exame ModelToEntity(ExameModel exameModel)
        {
            Exame exame = new Exame
            {
                IdExame = exameModel.IdExame,
                IdAgenteSaude = exameModel.IdAgenteSaude,
                IdPaciente = exameModel.IdPaciente,
                IdVirusBacteria = exameModel.IdVirusBacteria,
                IgG = exameModel.IgG,
                IgM = exameModel.IgM,
                Pcr = exameModel.Pcr,
                EstadoRealizacao = exameModel.EstadoRealizacao,
                MunicipioId = exameModel.MunicipioId,
                DataInicioSintomas = exameModel.DataInicioSintomas,
                DataExame = exameModel.DataExame
            };

            return exame;
        }

        public bool Update(ExameModel exameModel)
        {
            _context.Update(ModelToEntity(exameModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

    
    }
}
