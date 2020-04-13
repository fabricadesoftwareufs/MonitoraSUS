using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface ISituacaoVirusBacteriaService
    {
        bool Insert(SituacaoPessoaVirusBacteriaModel situacaoPessoaVirusBacteriaModel);
        bool Update(SituacaoPessoaVirusBacteriaModel situacaoPessoaVirusBacteriaModel);
        bool Delete(int id);
        List<SituacaoPessoaVirusBacteriaModel> GetAll();
        SituacaoPessoaVirusBacteriaModel GetById(int id);
        SituacaoPessoaVirusBacteriaModel GetByIdPessoa(int idPessoa);

    }
}
