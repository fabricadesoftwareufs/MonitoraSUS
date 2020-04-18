using Model;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    IdPessoa = p.Idpessoa,
                    IdEstado = p.IdEstado,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro,
                    IdEmpresaExame = p.IdEmpresaExame,
                }).ToList();

        public List<PessoaTrabalhaEstadoModel> GetAllSecretariesPendents()
            => _context
                .Pessoatrabalhaestado
                .Where(p => p.EhSecretario.Equals(1) && p.SituacaoCadastro.Equals("S"))
                .Select(p => new PessoaTrabalhaEstadoModel
                {
                    IdPessoa = p.Idpessoa,
                    IdEstado = p.IdEstado,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro,
                    IdEmpresaExame = p.IdEmpresaExame,
                }).ToList();

        public PessoaTrabalhaEstadoModel GetById(int idPessoa, int idEstado)
         => _context.Pessoatrabalhaestado
                .Where(p => p.Idpessoa == idPessoa && p.IdEstado == idEstado)
                .Select(p => new PessoaTrabalhaEstadoModel
                {
                    IdPessoa = p.Idpessoa,
                    IdEstado = p.IdEstado,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro,
                    IdEmpresaExame = p.IdEmpresaExame,
                }).FirstOrDefault();

        public PessoaTrabalhaEstadoModel GetByIdPessoa(int idPessoa)
        => _context
                .Pessoatrabalhaestado
                .Where(p => p.EhSecretario.Equals(1) && p.SituacaoCadastro.Equals("A") && p.Idpessoa == idPessoa)
                .Select(p => new PessoaTrabalhaEstadoModel
                {
                    IdPessoa = p.Idpessoa,
                    IdEstado = p.IdEstado,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro,
                    IdEmpresaExame = p.IdEmpresaExame,
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
            entity.Idpessoa = model.IdPessoa;
            entity.EhResponsavel = Convert.ToByte(model.EhResponsavel);
            entity.EhSecretario = Convert.ToByte(model.EhSecretario);
            entity.SituacaoCadastro = model.SituacaoCadastro;
            entity.IdEmpresaExame = model.IdEmpresaExame;

            return entity;
        }
    }
}
