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
            var situacao = _context.Situacaopessoavirusbacteria.Find(id);
            _context.Situacaopessoavirusbacteria.Remove(situacao);
            return _context.SaveChanges() == 1 ? true : false;
        }

        public List<SituacaoPessoaVirusBacteriaModel> GetAll()
        => _context.Situacaopessoavirusbacteria
                .Select(situacao => new SituacaoPessoaVirusBacteriaModel
                {

                    Idpessoa = situacao.Idpessoa,
                    IdVirusBacteria = situacao.IdVirusBacteria,
                    UltimaSituacaoSaude = situacao.UltimaSituacaoSaude

                }).ToList();

        public SituacaoPessoaVirusBacteriaModel GetById(int idPessoa,int idVirus)
       => _context.Situacaopessoavirusbacteria
                .Where(situacaoPessoa => situacaoPessoa.IdVirusBacteria == idVirus && situacaoPessoa.Idpessoa == idPessoa)
                .Select(situacao => new SituacaoPessoaVirusBacteriaModel
                {

                    Idpessoa = situacao.Idpessoa,
                    IdVirusBacteria = situacao.IdVirusBacteria,
                    UltimaSituacaoSaude = situacao.UltimaSituacaoSaude

                }).FirstOrDefault();


        public bool Insert(SituacaoPessoaVirusBacteriaModel situacaoModel)
        {
            _context.Add(ModelToEntity(situacaoModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

        private Situacaopessoavirusbacteria ModelToEntity(SituacaoPessoaVirusBacteriaModel situacaoModel)
        {
            Situacaopessoavirusbacteria s = new Situacaopessoavirusbacteria();

            s.IdVirusBacteria = situacaoModel.IdVirusBacteria;
            s.Idpessoa = situacaoModel.Idpessoa;
            s.UltimaSituacaoSaude = situacaoModel.UltimaSituacaoSaude;

            return s;
        }

        public bool Update(SituacaoPessoaVirusBacteriaModel situacaoModel)
        {
            _context.Update(ModelToEntity(situacaoModel));
            return _context.SaveChanges() == 1 ? true : false;
        }
    }
}
