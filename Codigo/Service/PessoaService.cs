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
        {
            throw new NotImplementedException();
        }

        public PessoaModel GetById(int id) =>
            _context
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

        public bool Insert(PessoaModel pessoaModel)
        {
            throw new NotImplementedException();
        }

        public bool Update(PessoaModel pessoaModel)
        {
            throw new NotImplementedException();
        }
    }
}
