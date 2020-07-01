using Model;
using Persistence;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;

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
            var municipio = _context.Municipio.Find(id);
            _context.Municipio.Remove(municipio);
            return _context.SaveChanges() == 1 ? true : false;
        }

        public List<MunicipioModel> GetAll()
          => _context.Municipio
                .Select(municipio => new MunicipioModel
                {
                    Id = municipio.Id,
                    Nome = municipio.Nome,
                    Uf = municipio.Uf,
                    Codigo = municipio.Codigo
                }).ToList();

        public MunicipioModel GetById(int id)
         => _context.Municipio
                .Where(municipioModel => municipioModel.Id == id)
                .Select(municipio => new MunicipioModel
                {
                    Id = municipio.Id,
                    Nome = municipio.Nome,
                    Uf = municipio.Uf,
                    Codigo = municipio.Codigo
                }).FirstOrDefault();

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
            _context.Add(ModelToEntity(municipioModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

        public bool Update(MunicipioModel municipioModel)
        {
            _context.Update(ModelToEntity(municipioModel));
            return _context.SaveChanges() == 1 ? true : false;
        }
        public Municipio ModelToEntity(MunicipioModel municipio)
        {

            return new Municipio
            {
                Id = municipio.Id,
                Nome = municipio.Nome,
                Uf = municipio.Uf,
                Codigo = municipio.Codigo,
            };
        }

        public MunicipioModel GetByIBGECode(int code)
          => _context.Municipio
                .Where(municipioModel => municipioModel.Codigo == code)
                .Select(municipio => new MunicipioModel
                {
                    Id = municipio.Id,
                    Nome = municipio.Nome,
                    Uf = municipio.Uf,
                    Codigo = municipio.Codigo
                }).FirstOrDefault();
    }
}
