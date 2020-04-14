using Model;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Interface
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

        public MunicipioModel GetByName(string name)
         => _context.Municipio
                .Where(municipioModel => municipioModel.Nome.ToUpper().Equals(name.ToUpper()))
                .Select(municipio => new MunicipioModel
                {
                    Id = municipio.Id,
                    Nome = municipio.Nome,
                    Uf = municipio.Uf,
                    Codigo = municipio.Codigo
                }).FirstOrDefault();

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
