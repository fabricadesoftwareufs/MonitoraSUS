using System;
using System.Collections.Generic;

namespace Persistence
{
    public partial class Internacao
    {
        public int IdInternacao { get; set; }
        public int Idpessoa { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string UsoO2 { get; set; }

        public Empresaexame IdEmpresaNavigation { get; set; }
        public Pessoa IdpessoaNavigation { get; set; }
    }
}
