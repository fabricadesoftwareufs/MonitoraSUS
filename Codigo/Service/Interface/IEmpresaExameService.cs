using Model;
using System;
using System.Collections.Generic;
using System.Text;

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
