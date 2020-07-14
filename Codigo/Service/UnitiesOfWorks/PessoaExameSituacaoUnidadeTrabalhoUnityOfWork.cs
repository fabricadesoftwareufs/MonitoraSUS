using Persistence;
using Repository;
using Repository.Interfaces;
using Service.Interface;
using Service.UnitiesOfWorks.Interfaces;

namespace Service.UnitiesOfWorks
{
    public class PessoaExameSituacaoUnidadeTrabalhoUnityOfWork : IPessoaExameSituacaoUnidadeTrabalhoUnityOfWork
    {
        private readonly monitorasusContext _context;
        public ISituacaoVirusBacteriaService SituacaoPessoaService { get; }
        public IPessoaService PessoaService { get; }
        public IExameRepository ExameRepository { get; }

        public PessoaExameSituacaoUnidadeTrabalhoUnityOfWork(monitorasusContext context)
        {
            SituacaoPessoaService = new SituacaoVirusBacteriaService(context);
            PessoaService = new PessoaService(context);
            ExameRepository = new ExameRepository(context);

            _context = context;
        }

        public void BeginTransaction() => _context.Database.BeginTransaction();

        public void Commit() => _context.Database.CommitTransaction();

        public void Rollback() => _context.Database.RollbackTransaction();
    }
}
