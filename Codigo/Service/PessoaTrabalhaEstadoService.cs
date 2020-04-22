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
        public bool Delete(int idPessoa, int idEstado)
        {
            var agente = _context.Pessoatrabalhaestado.Find(idPessoa, idEstado);
            _context.Pessoatrabalhaestado.Remove(agente);
            return _context.SaveChanges() == 1 ? true : false;
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
                    IdEmpresaExame = p.IdEmpresaExame
                }).ToList();

        public List<PessoaTrabalhaEstadoModel> GetAllAgents()
            => _context
                .Pessoatrabalhaestado
                .Where(p => p.EhSecretario.Equals(0))
                .Select(p => new PessoaTrabalhaEstadoModel
                {
                    IdPessoa = p.Idpessoa,
                    IdEstado = p.IdEstado,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro,
                    IdEmpresaExame = p.IdEmpresaExame
                }).ToList();

        public List<PessoaTrabalhaEstadoModel> GetAllGestores()
            => _context
                .Pessoatrabalhaestado
                .Where(p => p.EhResponsavel.Equals(1))
                .Select(p => new PessoaTrabalhaEstadoModel
                {
                    IdPessoa = p.Idpessoa,
                    IdEstado = p.IdEstado,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro,
                    IdEmpresaExame = p.IdEmpresaExame
                }).ToList();

        public PessoaTrabalhaEstadoModel GetById(int idPessoa, int idEstado)
            => _context
                .Pessoatrabalhaestado
                .Where(p => p.Idpessoa == idPessoa && p.IdEstado == idEstado)
                .Select(p => new PessoaTrabalhaEstadoModel
                {
                    IdPessoa = p.Idpessoa,
                    IdEstado = p.IdEstado,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro,
                    IdEmpresaExame = p.IdEmpresaExame
                }).FirstOrDefault();

        public PessoaTrabalhaEstadoModel GetByIdPessoa(int idPessoa)
            => _context
                .Pessoatrabalhaestado
                .Where(p => p.Idpessoa == idPessoa)
                .Select(p => new PessoaTrabalhaEstadoModel
                {
                    IdPessoa = p.Idpessoa,
                    IdEstado = p.IdEstado,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro,
                    IdEmpresaExame = p.IdEmpresaExame
                }).FirstOrDefault();

        public PessoaTrabalhaEstadoModel GetSecretarioAtivoByIdPessoa(int idPessoa)
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
                    _context.Pessoatrabalhaestado.Add(ModelToEntity(pessoaTrabalhaEstadoModel));
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
            _context.Update(ModelToEntity(pessoaTrabalhaEstadoModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

        private Pessoatrabalhaestado ModelToEntity(PessoaTrabalhaEstadoModel model)
        {
            return new Pessoatrabalhaestado
            {
                IdEstado = model.IdEstado,
                Idpessoa = model.IdPessoa,
                EhResponsavel = Convert.ToByte(model.EhResponsavel),
                EhSecretario = Convert.ToByte(model.EhSecretario),
                SituacaoCadastro = model.SituacaoCadastro,
                IdEmpresaExame = model.IdEmpresaExame
            };
        }
    }
}
