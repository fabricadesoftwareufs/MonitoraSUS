using Model;
using Model.ViewModel;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IPessoaTrabalhaMunicipioService
    {
        bool Insert(PessoaTrabalhaMunicipioModel pessoaTrabalhaMunicipioModel);
        bool Update(PessoaTrabalhaMunicipioModel pessoaTrabalhaMunicipioModel);
        bool Delete(int idPessoa);
        List<PessoaTrabalhaMunicipioModel> GetAll();
        List<SolicitanteAprovacaoViewModel> GetAllGestores();
        List<SolicitanteAprovacaoViewModel> GetAllGestoresMunicipio(int idMunicipio);
        List<SolicitanteAprovacaoViewModel> GetAllNotificadores();
        List<SolicitanteAprovacaoViewModel> GetAllNotificadoresMunicipio(int idMunicipio);
        PessoaTrabalhaMunicipioModel GetByIdPessoa(int idPessoa);
    }
}
