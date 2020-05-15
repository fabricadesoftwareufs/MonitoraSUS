using System;

namespace Persistence
{
    public partial class Situacaopessoavirusbacteria
    {
        public int IdVirusBacteria { get; set; }
        public int Idpessoa { get; set; }
        public string UltimaSituacaoSaude { get; set; }
        public DateTime? DataUltimoMonitoramento { get; set; }
        public string Descricao { get; set; }
        public int? IdGestor { get; set; }

        public Pessoa IdGestorNavigation { get; set; }
        public Virusbacteria IdVirusBacteriaNavigation { get; set; }
        public Pessoa IdpessoaNavigation { get; set; }
    }
}
