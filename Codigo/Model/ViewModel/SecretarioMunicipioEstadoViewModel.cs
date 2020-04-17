namespace Model.ViewModel
{
    public class SecretarioMunicipioEstadoViewModel
    {
        public PessoaModel Pessoa { get; set; }
        public PessoaTrabalhaMunicipioModel PessoaMunicipio { get; set; }
        public PessoaTrabalhaEstadoModel PessoaEstado { get; set; }
        public int Situacao { get; set; }
    }
}
