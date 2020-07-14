using Model;

namespace Repository.Interfaces
{
    public interface INotificacoesRepository
    {
        ConfiguracaoNotificarModel BuscarConfiguracaoNotificar(int IdEstado, int IdEmpresaExame);
        ConfiguracaoNotificarModel BuscarConfiguracaoNotificar(int IdMunicipio);
    }
}
