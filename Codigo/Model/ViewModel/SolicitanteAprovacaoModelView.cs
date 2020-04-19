using System;
using System.Collections.Generic;
using System.Text;

namespace Model.ViewModel
{
    public class SolicitanteAprovacaoModelView
    {
        public int IdPessoa { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string Status { get; set; }
        public string Situacao { get; set; }
    }
}
