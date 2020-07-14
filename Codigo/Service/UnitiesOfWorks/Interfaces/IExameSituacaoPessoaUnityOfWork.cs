using Repository.Interfaces;
using Service.Interface;

namespace Service.UnitiesOfWorks.Interfaces
{
    public interface IExameSituacaoPessoaUnityOfWork : ITransactions
    {
        IExameRepository ExameRepositorio { get; }
        ISituacaoVirusBacteriaService SituacaoPessoaService { get; }
    }
}
