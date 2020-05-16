using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface ISituacaoVirusBacteriaService
    {
        bool Insert(SituacaoPessoaVirusBacteriaModel situacaoPessoaVirusBacteriaModel);
        bool Update(SituacaoPessoaVirusBacteriaModel situacaoPessoaVirusBacteriaModel);
        bool Delete(int idPessoa, int idVirusBacteria);
        List<SituacaoPessoaVirusBacteriaModel> GetAll();
        SituacaoPessoaVirusBacteriaModel GetById(int idPessoa, int idVirus);
        List<SituacaoPessoaVirusBacteriaModel> GetByIdPaciente(int idPaciente);

    }
}
