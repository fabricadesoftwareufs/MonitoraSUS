using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IPessoaService
    {
        bool Insert(PessoaModel pessoaModel);
        bool Update(PessoaModel pessoaModel);
        bool Delete(int id);
        List<PessoaModel> GetAll();
        PessoaModel GetById(int id);
    }
}
