using Model;
using Persistence;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class VirusBacteriaService : IVirusBacteriaService
    {
        private readonly monitorasusContext _context;
        public VirusBacteriaService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Delete(int id)
        {
            var virus = _context.Virusbacteria.Find(id);
            _context.Virusbacteria.Remove(virus);
            return _context.SaveChanges() == 1 ? true : false;
        }

        public List<VirusBacteriaModel> GetAll()
             => _context.Virusbacteria
                .Select(virusbacteria => new VirusBacteriaModel
                {
                    IdVirusBacteria = virusbacteria.IdVirusBacteria,
                    Nome = virusbacteria.Nome,
                    DiasRecuperacao = virusbacteria.DiasRecuperacao
                }).ToList();


        public VirusBacteriaModel GetById(int id)
        => _context.Virusbacteria
                .Where(virusBacteriaModel => virusBacteriaModel.IdVirusBacteria == id)
                .Select(virusbacteria => new VirusBacteriaModel
                {
                    IdVirusBacteria = virusbacteria.IdVirusBacteria,
                    Nome = virusbacteria.Nome,
                    DiasRecuperacao = virusbacteria.DiasRecuperacao
                }).FirstOrDefault();

        public bool Insert(VirusBacteriaModel virusBacteriaModel)
        {
            _context.Add(ModelToEntity(virusBacteriaModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

        private Virusbacteria ModelToEntity(VirusBacteriaModel virusModel)
        {
            return new Virusbacteria
            {
                IdVirusBacteria = virusModel.IdVirusBacteria,
                Nome = virusModel.Nome,
                DiasRecuperacao = virusModel.DiasRecuperacao
            };
        }

        public bool Update(VirusBacteriaModel virusBacteriaModel)
        {
            _context.Update(ModelToEntity(virusBacteriaModel));
            return _context.SaveChanges() == 1 ? true : false;
        }
    }
}
