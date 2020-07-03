using Model;

namespace Service.Interface
{
    public interface IRecuperarSenhaService
    {
        bool Insert(RecuperarSenhaModel recuperarSenha);
        RecuperarSenhaModel GetByToken(string token);
        void SetTokenInvalid(int idUser);
        void DeleteByUser(int idUser);
        RecuperarSenhaModel GetByUser(int idUser);
        /// <summary>
        /// Checa se o token é valido
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        bool IsTokenValid(string token);

        /// <summary>
        /// Checará se o usuario em questão já possui algum token valido.
        /// </summary>
        /// <param name="idUser">Id do usuario a ser checado.</param>
        /// <returns>True caso o usuario possua 1 token (valido) aberto, false caso não.</returns>
        bool UserNotHasToken(int idUser);

	}
}
