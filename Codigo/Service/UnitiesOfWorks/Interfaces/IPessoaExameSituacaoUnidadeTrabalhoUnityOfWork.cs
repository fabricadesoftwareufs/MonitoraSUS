using Repository.Interfaces;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.UnitiesOfWorks.Interfaces
{
    public interface IPessoaExameSituacaoUnidadeTrabalhoUnityOfWork : ITransactions
    {
        ISituacaoVirusBacteriaService SituacaoPessoaService { get; }
        IPessoaService PessoaService { get; }
        IExameRepository ExameRepository { get; }
    }
}
