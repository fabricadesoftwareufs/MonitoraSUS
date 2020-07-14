using Repository.Interfaces;
using Service.Interface;

namespace Service.UnitiesOfWorks.Interfaces
{
    public interface IPessoaVirusEmpresaSituacaoMunicipioGeoTrabalhaMuniEstadoUnityOfWork : ITransactions
    {
        IPessoaTrabalhaMunicipioService PessoaTrabalhaMunicipioService { get; }
        IPessoaTrabalhaEstadoService PessoaTrabalhaEstadoContext { get; }
        IMunicipioGeoService MunicipioGeoService { get; }
        IVirusBacteriaService VirusBacteriaService { get; }
        IPessoaService PessoaService { get; }
        IEmpresaExameService EmpresaExameService { get; }
        ISituacaoVirusBacteriaService SituacaoPessoaService { get; }
        IMunicipioService MunicipioService { get; }
        IExameRepository ExameRepository { get; }
    }
}
