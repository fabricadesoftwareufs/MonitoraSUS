using System;
using System.Collections.Generic;

namespace Persistence
{
    public partial class Pessoatrabalhaestado
    {
        public int Idpessoa { get; set; }
        public int IdEstado { get; set; }
        public int IdEmpresaExame { get; set; }
        public byte EhResponsavel { get; set; }
        public byte EhSecretario { get; set; }
        public string SituacaoCadastro { get; set; }

        public Empresaexame IdEmpresaExameNavigation { get; set; }
        public Estado IdEstadoNavigation { get; set; }
        public Pessoa IdpessoaNavigation { get; set; }
    }
}
