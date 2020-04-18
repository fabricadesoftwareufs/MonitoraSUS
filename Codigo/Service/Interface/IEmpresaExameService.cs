using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IEmpresaExameService
    {
        bool Insert(EmpresaExameModel empresaExameModel);
        bool Update(EmpresaExameModel empresaExameModel);
        bool Delete(int id);
        List<EmpresaExameModel> GetAll();
        EmpresaExameModel GetById(int id);
    }
}
