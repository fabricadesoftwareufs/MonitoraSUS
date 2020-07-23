using Model;
using Model.ViewModel;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IPessoaService
    {
        PessoaModel Insert(PessoaModel pessoaModel);
        PessoaModel InsertGestor(PessoaViewModel pessoaViewModel);
        PessoaModel InsertAgente(PessoaViewModel pessoaViewModel);
        PessoaModel Update(PessoaModel pessoaModel, bool atualizaSintomas);
        bool Delete(int id);

        List<PessoaModel> GetAll();
        PessoaModel GetById(int id);
        PessoaModel GetByCpf(string cpf);
        PessoaModel GetByCns(string cns);
        List<PessoaModel> GetByCidade(string cidade);
		List<PessoaModel> GetByEstado(string estado);
	}
}
