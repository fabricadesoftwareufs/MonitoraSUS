using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IPessoaTrabalhaMunicipioService
    {
        bool Insert(PessoaTrabalhaMunicipioModel pessoaTrabalhaMunicipioModel);
        bool Update(PessoaTrabalhaMunicipioModel pessoaTrabalhaMunicipioModel);
        bool Delete(int idPessoa, int idMunicipio);
        List<PessoaTrabalhaMunicipioModel> GetAll();
        List<PessoaTrabalhaMunicipioModel> GetAllSecretariesPendents();
        List<PessoaTrabalhaMunicipioModel> GetAllAgents();
        PessoaTrabalhaMunicipioModel GetById(int idPessoa, int idMunicipio);
        PessoaTrabalhaMunicipioModel GetByIdPessoa(int idPessoa);
    }
}
