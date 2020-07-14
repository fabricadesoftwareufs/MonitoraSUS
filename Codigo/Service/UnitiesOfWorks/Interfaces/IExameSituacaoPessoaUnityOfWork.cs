using Repository.Interfaces;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.UnitiesOfWorks.Interfaces
{
    public interface IExameSituacaoPessoaUnityOfWork : ITransactions
    {
        IExameRepository ExameRepositorio { get; }
        ISituacaoVirusBacteriaService SituacaoPessoaService { get; }
    }
}
