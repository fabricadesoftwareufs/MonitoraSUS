namespace Persistence
{
    public partial class Usuario
    {
        public int IdUsuario { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }
        public byte TipoUsuario { get; set; }
        public int IdPessoa { get; set; }

        public Pessoa IdPessoaNavigation { get; set; }
    }
}
