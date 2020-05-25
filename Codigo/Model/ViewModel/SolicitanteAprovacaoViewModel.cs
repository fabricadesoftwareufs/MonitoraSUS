using System.ComponentModel.DataAnnotations;

namespace Model.ViewModel
{
    public class SolicitanteAprovacaoViewModel
    {
        public int IdPessoa { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Estado { get; set; }
        [Display(Name = "Cidade/Empresa")]
        public string Cidade { get; set; }
        public string Status { get; set; }
        public bool EhSecretario { get; set; }
		public int IdEmpresa { get; set; }
		[Display(Name = "Fone Celular")]
		public string FoneCelular { get; set; }

		public string StatusDescricao
        {
            get
            {
                switch (Status)
                {
                    case "I": return "Bloqueado";
                    case "A": return "Ativo";
                    case "S": return "Pendente";
                    default: return "Undefined";
                }
            }
        }
    }
}