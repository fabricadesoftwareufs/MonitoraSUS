using Microsoft.AspNetCore.Http;
using Model.ViewModel;

namespace Service.Interface
{
    public interface IImportarExameService
    {
        void Import(IFormFile file, UsuarioViewModel agente);
    }
}
