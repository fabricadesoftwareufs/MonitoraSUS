using Model;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Service
{
    public class InternacaoService : IInternacaoService
    {

        private readonly monitorasusContext _context;
        public InternacaoService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Delete(int idInternacao)
        {
            var internacao = _context.Internacao.Find(idInternacao);
            _context.Internacao.Remove(internacao);
            return _context.SaveChanges() == 1 ? true : false;
        }

        public List<InternacaoModel> GetAll()
        => _context.Internacao
            .Select(internacao => new InternacaoModel
            {
                IdPessoa = internacao.Idpessoa,
                IdInternacao = internacao.IdInternacao,
                IdEmpresa = internacao.IdEmpresa,
                DataInicio = internacao.DataInicio,
                DataFim = internacao.DataFim,
                UsoO2 = internacao.UsoO2
            }).ToList();

        public InternacaoModel GetById(int idInternacao)
        => _context.Internacao
            .Where(internacao => internacao.IdInternacao == idInternacao)
            .Select(internacao => new InternacaoModel
            {
                IdPessoa = internacao.Idpessoa,
                IdInternacao = internacao.IdInternacao,
                IdEmpresa = internacao.IdEmpresa,
                DataInicio = internacao.DataInicio,
                DataFim = internacao.DataFim,
                UsoO2 = internacao.UsoO2
            }).FirstOrDefault();

        public List<InternacaoModel> GetByIdPaciente(int idpaciente)
        => _context.Internacao
            .Where(internacao => internacao.Idpessoa == idpaciente)
            .Select(internacao => new InternacaoModel
            {
                IdPessoa = internacao.Idpessoa,
                IdInternacao = internacao.IdInternacao,
                IdEmpresa = internacao.IdEmpresa,
                DataInicio = internacao.DataInicio,
                DataFim = internacao.DataFim,
                UsoO2 = internacao.UsoO2
            }).ToList();

		public bool Insert(InternacaoModel internacaoModel)
		{
			if (internacaoModel.DataFim == null)
			{
				bool haInternacaoAberta = _context.Internacao.Where(internacao => internacao.DataFim == null && internacao.Idpessoa == internacaoModel.IdPessoa).Count() > 0;
				if (haInternacaoAberta)
					throw new ServiceException("Não é possível adicionar uma nova internação quando existe internações em aberto. Favor colocar data final da internação que encerrou.");
			}
			_context.Add(ModelToEntity(internacaoModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

        public bool update(InternacaoModel internacaoModel)
        {
            _context.Update(ModelToEntity(internacaoModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

        private Internacao ModelToEntity(InternacaoModel internacao)
            => new Internacao
            {
                IdInternacao = internacao.IdInternacao,
                Idpessoa = internacao.IdPessoa,
                IdEmpresa = internacao.IdEmpresa,
                DataInicio = internacao.DataInicio,
                DataFim = internacao.DataFim,
                UsoO2 = internacao.UsoO2
            };
    }
}
