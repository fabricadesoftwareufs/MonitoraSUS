using Model;
using Persistence;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class AreaAtuacaoService : IAreaAtuacaoService
	{
        private readonly monitorasusContext _context;
        public AreaAtuacaoService(monitorasusContext context)
        {
            _context = context;
        }

        public List<AreaAtuacaoModel> GetAll()
             => _context.Areaatuacao
                .Select(area => new AreaAtuacaoModel
                {
					IdAreaAtuacao = area.IdAreaAtuacao,
					Descricao = area.Descricao
                }).ToList();


        public AreaAtuacaoModel GetById(int id)
        => _context.Areaatuacao
				.Where(area => area.IdAreaAtuacao == id)
				.Select(area => new AreaAtuacaoModel
				{
					IdAreaAtuacao = area.IdAreaAtuacao,
					Descricao = area.Descricao
				}).FirstOrDefault();

    }
}
