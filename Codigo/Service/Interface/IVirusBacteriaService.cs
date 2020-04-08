using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IVirusBacteriaService
    {
        bool Insert(VirusBacteriaModel virusBacteriaModel);
        bool Update(VirusBacteriaModel virusBacteriaModel);
        bool Delete(int id);
        List<VirusBacteriaModel> GetAll();
        VirusBacteriaModel GetById(int id);
    }
}
