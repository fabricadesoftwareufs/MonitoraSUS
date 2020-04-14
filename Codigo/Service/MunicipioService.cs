using Model;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class MunicipioService : IMunicipioService
    {
        private readonly monitorasusContext _context;
        public MunicipioService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<MunicipioModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public MunicipioModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public List<MunicipioModel> GetByUFCode(string UFCode)
            => _context
                .Municipio
                .Where(m => m.Uf == UFCode)
                .Select(m => new MunicipioModel
                {
                    Id = m.Id,
                    Uf = m.Uf,
                    Codigo = m.Codigo,
                    Nome = m.Nome
                }).ToList();

        public bool Insert(MunicipioModel municipioModel)
        {
            throw new NotImplementedException();
        }

        public bool Update(MunicipioModel municipioModel)
        {
            throw new NotImplementedException();
        }
    }
}
