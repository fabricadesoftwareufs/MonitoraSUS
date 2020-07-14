using Repository.Interfaces;
using Service.Interface;

namespace Service.UnitiesOfWorks.Interfaces
{
    public interface IPessoaExameSituacaoUnidadeTrabalhoUnityOfWork : ITransactions
    {
        ISituacaoVirusBacteriaService SituacaoPessoaService { get; }
        IPessoaService PessoaService { get; }
        IExameRepository ExameRepository { get; }
    }
}
