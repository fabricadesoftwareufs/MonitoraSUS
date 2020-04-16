using Model;
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

        public bool Delete(int id)
        {
            throw new NotImplementedException();
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

        public List<PessoaTrabalhaMunicipioModel> GetAllSecretariesPendents()
            => _context
                .Pessoatrabalhamunicipio
                .Where(p => p.EhSecretario.Equals(1) && p.SituacaoCadastro.Contains('S'))
                .Select(p => new PessoaTrabalhaMunicipioModel
                {
                    IdPessoa = p.IdPessoa,
                    IdMunicipio = p.IdMunicipio,
                    EhResponsavel = Convert.ToBoolean(p.EhResponsavel),
                    EhSecretario = Convert.ToBoolean(p.EhSecretario),
                    SituacaoCadastro = p.SituacaoCadastro
                }).ToList();

        public PessoaTrabalhaMunicipioModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public bool Insert(PessoaTrabalhaMunicipioModel pessoaTrabalhaMunicipioModel)
        {
            if (pessoaTrabalhaMunicipioModel != null)
            {
                try
                {
                    _context.Pessoatrabalhamunicipio.Add(ModelToEntity(pessoaTrabalhaMunicipioModel, new Pessoatrabalhamunicipio()));
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
            throw new NotImplementedException();
        }

        private Pessoatrabalhamunicipio ModelToEntity(PessoaTrabalhaMunicipioModel model, Pessoatrabalhamunicipio entity)
        {
            entity.IdMunicipio = model.IdMunicipio;
            entity.IdPessoa = model.IdPessoa;
            entity.EhResponsavel = Convert.ToByte(model.EhResponsavel);
            entity.EhSecretario = Convert.ToByte(model.EhSecretario);
            entity.SituacaoCadastro = model.SituacaoCadastro;

            return entity;
        }
    }
}
