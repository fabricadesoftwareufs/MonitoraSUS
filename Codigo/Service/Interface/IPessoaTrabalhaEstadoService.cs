using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IPessoaTrabalhaEstadoService
    {
        bool Insert(PessoaTrabalhaEstadoModel pessoaTrabalhaEstadoModel);
        bool Update(PessoaTrabalhaEstadoModel pessoaTrabalhaEstadoModel);
        bool Delete(int idPessoa, int idEstado);
        List<PessoaTrabalhaEstadoModel> GetAll();
        PessoaTrabalhaEstadoModel GetById(int idPessoa, int idEstado);
        List<PessoaTrabalhaEstadoModel> GetAllSecretariesPendents();
        List<PessoaTrabalhaEstadoModel> GetAllAgents();
        PessoaTrabalhaEstadoModel GetSecretarioAtivoByIdPessoa(int idPessoa);
        PessoaTrabalhaEstadoModel GetByIdPessoa(int idPessoa);
    }
}
