using Model;
using Persistence;

namespace Repository.Interfaces
{
    public interface INotificacoesRepository
    {
        bool Update(Configuracaonotificar configuracao);
        ConfiguracaoNotificarModel BuscarConfiguracaoNotificar(int IdEstado, int IdEmpresaExame);
        ConfiguracaoNotificarModel BuscarConfiguracaoNotificar(int IdMunicipio);
        Configuracaonotificar Configura(ConfiguracaoNotificarModel configuracaoNotificar);
    }
}
