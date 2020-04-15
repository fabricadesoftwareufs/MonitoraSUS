using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IPessoaTrabalhaMunicipioService
    {
        bool Insert(PessoaTrabalhaMunicipioModel pessoaTrabalhaMunicipioModel);
        bool Update(PessoaTrabalhaMunicipioModel pessoaTrabalhaMunicipioModel);
        bool Delete(int id);
        List<PessoaTrabalhaMunicipioModel> GetAll();
        List<PessoaTrabalhaMunicipioModel> GetAllSecretariesPendents();
        PessoaTrabalhaMunicipioModel GetById(int id);
    }
}
