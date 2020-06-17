using Model;
using Persistence;
using Service.Interface;
using System;
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
                EmiteLaudoExame = Convert.ToByte(empresa.EmiteLaudoExame),
				EhPublico = Convert.ToByte(empresa.EhPublico),
				FazMonitoramento = Convert.ToByte(empresa.FazMonitoramento),
				NumeroLeitos = empresa.PossuiLeitosInternacao ? empresa.NumeroLeitos : 0,
                NumeroLeitosDisponivel = empresa.PossuiLeitosInternacao ? empresa.NumeroLeitosDisponivel : 0,
                NumeroLeitosUti = empresa.PossuiLeitosInternacao ? empresa.NumeroLeitosUti : 0,
                NumeroLeitosUtidisponivel = empresa.PossuiLeitosInternacao ? empresa.NumeroLeitosUtidisponivel : 0,
                PossuiLeitosInternacao = Convert.ToByte(empresa.PossuiLeitosInternacao),
				Cnes = empresa.Cnes
            };
        }

		public List<EmpresaExameModel> GetHospitais()
	   => _context.Empresaexame
		   .Where(empresaExame => empresaExame.Id != 1 && empresaExame.PossuiLeitosInternacao==1)
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
			   EmiteLaudoExame = Convert.ToBoolean(empresa.EmiteLaudoExame),
			   NumeroLeitos = empresa.NumeroLeitos,
			   NumeroLeitosDisponivel = empresa.NumeroLeitosDisponivel,
			   NumeroLeitosUti = empresa.NumeroLeitosUti,
			   NumeroLeitosUtidisponivel = empresa.NumeroLeitosUtidisponivel,
			   PossuiLeitosInternacao = Convert.ToBoolean(empresa.PossuiLeitosInternacao),
			   EhPublico = Convert.ToBoolean(empresa.EhPublico),
			   FazMonitoramento = Convert.ToBoolean(empresa.FazMonitoramento),
			   Cnes = empresa.Cnes
		   }).ToList();

		public List<EmpresaExameModel> GetAll()
        => _context.Empresaexame
            .Where(empresaExame => empresaExame.Id != 1)
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
                EmiteLaudoExame = Convert.ToBoolean(empresa.EmiteLaudoExame),
                NumeroLeitos = empresa.NumeroLeitos,
                NumeroLeitosDisponivel = empresa.NumeroLeitosDisponivel,
                NumeroLeitosUti = empresa.NumeroLeitosUti,
                NumeroLeitosUtidisponivel = empresa.NumeroLeitosUtidisponivel,
                PossuiLeitosInternacao = Convert.ToBoolean(empresa.PossuiLeitosInternacao),
				EhPublico = Convert.ToBoolean(empresa.EhPublico),
				FazMonitoramento = Convert.ToBoolean(empresa.FazMonitoramento),
				Cnes = empresa.Cnes
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
                EmiteLaudoExame = Convert.ToBoolean(empresa.EmiteLaudoExame),
                NumeroLeitos = empresa.NumeroLeitos,
                NumeroLeitosDisponivel = empresa.NumeroLeitosDisponivel,
                NumeroLeitosUti = empresa.NumeroLeitosUti,
                NumeroLeitosUtidisponivel = empresa.NumeroLeitosUtidisponivel,
                PossuiLeitosInternacao = Convert.ToBoolean(empresa.PossuiLeitosInternacao),
				EhPublico = Convert.ToBoolean(empresa.EhPublico),
				FazMonitoramento = Convert.ToBoolean(empresa.FazMonitoramento),
				Cnes = empresa.Cnes
			}).FirstOrDefault();

        public List<EmpresaExameModel> GetByCnpj(string cnpj)
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
                EmiteLaudoExame = Convert.ToBoolean(empresa.EmiteLaudoExame),
                NumeroLeitos = empresa.NumeroLeitos,
                NumeroLeitosDisponivel = empresa.NumeroLeitosDisponivel,
                NumeroLeitosUti = empresa.NumeroLeitosUti,
                NumeroLeitosUtidisponivel = empresa.NumeroLeitosUtidisponivel,
                PossuiLeitosInternacao = Convert.ToBoolean(empresa.PossuiLeitosInternacao),
				EhPublico = Convert.ToBoolean(empresa.EhPublico),
				FazMonitoramento = Convert.ToBoolean(empresa.FazMonitoramento),
				Cnes = empresa.Cnes
			}).ToList();

        public List<EmpresaExameModel> GetByUF(string uf)
             => _context.Empresaexame
                .Where(empresaExame => empresaExame.Estado.ToUpper().Equals(uf.ToUpper()))
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
                    EmiteLaudoExame = Convert.ToBoolean(empresa.EmiteLaudoExame),
                    NumeroLeitos = empresa.NumeroLeitos,
                    NumeroLeitosDisponivel = empresa.NumeroLeitosDisponivel,
                    NumeroLeitosUti = empresa.NumeroLeitosUti,
                    NumeroLeitosUtidisponivel = empresa.NumeroLeitosUtidisponivel,
                    PossuiLeitosInternacao = Convert.ToBoolean(empresa.PossuiLeitosInternacao),
					EhPublico = Convert.ToBoolean(empresa.EhPublico),
					FazMonitoramento = Convert.ToBoolean(empresa.FazMonitoramento),
					Cnes = empresa.Cnes
				}).ToList();

        public List<EmpresaExameModel> ListByUF(string uf)
             => _context.Empresaexame
                .Where(empresaExame => empresaExame.Id == 1 || empresaExame.Estado.ToUpper().Equals(uf.ToUpper()))
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
                    EmiteLaudoExame = Convert.ToBoolean(empresa.EmiteLaudoExame),
                    NumeroLeitos = empresa.NumeroLeitos,
                    NumeroLeitosDisponivel = empresa.NumeroLeitosDisponivel,
                    NumeroLeitosUti = empresa.NumeroLeitosUti,
                    NumeroLeitosUtidisponivel = empresa.NumeroLeitosUtidisponivel,
                    PossuiLeitosInternacao = Convert.ToBoolean(empresa.PossuiLeitosInternacao),
					EhPublico = Convert.ToBoolean(empresa.EhPublico),
					FazMonitoramento = Convert.ToBoolean(empresa.FazMonitoramento),
					Cnes = empresa.Cnes
				}).ToList();
        public List<EmpresaExameModel> ListAll()
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
                    EmiteLaudoExame = Convert.ToBoolean(empresa.EmiteLaudoExame),
                    NumeroLeitos = empresa.NumeroLeitos,
                    NumeroLeitosDisponivel = empresa.NumeroLeitosDisponivel,
                    NumeroLeitosUti = empresa.NumeroLeitosUti,
                    NumeroLeitosUtidisponivel = empresa.NumeroLeitosUtidisponivel,
                    PossuiLeitosInternacao = Convert.ToBoolean(empresa.PossuiLeitosInternacao),
					EhPublico = Convert.ToBoolean(empresa.EhPublico),
					FazMonitoramento = Convert.ToBoolean(empresa.FazMonitoramento),
					Cnes = empresa.Cnes
				}).ToList();
    }
}
