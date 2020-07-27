using Model;
using Persistence;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;

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
            var estado = _context.Estado.Find(id);
            _context.Estado.Remove(estado);
            return _context.SaveChanges() == 1 ? true : false;
        }

        public EstadoModel GetById(int id)
         => _context.Estado
             .Where(estadoModel => estadoModel.Id == id)
             .Select(e => new EstadoModel
             {
                 Id = e.Id,
                 CodigoUf = e.CodigoUf,
                 Nome = e.Nome,
                 Regiao = e.Regiao,
                 Uf = e.Uf
             }).FirstOrDefault();

        public bool Insert(EstadoModel estadoModel)
        {
            _context.Add(ModelToEntity(estadoModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

        public bool Update(EstadoModel estadoModel)
        {
            _context.Update(ModelToEntity(estadoModel));
            return _context.SaveChanges() == 1 ? true : false;
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

        public Estado ModelToEntity(EstadoModel estado)
        {

            return new Estado
            {
                Id = estado.Id,
                Nome = estado.Nome,
                CodigoUf = estado.CodigoUf,
                Regiao = estado.Regiao,
                Uf = estado.Uf
            };
        }

        public EstadoModel GetByUf(string uf)
         => _context.Estado
             .Where(estadoModel => estadoModel.Uf.ToUpper().Equals(uf.ToUpper()))
             .Select(e => new EstadoModel
             {
                 Id = e.Id,
                 CodigoUf = e.CodigoUf,
                 Nome = e.Nome,
                 Regiao = e.Regiao,
                 Uf = e.Uf
             }).FirstOrDefault();

        public EstadoModel GetByCodUf(int codigoUf)
         => _context.Estado
             .Where(estadoModel => estadoModel.CodigoUf.Equals(codigoUf))
             .Select(e => new EstadoModel
             {
                 Id = e.Id,
                 CodigoUf = e.CodigoUf,
                 Nome = e.Nome,
                 Regiao = e.Regiao,
                 Uf = e.Uf
             }).FirstOrDefault();
    }
}

