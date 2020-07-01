using Microsoft.EntityFrameworkCore;
using Model;
using Persistence;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class SituacaoVirusBacteriaService : ISituacaoVirusBacteriaService
    {

        private readonly monitorasusContext _context;

        public SituacaoVirusBacteriaService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Delete(int idPessoa, int idVirusBacteria)
        {
            var situacao = ModelToEntity(GetById(idPessoa, idVirusBacteria));
            _context.Situacaopessoavirusbacteria.Remove(situacao);
            return _context.SaveChanges() == 1 ? true : false;
        }

        public List<SituacaoPessoaVirusBacteriaModel> GetAll()
        => _context.Situacaopessoavirusbacteria
                .Select(situacao => new SituacaoPessoaVirusBacteriaModel
                {

                    Idpessoa = situacao.Idpessoa,
                    IdVirusBacteria = situacao.IdVirusBacteria,
                    DataUltimoMonitoramento = situacao.DataUltimoMonitoramento,
                    IdGestor = situacao.IdGestor,
                    Descricao = situacao.Descricao,
                }).ToList();

        public SituacaoPessoaVirusBacteriaModel GetById(int idPessoa, int idVirus)
       => _context.Situacaopessoavirusbacteria
                .Where(situacaoPessoa => situacaoPessoa.IdVirusBacteria == idVirus && situacaoPessoa.Idpessoa == idPessoa)
                .Select(situacao => new SituacaoPessoaVirusBacteriaModel
                {

                    Idpessoa = situacao.Idpessoa,
                    IdVirusBacteria = situacao.IdVirusBacteria,
                    DataUltimoMonitoramento = situacao.DataUltimoMonitoramento,
                    IdGestor = situacao.IdGestor,
                    Descricao = situacao.Descricao,

                }).FirstOrDefault();


        public bool Insert(SituacaoPessoaVirusBacteriaModel situacaoModel)
        {
            var entity = ModelToEntity(situacaoModel);
            _context.Add(entity);
            bool value = _context.SaveChanges() == 1 ? true : false;
            _context.Entry(entity).State = EntityState.Detached;
            return value;
        }

        private Situacaopessoavirusbacteria ModelToEntity(SituacaoPessoaVirusBacteriaModel situacaoModel)
        {
            Situacaopessoavirusbacteria s = new Situacaopessoavirusbacteria();

            s.IdVirusBacteria = situacaoModel.IdVirusBacteria;
            s.Idpessoa = situacaoModel.Idpessoa;
            s.DataUltimoMonitoramento = situacaoModel.DataUltimoMonitoramento;
            s.IdGestor = situacaoModel.IdGestor;
            s.Descricao = situacaoModel.Descricao;

            return s;
        }

        public bool Update(SituacaoPessoaVirusBacteriaModel situacaoModel)
        {
            var entity = ModelToEntity(situacaoModel);
            _context.Update(entity);
            bool value = _context.SaveChanges() == 1 ? true : false;
            _context.Entry(entity).State = EntityState.Detached;
            return value;
        }

        public List<SituacaoPessoaVirusBacteriaModel> GetByIdPaciente(int idPaciente)
         => _context.Situacaopessoavirusbacteria
          .Where(situacaoPessoa => situacaoPessoa.Idpessoa == idPaciente)
          .Select(situacao => new SituacaoPessoaVirusBacteriaModel
          {

              Idpessoa = situacao.Idpessoa,
              IdVirusBacteria = situacao.IdVirusBacteria,
              DataUltimoMonitoramento = situacao.DataUltimoMonitoramento,
              IdGestor = situacao.IdGestor,
              Descricao = situacao.Descricao,
          }).ToList();
    }
}
