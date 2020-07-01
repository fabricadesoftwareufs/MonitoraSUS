using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IMunicipioService
    {
        bool Insert(MunicipioModel municipioModel);
        bool Update(MunicipioModel municipioModel);
        bool Delete(int id);
        List<MunicipioModel> GetAll();
        List<MunicipioModel> GetByUFCode(string UFCode);
        MunicipioModel GetByIBGECode(int code);
        MunicipioModel GetById(int id);
        MunicipioModel GetByName(string name);
    }
}
