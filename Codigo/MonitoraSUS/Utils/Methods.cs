using Model;
using System;
using System.Text;
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
    }
}
