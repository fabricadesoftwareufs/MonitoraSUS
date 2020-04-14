using Model;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class SituacaoVirusBacteriaService : ISituacaoVirusBacteriaService
    {

        private readonly monitorasusContext _context;

        public SituacaoVirusBacteriaService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<SituacaoPessoaVirusBacteriaModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public SituacaoPessoaVirusBacteriaModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public SituacaoPessoaVirusBacteriaModel GetByIdPessoa(int idPessoa)
       => _context.Situacaopessoavirusbacteria
                .Where(situacaoPessoa => situacaoPessoa.Idpessoa == idPessoa)
                .Select(situacao => new SituacaoPessoaVirusBacteriaModel
                {
                    
                    Idpessoa = situacao.Idpessoa,
                    IdVirusBacteria = situacao.IdVirusBacteria,
                    UltimaSituacaoSaude = situacao.UltimaSituacaoSaude

                }).FirstOrDefault();


        public bool Insert(SituacaoPessoaVirusBacteriaModel situacaoPessoaVirusBacteriaModel)
        {
            throw new NotImplementedException();
        }

        public bool Update(SituacaoPessoaVirusBacteriaModel situacaoPessoaVirusBacteriaModel)
        {
            throw new NotImplementedException();
        }
    }
}
