using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Util.Util;

namespace Util
{
    public class Methods
    {
        public static string RemoveSpecialsCaracts(string poluatedString) =>  Regex.Replace(poluatedString, @"[^0-9a-zA-Z_]", string.Empty);

        public static string RemoverAcentos(string texto)
        {
            return new string(texto
                .Normalize(NormalizationForm.FormD)
                .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }

        
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

            return Criptography.GenerateHashString(frase.ToString());
        }


        public static string ReturnRole(int userType)
        {
            switch (userType)
            {
                case 0: return "USUARIO";
                case 1: return "AGENTE";
                case 2: return "GESTOR";
                case 3: return "SECRETARIO";
                case 4: return "ADM";
                default: return "UNDEFINED";
            }
        }

        public static int ReturnRoleId(string userType)
        {
            switch (userType.ToUpper())
            {
                case "USUARIO": return 0;
                case "AGENTE": return 1;
                case "GESTOR": return 2;
                case "SECRETARIO": return 3;
                case "ADM": return 4;
                default: return -1;
            }
        }


        /// <summary>
        /// retrnar cpf com padrao de caracteres - mask
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns></returns>
        public static string PatternCpf(string cpf)
        {
            cpf = cpf.Substring(0, 3) + "." + cpf.Substring(3, 3) + "." + cpf.Substring(6, 3) + "-" + cpf.Substring(9, 2);
            return cpf;
        }

		/// <summary>
		/// Remove caracteres não numéricos
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string RemoveNaoNumericos(string text)
		{
			System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"[^0-9]");
			string ret = reg.Replace(text, string.Empty);
			return ret;
		}

		public static bool ValidarCpf(string cpf)
        {
			cpf = RemoveNaoNumericos(cpf);

            if (string.IsNullOrEmpty(cpf))
                return false;

			var multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito += resto.ToString();
            return cpf.EndsWith(digito);
        }

        public static bool ValidarCnpj(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }

        public static bool SoContemNumeros(string texto)
        {
            texto = texto.Replace(".", "").Replace("-", "");
            var value = Regex.IsMatch(texto, "^[0-9]*$");
            return value;
        }

        public static bool SoContemLetras(string texto)
        {
            texto = texto.Replace(".", "").Replace("-", "");
            var value = Regex.IsMatch(texto, @"^([a-zA-Z ])*$");
            return value;
        }

        /// <summary>
        /// Realiza requisição para o google e retorna o valor do captcha.
        /// </summary>
        /// <param name="token">Valor do campo g-recaptcha-response do formulario.</param>
        /// <param name="secretKey">Valor da chave secreta do AppSettings.</param>
        /// <returns></returns>
        public static async Task<float> ValidateCaptcha(string token, string secretKey)
        {
            try
            {
                var cliente = new HttpClient();
                var uri = $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}";

                var resultado = await cliente.GetStringAsync(uri);
                var jsonResponse = JsonConvert.DeserializeObject<RecaptchaModel>(resultado);

                if (jsonResponse.Success)
                    return jsonResponse.Score;
                else
                    return 0;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static async Task<(string lat, string lng)> BuscaLatLong(string address, string googleKey)
        {
            var root = new RootObject();

            var url =
                string.Format(
                    "https://maps.googleapis.com/maps/api/geocode/json?address={0}&sensor=false&key={1}", address, googleKey);
            var req = (HttpWebRequest)WebRequest.Create(url);

            var res = (HttpWebResponse)req.GetResponse();

            using (var streamreader = new StreamReader(res.GetResponseStream()))
            {
                var result = streamreader.ReadToEnd();

                if (!string.IsNullOrWhiteSpace(result))
                {
                    root = JsonConvert.DeserializeObject<RootObject>(result);
                }
            }
            var latitude = Convert.ToString(root.results[0].geometry.location.lat, CultureInfo.InvariantCulture);

            var longitude = Convert.ToString(root.results[0].geometry.location.lng, CultureInfo.InvariantCulture);

            return (latitude, longitude);
        }

    }
}
