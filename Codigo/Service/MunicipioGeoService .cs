using Google.Protobuf.WellKnownTypes;
using Model;
using Persistence;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Util;

namespace Service
{
    public class MunicipioGeoService : IMunicipioGeoService
    {
        private readonly monitorasusContext _context;

        public MunicipioGeoService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Delete(int id)
        {
            var municipio = _context.Municipio.Find(id);
            _context.Municipio.Remove(municipio);
            return _context.SaveChanges() == 1 ? true : false;
        }

        public List<MunicipioGeoModel> GetAll()
          => _context.Municipiosgeo
                .Select(municipio => new MunicipioGeoModel
                {
                       CodigoIbge = municipio.CodigoIbge,
                        Nome = municipio.Nome,
                        Latitude = municipio.Latitude,
                        Longitude = municipio.Longitude,
                        Capital = municipio.Capital,
                        CodigoUf = municipio.CodigoUf
                }).ToList();

        public MunicipioGeoModel GetById(int id)
         => _context.Municipiosgeo
                .Where(municipioModel => municipioModel.CodigoIbge == id)
                .Select(municipio => new MunicipioGeoModel
                {
                    CodigoIbge = municipio.CodigoIbge,
                    Nome = municipio.Nome,
                    Latitude = municipio.Latitude,
                    Longitude = municipio.Longitude,
                    Capital = municipio.Capital,
                    CodigoUf = municipio.CodigoUf
                }).FirstOrDefault();

        public MunicipioGeoModel GetByName(string name, int codUf)
         => _context.Municipiosgeo
                .Where(municipioModel => Methods.RemoverAcentos(municipioModel.Nome).ToUpper().Equals(Methods.RemoverAcentos(name).ToUpper())
                 && municipioModel.CodigoUf == codUf)
                .Select(municipio => new MunicipioGeoModel
                {
                    CodigoIbge = municipio.CodigoIbge,
                    Nome = municipio.Nome,
                    Latitude = municipio.Latitude,
                    Longitude = municipio.Longitude,
                    Capital = municipio.Capital,
                    CodigoUf = municipio.CodigoUf
                }).FirstOrDefault();

        public List<MunicipioGeoModel> GetByUFCode(int codigoUf)
            => _context
                .Municipiosgeo
                .Where(municipio => municipio.CodigoUf == codigoUf)
                .Select(municipio => new MunicipioGeoModel
                {
                    CodigoIbge = municipio.CodigoIbge,
                    Nome = municipio.Nome,
                    Latitude = municipio.Latitude,
                    Longitude = municipio.Longitude,
                    Capital = municipio.Capital,
                    CodigoUf = municipio.CodigoUf
                }).ToList();

        public bool Insert(MunicipioGeoModel municipioModel)
        {
            _context.Add(ModelToEntity(municipioModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

        public bool Update(MunicipioGeoModel municipioModel)
        {
            _context.Update(ModelToEntity(municipioModel));
            return _context.SaveChanges() == 1 ? true : false;
        }
        public Municipiosgeo ModelToEntity(MunicipioGeoModel municipio)
        {

            return new Municipiosgeo
            {
                CodigoIbge = municipio.CodigoIbge,
                Nome = municipio.Nome,
                Latitude = municipio.Latitude,
                Longitude = municipio.Longitude,
                Capital = municipio.Capital,
                CodigoUf = municipio.CodigoUf
            };
        }

        public MunicipioGeoModel GetByIBGECode(int codigo)
          => _context.Municipiosgeo
                .Where(municipioModel => municipioModel.CodigoIbge == codigo)
                .Select(municipio => new MunicipioGeoModel
                {
                    CodigoIbge = municipio.CodigoIbge,
                    Nome = municipio.Nome,
                    Latitude = municipio.Latitude,
                    Longitude = municipio.Longitude,
                    Capital = municipio.Capital,
                    CodigoUf = municipio.CodigoUf
                }).FirstOrDefault();
    }
}
