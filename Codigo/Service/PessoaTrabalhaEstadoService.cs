using Model;
using Model.ViewModel;
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
        public bool Delete(int idPessoa)
        {
            var pessoas = _context.Pessoatrabalhaestado.Where(p => p.Idpessoa.Equals(idPessoa));
            foreach (Pessoatrabalhaestado pessoa in pessoas)
                _context.Pessoatrabalhaestado.Remove(pessoa);
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

        public List<SolicitanteAprovacaoViewModel> GetAllGestores()
            => _context
                .Pessoatrabalhaestado
                .Where(p => p.EhResponsavel.Equals(1))
                .Select(p => new SolicitanteAprovacaoViewModel
                {
                    IdPessoa = p.Idpessoa,
                    Cidade = (p.IdEmpresaExame == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO) ? "N/A" : p.IdEmpresaExameNavigation.Nome,
                    Cpf = p.IdpessoaNavigation.Cpf,
                    Nome = p.IdpessoaNavigation.Nome,
                    Estado = p.IdEstadoNavigation.Uf,
                    Status = p.SituacaoCadastro,
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
					IdEmpresa = p.IdEmpresaExame,
					FoneCelular = p.IdpessoaNavigation.FoneCelular
                }).ToList();
        public List<SolicitanteAprovacaoViewModel> GetAllGestoresEstado(int idEstado)
            => _context
                .Pessoatrabalhaestado
                .Where(p => p.EhResponsavel.Equals(1) && p.IdEstado.Equals(idEstado))
                .Select(p => new SolicitanteAprovacaoViewModel
                {
                    IdPessoa = p.Idpessoa,
                    Cidade = (p.IdEmpresaExame == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO) ? "N/A" : p.IdEmpresaExameNavigation.Nome,
                    Cpf = p.IdpessoaNavigation.Cpf,
                    Nome = p.IdpessoaNavigation.Nome,
                    Estado = p.IdEstadoNavigation.Uf,
                    Status = p.SituacaoCadastro,
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
					IdEmpresa = p.IdEmpresaExame,
					FoneCelular = p.IdpessoaNavigation.FoneCelular
				}).ToList();
        public List<SolicitanteAprovacaoViewModel> GetAllGestoresEmpresa(int idEmpresa)
            => _context
                .Pessoatrabalhaestado
                .Where(p => p.EhResponsavel.Equals(1) && p.IdEmpresaExame.Equals(idEmpresa))
                .Select(p => new SolicitanteAprovacaoViewModel
                {
                    IdPessoa = p.Idpessoa,
                    Cidade = (p.IdEmpresaExame == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO) ? "N/A" : p.IdEmpresaExameNavigation.Nome,
                    Cpf = p.IdpessoaNavigation.Cpf,
                    Nome = p.IdpessoaNavigation.Nome,
                    Estado = p.IdEstadoNavigation.Uf,
                    Status = p.SituacaoCadastro,
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
					IdEmpresa = p.IdEmpresaExame,
					FoneCelular = p.IdpessoaNavigation.FoneCelular
				}).ToList();
        public List<SolicitanteAprovacaoViewModel> GetAllNotificadores()
            => _context
                .Pessoatrabalhaestado
                .Where(p => p.EhResponsavel.Equals(0) && p.EhSecretario.Equals(0))
                .Select(p => new SolicitanteAprovacaoViewModel
                {
                    IdPessoa = p.Idpessoa,
                    Cidade = (p.IdEmpresaExame == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO) ? "N/A" : p.IdEmpresaExameNavigation.Nome,
                    Cpf = p.IdpessoaNavigation.Cpf,
                    Nome = p.IdpessoaNavigation.Nome,
                    Estado = p.IdEstadoNavigation.Uf,
                    Status = p.SituacaoCadastro,
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
					IdEmpresa = p.IdEmpresaExame,
					FoneCelular = p.IdpessoaNavigation.FoneCelular
				}).ToList();
        public List<SolicitanteAprovacaoViewModel> GetAllNotificadoresEstado(int idEstado)
                => _context
                .Pessoatrabalhaestado
                .Where(p => p.EhResponsavel.Equals(0) && p.EhSecretario.Equals(0) && p.IdEstado.Equals(idEstado))
                .Select(p => new SolicitanteAprovacaoViewModel
                {
                    IdPessoa = p.Idpessoa,
                    Cidade = (p.IdEmpresaExame == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO) ? "N/A" : p.IdEmpresaExameNavigation.Nome,
                    Cpf = p.IdpessoaNavigation.Cpf,
                    Nome = p.IdpessoaNavigation.Nome,
                    Estado = p.IdEstadoNavigation.Uf,
                    Status = p.SituacaoCadastro,
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
					IdEmpresa = p.IdEmpresaExame,
					FoneCelular = p.IdpessoaNavigation.FoneCelular
				}).ToList();
        public List<SolicitanteAprovacaoViewModel> GetAllNotificadoresEmpresa(int idEmpresa)
                => _context
                .Pessoatrabalhaestado
                .Where(p => p.EhResponsavel.Equals(0) && p.EhSecretario.Equals(0) && p.IdEmpresaExame.Equals(idEmpresa))
                .Select(p => new SolicitanteAprovacaoViewModel
                {
                    IdPessoa = p.Idpessoa,
                    Cidade = (p.IdEmpresaExame == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO) ? "N/A" : p.IdEmpresaExameNavigation.Nome,
                    Cpf = p.IdpessoaNavigation.Cpf,
                    Nome = p.IdpessoaNavigation.Nome,
                    Estado = p.IdEstadoNavigation.Uf,
                    Status = p.SituacaoCadastro,
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
					IdEmpresa = p.IdEmpresaExame,
					FoneCelular = p.IdpessoaNavigation.FoneCelular
				}).ToList();

        public PessoaTrabalhaEstadoModel GetByIdPessoa(int idPessoa)
            => _context
                .Pessoatrabalhaestado
                .Where(p => p.Idpessoa.Equals(idPessoa))
                .Select(p => new PessoaTrabalhaEstadoModel
                {
                    IdPessoa = p.Idpessoa,
                    IdEstado = p.IdEstado,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro,
                    IdEmpresaExame = p.IdEmpresaExame
                }).FirstOrDefault();

        public List<PessoaTrabalhaEstadoModel> GetByIdEmpresa(int idEmpresa)
           => _context
               .Pessoatrabalhaestado
               .Where(p => p.IdEmpresaExame.Equals(idEmpresa))
               .Select(p => new PessoaTrabalhaEstadoModel
               {
                   IdPessoa = p.Idpessoa,
                   IdEstado = p.IdEstado,
                   EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                   EhSecretario = Convert.ToBoolean(p.EhSecretario),
                   SituacaoCadastro = p.SituacaoCadastro,
                   IdEmpresaExame = p.IdEmpresaExame
               }).ToList();

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
