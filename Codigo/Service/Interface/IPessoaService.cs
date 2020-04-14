using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IPessoaService
    {
        PessoaModel Insert(PessoaModel pessoaModel);
        bool Update(PessoaModel pessoaModel);
        bool Delete(int id);
        List<PessoaModel> GetAll();
        PessoaModel GetById(int id);
    }
}
