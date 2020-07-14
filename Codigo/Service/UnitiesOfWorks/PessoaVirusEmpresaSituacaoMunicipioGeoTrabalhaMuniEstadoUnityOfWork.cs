using Persistence;
using Repository;
using Repository.Interfaces;
using Service.Interface;
using Service.UnitiesOfWorks.Interfaces;

namespace Service.UnitiesOfWorks
{
    public class PessoaVirusEmpresaSituacaoMunicipioGeoTrabalhaMuniEstadoUnityOfWork : IPessoaVirusEmpresaSituacaoMunicipioGeoTrabalhaMuniEstadoUnityOfWork
    {
        private readonly monitorasusContext _context;
        public IPessoaTrabalhaMunicipioService PessoaTrabalhaMunicipioService { get; }
        public IPessoaTrabalhaEstadoService PessoaTrabalhaEstadoContext { get; }
        public IMunicipioGeoService MunicipioGeoService { get; }
        public IVirusBacteriaService VirusBacteriaService { get; }
        public IPessoaService PessoaService { get; }
        public IEmpresaExameService EmpresaExameService { get; }
        public ISituacaoVirusBacteriaService SituacaoPessoaService { get; }
        public IMunicipioService MunicipioService { get; }
        public IExameRepository ExameRepository { get; }

        public PessoaVirusEmpresaSituacaoMunicipioGeoTrabalhaMuniEstadoUnityOfWork(monitorasusContext context)
        {
            PessoaTrabalhaMunicipioService = new PessoaTrabalhaMunicipioService(context);
            PessoaTrabalhaEstadoContext = new PessoaTrabalhaEstadoService(context);
            MunicipioGeoService = new MunicipioGeoService(context);
            VirusBacteriaService = new VirusBacteriaService(context);
            PessoaService = new PessoaService(context);
            EmpresaExameService = new EmpresaExameService(context);
            SituacaoPessoaService = new SituacaoVirusBacteriaService(context);
            MunicipioService = new MunicipioService(context);
            ExameRepository = new ExameRepository(context);

            _context = context;
        }

        public void BeginTransaction() => _context.Database.BeginTransaction();

        public void Commit() => _context.Database.CommitTransaction();

        public void Rollback() => _context.Database.RollbackTransaction();
    }
}
