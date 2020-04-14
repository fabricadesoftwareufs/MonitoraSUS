using Model;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Interface
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
        {
            throw new NotImplementedException();
        }

        public EstadoModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public EstadoModel GetByName(string name)
              => _context.Estado
                .Where(estadoModel => estadoModel.Nome.ToUpper().Equals(name.ToUpper()))
                .Select(estado => new EstadoModel
                {
                    Id = estado.Id,
                    CodigoUf = estado.CodigoUf,
                    Nome = estado.Nome,
                    Uf = estado.Uf,
                    Regiao = estado.Regiao

                }).FirstOrDefault();


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
