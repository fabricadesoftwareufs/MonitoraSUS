using Persistence;
using Repository;
using Repository.Interfaces;
using Service.Interface;
using Service.UnitiesOfWorks.Interfaces;

namespace Service.UnitiesOfWorks
{
    public class ExameSituacaoPessoaUnityOfWork : IExameSituacaoPessoaUnityOfWork, ITransactions
    {
        private readonly monitorasusContext _context;
        public IExameRepository ExameRepositorio { get; }
        public ISituacaoVirusBacteriaService SituacaoPessoaService { get; }

        public ExameSituacaoPessoaUnityOfWork(monitorasusContext context)
        {
            ExameRepositorio = new ExameRepository(context);
            SituacaoPessoaService = new SituacaoVirusBacteriaService(context);
            _context = context;
        }

        public void BeginTransaction() => _context.Database.BeginTransaction();

        public void Commit() => _context.Database.CommitTransaction();

        public void Rollback() => _context.Database.RollbackTransaction();
    }
}
