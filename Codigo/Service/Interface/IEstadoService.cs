using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IEstadoService
    {
        bool Insert(EstadoModel estadoModel);
        bool Update(EstadoModel estadoModel);
        bool Delete(int id);
        List<EstadoModel> GetAll();
        EstadoModel GetById(int id);
        EstadoModel GetByName(string name);
        EstadoModel GetByUf(string uf);
        EstadoModel GetByCodUf(int codigoUf);

    }
}
