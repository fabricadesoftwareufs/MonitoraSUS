using Model;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class EmpresaExameService : IEmpresaExameService
    {
        private readonly monitorasusContext _context;

        public EmpresaExameService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Delete(int id)
        {
            var empresa = _context.Empresaexame.Find(id);
            _context.Empresaexame.Remove(empresa);
            return _context.SaveChanges() == 1 ? true : false;
        }

        public bool Insert(EmpresaExameModel empresaExameModel)
        {
            _context.Add(ModelToEntity(empresaExameModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

        public bool Update(EmpresaExameModel empresaExameModel)
        {
            _context.Update(ModelToEntity(empresaExameModel));
            return _context.SaveChanges() == 1 ? true : false;
        }


        private Empresaexame ModelToEntity(EmpresaExameModel empresa)
        {
            return new Empresaexame
            {
                Id = empresa.Id,
                Cnpj = empresa.Cnpj,
                Nome = empresa.Nome,
                Cep = empresa.Cep,
                Rua = empresa.Rua,
                Bairro = empresa.Bairro,
                Cidade = empresa.Cidade,
                Estado = empresa.Estado,
                Numero = empresa.Numero,
                Complemento = empresa.Complemento,
                Latitude = empresa.Latitude,
                Longitude = empresa.Longitude,
                FoneCelular = empresa.FoneCelular,
                FoneFixo = empresa.FoneFixo,
                Email = empresa.Email
            };
        }

     
        public List<EmpresaExameModel> GetAll()
        => _context.Empresaexame
            .Select(empresa => new EmpresaExameModel
            {
                Id = empresa.Id,
                Cnpj = empresa.Cnpj,
                Nome = empresa.Nome,
                Cep = empresa.Cep,
                Rua = empresa.Rua,
                Bairro = empresa.Bairro,
                Cidade = empresa.Cidade,
                Estado = empresa.Estado,
                Numero = empresa.Numero,
                Complemento = empresa.Complemento,
                Latitude = empresa.Latitude,
                Longitude = empresa.Longitude,
                FoneCelular = empresa.FoneCelular,
                FoneFixo = empresa.FoneFixo,
                Email = empresa.Email
            }).ToList();

        public EmpresaExameModel GetById(int id)
         => _context.Empresaexame
            .Where(empresaExame => empresaExame.Id == id)
            .Select(empresa => new EmpresaExameModel
            {
                Id = empresa.Id,
                Cnpj = empresa.Cnpj,
                Nome = empresa.Nome,
                Cep = empresa.Cep,
                Rua = empresa.Rua,
                Bairro = empresa.Bairro,
                Cidade = empresa.Cidade,
                Estado = empresa.Estado,
                Numero = empresa.Numero,
                Complemento = empresa.Complemento,
                Latitude = empresa.Latitude,
                Longitude = empresa.Longitude,
                FoneCelular = empresa.FoneCelular,
                FoneFixo = empresa.FoneFixo,
                Email = empresa.Email
            }).FirstOrDefault();

    }
}
