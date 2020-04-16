namespace Model
{
    public class PessoaTrabalhaMunicipioModel
    {
        public int IdPessoa { get; set; }
        public int IdMunicipio { get; set; }
        public bool EhResponsavel { get; set; }
        public bool EhSecretario { get; set; }
        public string SituacaoCadastro { get; set; }
    }
}
