using System.Collections.Generic;

namespace Model.ViewModel
{
    public class SolicitanteEmpresaViewModel
    {
        public List<SolicitanteAprovacaoViewModel> Solicitantes { get; set; }
        public List<EmpresaExameModel> Empresas { get; set; }
    }
}
