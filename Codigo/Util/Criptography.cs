using System.Security.Cryptography;
using System.Text;

namespace Util
{
    public class Criptography
    {
        /// <summary>
        /// Enum ALGORITMOS - Representa os tipos de algoritmos de hash disponíveis para geração de senhas.
        /// </summary>
        private enum ALGORITMS { SHA1, MD5, SHA256 };

        /// <summary>
        /// Propriedade Algoritmo - Representa o algoritmo de hash específico.
        /// </summary>
        private static HashAlgorithm Algoritm { get; set; }

        /// <summary>
        /// Método AlgoritmoFactory - Cria um algoritmo de hash específico conforme a opção escolhida.
        /// </summary>
        /// <param name="opcao"> Tipo do algoritmo. </param>
        private static void AlgoritmoFactory(ALGORITMS opcao)
        {
            if (opcao == ALGORITMS.MD5)
                Algoritm = MD5.Create();
            else if (opcao == ALGORITMS.SHA1)
                Algoritm = SHA1.Create();
            else
                Algoritm = SHA256.Create();
        }

        /// <summary>
        /// Método GerarHashSenha - Gera um hash específico utilizado para gerar senhas.
        /// </summary>
        /// <param name="strToBeHashed"> Senha em texto plano (sem aplicação de Hash ou Criptografia). </param>
        /// <returns> Hash da senha fornecida. </returns>
        public static string GenerateHashString(string strToBeHashed)
        {
            var hashedString = Encoding.UTF8.GetBytes(strToBeHashed);

            // Aplicação da sequência MD5, SHA256, MD5, SHA256.
            AlgoritmoFactory(ALGORITMS.MD5);
            hashedString = Algoritm.ComputeHash(hashedString);
            AlgoritmoFactory(ALGORITMS.SHA256);
            hashedString = Algoritm.ComputeHash(hashedString);
            AlgoritmoFactory(ALGORITMS.MD5);
            hashedString = Algoritm.ComputeHash(hashedString);
            AlgoritmoFactory(ALGORITMS.SHA256);
            hashedString = Algoritm.ComputeHash(hashedString);

            var stringSenha = new StringBuilder();
            foreach (var caractere in hashedString)
                stringSenha.Append(caractere.ToString("X2"));

            return stringSenha.ToString();
        }
    }
}
