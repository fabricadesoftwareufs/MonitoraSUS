using Model;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
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

        public PessoaModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public PessoaModel Insert(PessoaModel pessoaModel)
        {
            if (pessoaModel != null)
            {
                try
                {
                    _context.Pessoa.Add(ModelToEntity(pessoaModel, new Pessoa()));
                    return _context.SaveChanges() == 1 ? pessoaModel : null;
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

            entity.Sexo = model.Sexo;
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
