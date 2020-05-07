using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IConfiguracaoNotificarService
    {
        bool Insert(ConfiguracaoNotificarModel configuracaoNotificarModel);
        bool Update(ConfiguracaoNotificarModel configuracaoNotificarModel);
        bool Delete(int id);
        List<ConfiguracaoNotificarModel> GetAll();
        ConfiguracaoNotificarModel GetById(int id);
        ConfiguracaoNotificarModel GetByIdEstado(int idEstado);
        ConfiguracaoNotificarModel GetByIdMunicipio(int idMunicipio);
        ConfiguracaoNotificarModel GetByIdIdEmpresaExame(int idEmpresaExame);
    }
}
