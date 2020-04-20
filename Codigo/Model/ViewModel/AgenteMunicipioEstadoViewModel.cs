namespace Model.ViewModel
{
    public class AgenteMunicipioEstadoViewModel
    {
        public PessoaModel Pessoa { get; set; }
        public PessoaTrabalhaMunicipioModel PessoaMunicipio { get; set; }
        public PessoaTrabalhaEstadoModel PessoaEstado { get; set; }
        public string Situacao { get; set; }
    }
}
