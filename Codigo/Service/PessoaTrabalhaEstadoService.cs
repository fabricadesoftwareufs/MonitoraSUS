using Model;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class PessoaTrabalhaEstadoService : IPessoaTrabalhaEstadoService
    {
        private readonly monitorasusContext _context;
        public PessoaTrabalhaEstadoService(monitorasusContext context)
        {
            _context = context;
        }
        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<PessoaTrabalhaEstadoModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public PessoaTrabalhaEstadoModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public bool Insert(PessoaTrabalhaEstadoModel pessoaTrabalhaEstadoModel)
        {
            if (pessoaTrabalhaEstadoModel != null)
            {
                try
                {
                    _context.Pessoatrabalhaestado.Add(ModelToEntity(pessoaTrabalhaEstadoModel, new Pessoatrabalhaestado()));
                    return _context.SaveChanges() == 1 ? true : false;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

            return false;
        }

        public bool Update(PessoaTrabalhaEstadoModel pessoaTrabalhaEstadoModel)
        {
            throw new NotImplementedException();
        }

        private Pessoatrabalhaestado ModelToEntity(PessoaTrabalhaEstadoModel model, Pessoatrabalhaestado entity)
        {
            entity.IdEstado = model.IdEstado;
            entity.IdPessoa = model.IdPessoa;
            entity.EhResponsavel = Convert.ToByte(model.EhResponsavel);
            entity.EhSecretario = Convert.ToByte(model.EhSecretario);
            entity.SituacaoCadastro = model.SituacaoCadastro;

            return entity;
        }
    }
}
