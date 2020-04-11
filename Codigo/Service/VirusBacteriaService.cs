using Model;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            throw new NotImplementedException();
        }

        public List<VirusBacteriaModel> GetAll()
             => _context.Virusbacteria
                .Select(virusbacteria => new VirusBacteriaModel
                {
                    IdVirusBacteria = virusbacteria.IdVirusBacteria,
                    Nome = virusbacteria.Nome
                }).ToList();


        public VirusBacteriaModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public bool Insert(VirusBacteriaModel virusBacteriaModel)
        {
            throw new NotImplementedException();
        }

        public bool Update(VirusBacteriaModel virusBacteriaModel)
        {
            throw new NotImplementedException();
        }
    }
}
