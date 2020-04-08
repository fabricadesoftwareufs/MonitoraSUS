using System;
using System.Collections.Generic;

namespace Persistence
{
    public partial class Pessoatrabalhamunicipio
    {
        public int IdPessoa { get; set; }
        public int IdMunicipio { get; set; }
        public byte EhResponsavel { get; set; }
        public byte EhSecretario { get; set; }
        public string SituacaoCadastro { get; set; }

        public Municipio IdMunicipioNavigation { get; set; }
        public Pessoa IdPessoaNavigation { get; set; }
    }
}
