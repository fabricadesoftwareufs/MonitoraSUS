using System.Collections.Generic;

namespace Model.ViewModel
{
    public class TotalizadoresExameViewModel
    {

        public TotalizadoresExameViewModel()
        {
            Exames = new List<ExameViewModel>();
            Negativos = 0;
            Positivos = 0;
            Imunizados = 0;
            Indeterminados = 0;

        }

        public List<ExameViewModel> Exames { get; set; }
        public int Negativos;
        public int Positivos;
        public int Imunizados;
        public int Indeterminados;
    }
}
