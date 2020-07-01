using Model;
using Model.ViewModel;
using System.Collections.Generic;
using System.Security.Claims;

namespace Service.Interface
{
    public interface IUsuarioService
    {
        bool Insert(UsuarioModel usuarioModel);
        bool Update(UsuarioModel usuarioModel);
        bool Delete(int id);
        List<UsuarioModel> GetAll();
        UsuarioModel GetById(int id);
        UsuarioModel GetByLogin(string cpf, string senha);
        UsuarioModel GetByIdPessoa(int idPessoa);
        UsuarioModel GetByCpf(string cpf);
		UsuarioViewModel RetornLoggedUser(ClaimsIdentity claimsIdentity);
		string MessageEmail(RecuperarSenhaModel senhaModel, int finalidadeEmail);
	}
}
