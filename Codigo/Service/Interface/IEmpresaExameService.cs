using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IEmpresaExameService
    {
        EmpresaExameModel Insert(EmpresaExameModel empresaExameModel);
        EmpresaExameModel Update(EmpresaExameModel empresaExameModel);
        bool Delete(int id);
        List<EmpresaExameModel> GetAll();
		List<EmpresaExameModel> GetHospitais();
		EmpresaExameModel GetById(int id);
        EmpresaExameModel GetByCNES(string cnes);
        List<EmpresaExameModel> GetByCnpj(string cnpj);
        List<EmpresaExameModel> GetByUF(string uf);
        List<EmpresaExameModel> ListByUF(string uf);
        List<EmpresaExameModel> ListAll();
    }
}
