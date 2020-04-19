using Model;
using System;
using System.Text;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;


namespace MonitoraSUS.Utils
{
    public class Methods
    {
        public static string RemoveSpecialsCaracts(string poluatedString) => Regex.Replace(poluatedString, "[^0-9a-zA-Z]+", "");


        public static string GenerateToken()
        {
            var frase = new StringBuilder();
            var random = new Random();
            int length = random.Next(30, 99);
            char letra;

            for (int i = 0; i < length; i++)
            {
                var flt = random.NextDouble();
                var shift = Convert.ToInt32(Math.Floor(length * flt));
                letra = Convert.ToChar(shift + length);
                frase.Append(letra);
            }

            return Criptografia.GerarHash(frase.ToString());
        }

        public static string MessageEmail(RecuperarSenhaModel senhaModel, int finalidadeEmail = 0)
        {
            return "<html><body>" +
                "Foi solicitado uma recuperação de senha, clique no link abaixo para iniciar o processo de recuperação.<br>" +
                "<a href='https://localhost:5001/Login/RecuperarSenha/" +
                senhaModel.Token +
                "'>Clique aqui mudar a senha</a>";
        }

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
                     IdPessoa = int.Parse(claimsIdentity.Claims.Where(s => s.Type == ClaimTypes.NameIdentifier).Select(s => s.Value).FirstOrDefault())
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
