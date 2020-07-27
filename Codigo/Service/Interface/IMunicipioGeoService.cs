using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IMunicipioGeoService
    {
        bool Insert(MunicipioGeoModel municipioModel);
        bool Update(MunicipioGeoModel municipioModel);
        bool Delete(int id);
        List<MunicipioGeoModel> GetAll();
        List<MunicipioGeoModel> GetByUFCode(int codigoUf);
        MunicipioGeoModel GetByIBGECode(int code);
        MunicipioGeoModel GetById(int id);
        MunicipioGeoModel GetByName(string name, int codUf);
    }
}
