using Model;
using Model.ViewModel;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class PessoaTrabalhaMunicipioService : IPessoaTrabalhaMunicipioService
    {
        private readonly monitorasusContext _context;
        public PessoaTrabalhaMunicipioService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Delete(int idPessoa)
        {
            var pessoas = _context.Pessoatrabalhamunicipio.Where(p => p.IdPessoa == idPessoa);
            foreach (Pessoatrabalhamunicipio pessoa in pessoas)
                _context.Pessoatrabalhamunicipio.Remove(pessoa);
            return _context.SaveChanges() == 1 ? true : false;
        }


        public List<PessoaTrabalhaMunicipioModel> GetAll()
            => _context
                .Pessoatrabalhamunicipio
                .Select(p => new PessoaTrabalhaMunicipioModel
                {
                    IdPessoa = p.IdPessoa,
                    IdMunicipio = p.IdMunicipio,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro
                }).ToList();

        public List<SolicitanteAprovacaoViewModel> GetAllGestores()
            => _context
                .Pessoatrabalhamunicipio
                .Where(p => p.EhResponsavel.Equals(1))
                .Select(p => new SolicitanteAprovacaoViewModel
                {
                    IdPessoa = p.IdPessoa,
                    Cpf = p.IdPessoaNavigation.Cpf,
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    Nome = p.IdPessoaNavigation.Nome,
                    Cidade = p.IdMunicipioNavigation.Nome,
                    Estado = p.IdMunicipioNavigation.Uf,
                    Status = p.SituacaoCadastro,
					IdEmpresa = 1,
					FoneCelular = p.IdPessoaNavigation.FoneCelular
				}).ToList();

        public List<SolicitanteAprovacaoViewModel> GetAllGestoresMunicipio(int idMunicipio)
            => _context
                .Pessoatrabalhamunicipio
                .Where(p => p.EhResponsavel.Equals(1) && p.IdMunicipio.Equals(idMunicipio))
                .Select(p => new SolicitanteAprovacaoViewModel
                {
                    IdPessoa = p.IdPessoa,
                    Cpf = p.IdPessoaNavigation.Cpf,
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    Nome = p.IdPessoaNavigation.Nome,
                    Cidade = p.IdMunicipioNavigation.Nome,
                    Estado = p.IdMunicipioNavigation.Uf,
                    Status = p.SituacaoCadastro,
					IdEmpresa = 1,
					FoneCelular = p.IdPessoaNavigation.FoneCelular
				}).ToList();

        public List<SolicitanteAprovacaoViewModel> GetAllNotificadores()
                        => _context
                .Pessoatrabalhamunicipio
                .Where(p => p.EhResponsavel.Equals(0))
                .Select(p => new SolicitanteAprovacaoViewModel
                {
                    IdPessoa = p.IdPessoa,
                    Cpf = p.IdPessoaNavigation.Cpf,
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    Nome = p.IdPessoaNavigation.Nome,
                    Cidade = p.IdMunicipioNavigation.Nome,
                    Estado = p.IdMunicipioNavigation.Uf,
                    Status = p.SituacaoCadastro,
					IdEmpresa = 1,
					FoneCelular = p.IdPessoaNavigation.FoneCelular
				}).ToList();

        public List<SolicitanteAprovacaoViewModel> GetAllNotificadoresMunicipio(int idMunicipio)
                        => _context
                .Pessoatrabalhamunicipio
                .Where(p => p.EhResponsavel.Equals(0) && p.IdMunicipio.Equals(idMunicipio))
                .Select(p => new SolicitanteAprovacaoViewModel
                {
                    IdPessoa = p.IdPessoa,
                    Cpf = p.IdPessoaNavigation.Cpf,
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    Nome = p.IdPessoaNavigation.Nome,
                    Cidade = p.IdMunicipioNavigation.Nome,
                    Estado = p.IdMunicipioNavigation.Uf,
                    Status = p.SituacaoCadastro,
					IdEmpresa = 1,
					FoneCelular = p.IdPessoaNavigation.FoneCelular
				}).ToList();


        public PessoaTrabalhaMunicipioModel GetByIdPessoa(int idPessoa)
         => _context
                .Pessoatrabalhamunicipio
                .Where(pessoaMunicipio => pessoaMunicipio.IdPessoa == idPessoa)
                .Select(p => new PessoaTrabalhaMunicipioModel
                {
                    IdPessoa = p.IdPessoa,
                    IdMunicipio = p.IdMunicipio,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro
                }).FirstOrDefault();

        public bool Insert(PessoaTrabalhaMunicipioModel pessoaTrabalhaMunicipioModel)
        {
            if (pessoaTrabalhaMunicipioModel != null)
            {
                try
                {
                    _context.Pessoatrabalhamunicipio.Add(ModelToEntity(pessoaTrabalhaMunicipioModel));
                    return _context.SaveChanges() == 1 ? true : false;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

            return false;
        }

        public bool Update(PessoaTrabalhaMunicipioModel pessoaTrabalhaMunicipioModel)
        {

            _context.Update(ModelToEntity(pessoaTrabalhaMunicipioModel));
            return _context.SaveChanges() == 1 ? true : false;

        }
        private Pessoatrabalhamunicipio ModelToEntity(PessoaTrabalhaMunicipioModel model)
        {
            return new Pessoatrabalhamunicipio
            {
                IdMunicipio = model.IdMunicipio,
                IdPessoa = model.IdPessoa,
                EhResponsavel = Convert.ToByte(model.EhResponsavel),
                EhSecretario = Convert.ToByte(model.EhSecretario),
                SituacaoCadastro = model.SituacaoCadastro
            };
        }
    }
}
