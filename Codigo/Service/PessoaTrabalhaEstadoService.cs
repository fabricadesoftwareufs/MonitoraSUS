using Model;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
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
            => _context
                .Pessoatrabalhaestado
                .Select(p => new PessoaTrabalhaEstadoModel
                {
                    IdPessoa = p.IdPessoa,
                    IdEstado = p.IdEstado,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro
                }).ToList();

        public List<PessoaTrabalhaEstadoModel> GetAllAgents()
            => _context
                .Pessoatrabalhaestado
                .Where(p => p.EhSecretario.Equals(0))
                .Select(p => new PessoaTrabalhaEstadoModel
                {
                    IdPessoa = p.IdPessoa,
                    IdEstado = p.IdEstado,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro
                }).ToList();

        public List<PessoaTrabalhaEstadoModel> GetAllSecretariesPendents()
            => _context
                .Pessoatrabalhaestado
                .Where(p => p.EhSecretario.Equals(1) && p.SituacaoCadastro.Contains('S'))
                .Select(p => new PessoaTrabalhaEstadoModel
                {
                    IdPessoa = p.IdPessoa,
                    IdEstado = p.IdEstado,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro
                }).ToList();

        public PessoaTrabalhaEstadoModel GetById(int id)
            => _context
                .Pessoatrabalhaestado
                .Where(p => p.IdPessoa == id)
                .Select(p => new PessoaTrabalhaEstadoModel
                {
                    IdPessoa = p.IdPessoa,
                    IdEstado = p.IdEstado,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro
                }).FirstOrDefault();


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
