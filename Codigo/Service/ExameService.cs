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
            throw new NotImplementedException();
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
        {
            throw new NotImplementedException();
        }

        public bool Insert(ExameModel exameModel)
        {
            _context.Add(ModelToEntity(exameModel, new Exame()));
            return _context.SaveChanges() == 1 ? true : false;
        }

        private Exame ModelToEntity(ExameModel exameModel, Exame exame)
        {
            exame.IdAgenteSaude = exameModel.IdAgenteSaude;
            exame.IdPaciente = exameModel.IdPaciente;
            exame.IdVirusBacteria = exameModel.IdVirusBacteria;
            exame.IgG = exameModel.IgG;
            exame.IgM = exameModel.IgM;
            exame.Pcr = exameModel.Pcr;
            exame.EstadoRealizacao  = exameModel.EstadoRealizacao;
            exame.MunicipioId       = exameModel.MunicipioId;
            exame.DataInicioSintomas = exameModel.DataInicioSintomas;
            exame.DataExame = exameModel.DataExame;

            return exame;
        }

        public bool Update(ExameModel exameModel)
        {
            throw new NotImplementedException();
        }

    
    }
}
