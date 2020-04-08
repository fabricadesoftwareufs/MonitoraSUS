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
        PessoaTrabalhaMunicipioModel GetById(int id);
    }
}
