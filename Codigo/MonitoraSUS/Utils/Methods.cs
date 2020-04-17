using Model;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace MonitoraSUS.Utils
{
    public class Methods
    {
        public static string RemoveSpecialsCaracts(string poluatedString) => Regex.Replace(poluatedString, "[^0-9a-zA-Z]+", "");

        public static string ReturnRole(int userType)
        {
            switch (userType)
            {
                case 0: return "USUARIO";
                case 1: return "AGENTE";
                case 2: return "COORDENADOR";
                case 3: return "SECRETARIO";
                case 4: return "ADM";
                default: return "UNDEFINED";
            }
        }
        
        /// <summary>
        /// Recebe o Usuario da sessão em questão e retorna os dados do mesmo em um objeto usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UsuarioViewModel RetornLoggedUser(ClaimsIdentity claimsIdentity)
        {
            var usuario = new UsuarioModel
                 {
                     IdUsuario = int.Parse(claimsIdentity.Claims.Where(s => s.Type == ClaimTypes.SerialNumber).Select(s => s.Value).FirstOrDefault()),
                     Cpf = claimsIdentity.Claims.Where(s => s.Type == ClaimTypes.UserData).Select(s => s.Value).FirstOrDefault(),
                     Email = claimsIdentity.Claims.Where(s => s.Type == ClaimTypes.Email).Select(s => s.Value).FirstOrDefault(),
                     IdPessoa = Convert.ToInt32(claimsIdentity.Claims.Where(s => s.Type == ClaimTypes.NameIdentifier).Select(s => s.Value).FirstOrDefault())
                 };

            var usuarioViewModel = new UsuarioViewModel
            {
                usuarioModel = usuario,
                RoleUsuario = claimsIdentity.Claims.Where(s => s.Type == ClaimTypes.Role).Select(s => s.Value).FirstOrDefault()
            };

            return usuarioViewModel;
        }          
    }
}
