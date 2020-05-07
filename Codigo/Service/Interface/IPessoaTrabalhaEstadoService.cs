using Model;
using Model.ViewModel;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IPessoaTrabalhaEstadoService
    {
        bool Insert(PessoaTrabalhaEstadoModel pessoaTrabalhaEstadoModel);
        bool Update(PessoaTrabalhaEstadoModel pessoaTrabalhaEstadoModel);
        bool Delete(int idPessoa);
        List<PessoaTrabalhaEstadoModel> GetAll();
        List<SolicitanteAprovacaoViewModel> GetAllGestores();
        List<SolicitanteAprovacaoViewModel> GetAllGestoresEstado(int idEstado);
        List<SolicitanteAprovacaoViewModel> GetAllGestoresEmpresa(int idEmpresa);
        List<SolicitanteAprovacaoViewModel> GetAllNotificadores();
        List<SolicitanteAprovacaoViewModel> GetAllNotificadoresEstado(int idEstado);
        List<SolicitanteAprovacaoViewModel> GetAllNotificadoresEmpresa(int idEmpresa);
        PessoaTrabalhaEstadoModel GetByIdPessoa(int idPessoa);
        List<PessoaTrabalhaEstadoModel> GetByIdEmpresa(int idEmpresa);
    }
}
