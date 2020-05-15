using System;

namespace Model
{
    public class SituacaoPessoaVirusBacteriaModel
    {
        public int IdVirusBacteria { get; set; }
        public int Idpessoa { get; set; }
        public string UltimaSituacaoSaude { get; set; }
        public DateTime? DataUltimoMonitoramento { get; set; }
        public string Descricao { get; set; }
        public int? IdGestor { get; set; }
    }
}
