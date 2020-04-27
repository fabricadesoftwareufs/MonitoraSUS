using Model;
using Persistence;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;

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
                Email = empresa.Email,
                // A partir daqui
                EmiteLaudoExame = empresa.EmiteLaudoExame,
                NumeroLeitos = empresa.NumeroLeitos,
                NumeroLeitosDisponivel = empresa.NumeroLeitosDisponivel,
                NumeroLeitosUti = empresa.NumeroLeitosUti,
                NumeroLeitosUtidisponivel = empresa.NumeroLeitosUtidisponivel,
                PossuiLeitosInternacao = empresa.PossuiLeitosInternacao
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
                Email = empresa.Email,
                EmiteLaudoExame = empresa.EmiteLaudoExame,
                NumeroLeitos = empresa.NumeroLeitos,
                NumeroLeitosDisponivel = empresa.NumeroLeitosDisponivel,
                NumeroLeitosUti = empresa.NumeroLeitosUti,
                NumeroLeitosUtidisponivel = empresa.NumeroLeitosUtidisponivel,
                PossuiLeitosInternacao = empresa.PossuiLeitosInternacao
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
                Email = empresa.Email,
                EmiteLaudoExame = empresa.EmiteLaudoExame,
                NumeroLeitos = empresa.NumeroLeitos,
                NumeroLeitosDisponivel = empresa.NumeroLeitosDisponivel,
                NumeroLeitosUti = empresa.NumeroLeitosUti,
                NumeroLeitosUtidisponivel = empresa.NumeroLeitosUtidisponivel,
                PossuiLeitosInternacao = empresa.PossuiLeitosInternacao
            }).FirstOrDefault();

        public EmpresaExameModel GetByCnpj(string cnpj)
         => _context.Empresaexame
            .Where(empresaExame => empresaExame.Cnpj.Equals(cnpj))
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
                Email = empresa.Email,
                EmiteLaudoExame = empresa.EmiteLaudoExame,
                NumeroLeitos = empresa.NumeroLeitos,
                NumeroLeitosDisponivel = empresa.NumeroLeitosDisponivel,
                NumeroLeitosUti = empresa.NumeroLeitosUti,
                NumeroLeitosUtidisponivel = empresa.NumeroLeitosUtidisponivel,
                PossuiLeitosInternacao = empresa.PossuiLeitosInternacao
            }).FirstOrDefault();

    }
}
