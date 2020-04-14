using Model;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class EstadoService : IEstadoService
    {
        private readonly monitorasusContext _context;
        public EstadoService(monitorasusContext context)
        {
            _context = context;
        }
        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<EstadoModel> GetAll()
            => _context
                .Estado
                .Select(e => new EstadoModel
                {
                    Id = e.Id,
                    CodigoUf = e.CodigoUf,
                    Nome = e.Nome,
                    Regiao = e.Regiao,
                    Uf = e.Uf
                }).ToList();

        public EstadoModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public bool Insert(EstadoModel estadoModel)
        {
            throw new NotImplementedException();
        }

        public bool Update(EstadoModel estadoModel)
        {
            throw new NotImplementedException();
        }
    }
}
