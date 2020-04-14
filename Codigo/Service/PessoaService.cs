using Model;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class PessoaService : IPessoaService
    {
        private readonly monitorasusContext _context;
        public PessoaService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<PessoaModel> GetAll()
            => _context
                .Pessoa
                .Select(p => new PessoaModel
                {
                    Idpessoa = p.Idpessoa,
                    Cpf = p.Cpf,
                    Nome = p.Nome,
                    Bairro = p.Bairro,
                    Cep = p.Cep,
                    Cancer = Convert.ToBoolean(p.Cancer),
                    Cardiopatia = Convert.ToBoolean(p.Cardiopatia),
                    Cidade = p.Cidade,
                    Complemento = p.Complemento,
                    DataNascimento = p.DataNascimento,
                    Diabetes = Convert.ToBoolean(p.Diabetes),
                    DoencaRespiratoria = Convert.ToBoolean(p.DoencaRespiratoria),
                    Email = p.Email,
                    Estado = p.Estado,
                    FoneCelular = p.FoneCelular,
                    FoneFixo = p.FoneFixo,
                    Hipertenso = Convert.ToBoolean(p.Hipertenso),
                    Imunodeprimido = Convert.ToBoolean(p.Imunodeprimido),
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    Numero = p.Numero,
                    Obeso = Convert.ToBoolean(p.Obeso),
                    Rua = p.Rua,
                    Sexo = p.Sexo
                }).ToList();

        public PessoaModel GetById(int id)
            => _context
                .Pessoa
                .Where(p => p.Idpessoa == id)
                .Select(model => new PessoaModel
                {
                  Idpessoa = model.Idpessoa,
                  Nome = model.Nome,
                  Sexo = model.Sexo,
                  Cpf = model.Cpf,
                  DataNascimento = model.DataNascimento,
                  Email = model.Email,
                  FoneCelular = model.FoneCelular,
                  FoneFixo = model.FoneFixo,
                  Latitude = model.Latitude,
                  Longitude = model.Longitude,
                  Estado = model.Estado,
                  Bairro = model.Bairro,
                  Cep = model.Cep,
                  Rua = model.Rua,
                  Complemento = model.Complemento,
                  Cidade = model.Cidade,
                  Numero = model.Numero,
                  Cancer = Convert.ToBoolean(model.Cancer),
                  Cardiopatia = Convert.ToBoolean(model.Cardiopatia),
                  Diabetes = Convert.ToBoolean(model.Diabetes),
                  DoencaRespiratoria = Convert.ToBoolean(model.DoencaRespiratoria),
                  Hipertenso = Convert.ToBoolean(model.Hipertenso),
                  Imunodeprimido = Convert.ToBoolean(model.Imunodeprimido),
                  Obeso = Convert.ToBoolean(model.Obeso)
                 }).FirstOrDefault();

        public PessoaModel Insert(PessoaModel pessoaModel)
        {
            if (pessoaModel != null)
            {
                try
                {
                    var pessoaInserida = new Pessoa();
                    _context.Pessoa.Add(ModelToEntity(pessoaModel, pessoaInserida));
                    _context.SaveChanges();

                    // Returning the last inserted ID.
                    pessoaModel.Idpessoa = pessoaInserida.Idpessoa;
                    return pessoaModel;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

            return null;
        }

        public bool Update(PessoaModel pessoaModel)
        {
            throw new NotImplementedException();
        }
        
        // 
        private Pessoa ModelToEntity(PessoaModel model, Pessoa entity)
        {
            entity.Idpessoa = model.Idpessoa;
            entity.Nome = model.Nome;
            entity.Cpf = model.Cpf;
            entity.Sexo = model.Sexo == "Masculino" ? "M" : "F";
            entity.Cep = model.Cep;
            entity.Rua = model.Rua;
            entity.Bairro = model.Bairro;
            entity.Cidade = model.Cidade;
            entity.Estado = model.Estado;
            entity.Numero = model.Numero;
            entity.Complemento = model.Complemento;
            entity.Latitude = model.Latitude;
            entity.Longitude = model.Longitude;
            entity.FoneCelular = model.FoneCelular;
            entity.FoneFixo = model.FoneFixo;
            entity.Email = model.Email;
            entity.DataNascimento = model.DataNascimento;
            entity.Hipertenso = Convert.ToByte(model.Hipertenso);
            entity.Diabetes = Convert.ToByte(model.Diabetes);
            entity.Obeso = Convert.ToByte(model.Obeso);
            entity.Cardiopatia = Convert.ToByte(model.Cardiopatia);
            entity.Imunodeprimido = Convert.ToByte(model.Imunodeprimido);
            entity.Cancer = Convert.ToByte(model.Cancer);
            entity.DoencaRespiratoria = Convert.ToByte(model.DoencaRespiratoria);

            return entity;
         }
  }
}