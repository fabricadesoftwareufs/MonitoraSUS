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
            var pessoa = _context.Pessoa.Find(id);
            _context.Pessoa.Remove(pessoa);
            return _context.SaveChanges() == 1 ? true : false;
        }

        public List<PessoaModel> GetAll()
         => _context.Pessoa
                .Select(pessoa => new PessoaModel
                {
                    Idpessoa = pessoa.Idpessoa,
                    Nome = pessoa.Nome,
                    Cpf = pessoa.Cpf,
                    Email = pessoa.Email,
                    FoneFixo = pessoa.FoneFixo,
                    FoneCelular = pessoa.FoneCelular,
                    DataNascimento = pessoa.DataNascimento,
                    Sexo = pessoa.Sexo,
                    Cep = pessoa.Cep,
                    Rua = pessoa.Rua,
                    Estado = pessoa.Estado,
                    Cidade = pessoa.Cidade,
                    Complemento = pessoa.Complemento,
                    Bairro = pessoa.Bairro,
                    Numero = pessoa.Numero,
                    Latitude = pessoa.Latitude,
                    Longitude = pessoa.Longitude,
                    Imunodeprimido = Convert.ToBoolean(pessoa.Imunodeprimido),
                    Cancer = Convert.ToBoolean(pessoa.Cancer),
                    Hipertenso = Convert.ToBoolean(pessoa.Hipertenso),
                    Obeso = Convert.ToBoolean(pessoa.Obeso),
                    Diabetes = Convert.ToBoolean(pessoa.Diabetes),
                    DoencaRespiratoria = Convert.ToBoolean(pessoa.DoencaRespiratoria),
                    Cardiopatia = Convert.ToBoolean(pessoa.Cardiopatia)
                }).ToList();

        public PessoaModel GetById(int id)
        => _context.Pessoa
                .Where(pessoaModel => pessoaModel.Idpessoa == id)
                .Select(pessoa => new PessoaModel
                {
                    Idpessoa = pessoa.Idpessoa,
                    Nome = pessoa.Nome,
                    Cpf = pessoa.Cpf,
                    Email = pessoa.Email,
                    FoneFixo = pessoa.FoneFixo,
                    FoneCelular = pessoa.FoneCelular,
                    DataNascimento = pessoa.DataNascimento,
                    Sexo = pessoa.Sexo,
                    Cep = pessoa.Cep,
                    Rua = pessoa.Rua,
                    Estado = pessoa.Estado,
                    Cidade = pessoa.Cidade,
                    Complemento = pessoa.Complemento,
                    Bairro = pessoa.Bairro,
                    Numero = pessoa.Numero,
                    Latitude = pessoa.Latitude,
                    Longitude = pessoa.Longitude,
                    Imunodeprimido = Convert.ToBoolean(pessoa.Imunodeprimido),
                    Cancer = Convert.ToBoolean(pessoa.Cancer),
                    Hipertenso = Convert.ToBoolean(pessoa.Hipertenso),
                    Obeso = Convert.ToBoolean(pessoa.Obeso),
                    Diabetes = Convert.ToBoolean(pessoa.Diabetes),
                    DoencaRespiratoria = Convert.ToBoolean(pessoa.DoencaRespiratoria),
                    Cardiopatia = Convert.ToBoolean(pessoa.Cardiopatia)
                }).FirstOrDefault();

        public bool Insert(PessoaModel pessoaModel)
        {
            _context.Add(ModelToEntity(pessoaModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

        private Pessoa ModelToEntity(PessoaModel pessoaModel)
        {
            Pessoa pessoa = new Pessoa();
            pessoa.Idpessoa = pessoaModel.Idpessoa;
            pessoa.Nome = pessoaModel.Nome;
            pessoa.Cpf = pessoaModel.Cpf;
            pessoa.Email = pessoaModel.Email;
            pessoa.FoneFixo = pessoaModel.FoneFixo;
            pessoa.FoneCelular = pessoaModel.FoneCelular;
            pessoa.DataNascimento = pessoaModel.DataNascimento;
            pessoa.Sexo = pessoaModel.Sexo;
            pessoa.Cep = pessoaModel.Cep;
            pessoa.Rua = pessoaModel.Rua;
            pessoa.Estado = pessoaModel.Estado;
            pessoa.Cidade = pessoaModel.Cidade;
            pessoa.Complemento = pessoaModel.Complemento;
            pessoa.Bairro = pessoaModel.Bairro;
            pessoa.Numero = pessoaModel.Numero;
            pessoa.Latitude = pessoaModel.Latitude;
            pessoa.Longitude = pessoaModel.Longitude;
            pessoa.Imunodeprimido = Convert.ToByte(pessoaModel.Imunodeprimido);
            pessoa.Cancer = Convert.ToByte(pessoaModel.Cancer);
            pessoa.Hipertenso = Convert.ToByte(pessoaModel.Hipertenso);
            pessoa.Obeso = Convert.ToByte(pessoaModel.Obeso);
            pessoa.Diabetes = Convert.ToByte(pessoaModel.Diabetes);
            pessoa.DoencaRespiratoria = Convert.ToByte(pessoaModel.DoencaRespiratoria);
            pessoa.Cardiopatia = Convert.ToByte(pessoaModel.Cardiopatia);

            return pessoa;
        }

        public bool Update(PessoaModel pessoaModel)
        {
            _context.Update(ModelToEntity(pessoaModel));
            return _context.SaveChanges() == 1 ? true : false;
        }


        public PessoaModel GetByCpf(string cpf)
         => _context.Pessoa
                .Where(pessoaModel => pessoaModel.Cpf.Equals(cpf))
                .Select(pessoa => new PessoaModel
                {
                    Idpessoa = pessoa.Idpessoa,
                    Nome = pessoa.Nome,
                    Cpf = pessoa.Cpf,
                    Email = pessoa.Email,
                    FoneFixo = pessoa.FoneFixo,
                    FoneCelular = pessoa.FoneCelular,
                    DataNascimento = pessoa.DataNascimento,
                    Sexo = pessoa.Sexo,
                    Cep = pessoa.Cep,
                    Rua = pessoa.Rua,
                    Estado = pessoa.Estado,
                    Cidade = pessoa.Cidade,
                    Complemento = pessoa.Complemento,
                    Bairro = pessoa.Bairro,
                    Numero = pessoa.Numero,
                    Latitude = pessoa.Latitude,
                    Longitude = pessoa.Longitude,
                    Imunodeprimido = Convert.ToBoolean(pessoa.Imunodeprimido),
                    Cancer = Convert.ToBoolean(pessoa.Cancer),
                    Hipertenso = Convert.ToBoolean(pessoa.Hipertenso),
                    Obeso = Convert.ToBoolean(pessoa.Obeso),
                    Diabetes = Convert.ToBoolean(pessoa.Diabetes),
                    DoencaRespiratoria = Convert.ToBoolean(pessoa.DoencaRespiratoria),
                    Cardiopatia = Convert.ToBoolean(pessoa.Cardiopatia)
                }).FirstOrDefault();

    }
}
