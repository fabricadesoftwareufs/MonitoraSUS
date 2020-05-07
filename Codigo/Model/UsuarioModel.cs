namespace Model
{
    /// <summary>
    /// Tipo de Usuario : 0- comum, 1- agente, 2- gestor, 3 - secretario, 4 - adms
    /// </summary>
    public class UsuarioModel
    {
        public const int PERFIL_COMUM = 0;
        public const int PERFIL_AGENTE = 1;
        public const int PERFIL_GESTOR = 2;
        public const int PERFIL_SECRETARIO = 3;
        public const int PERFIL_ADMS = 4;

        public int IdUsuario { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }
        public int TipoUsuario { get; set; }
        public int IdPessoa { get; set; }
    }
}
