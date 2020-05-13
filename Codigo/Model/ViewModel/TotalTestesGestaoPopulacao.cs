using System.Collections.Generic;

namespace Model.ViewModel
{
    public class TotalTestesGestaoPopulacao
    {
        public int TotalGestao
        {
            get
            {
                return TotalGestaoIndeterminados + TotalGestaoNegativos + TotalGestaoPositivos + TotalGestaoImunizados;
            }
        }
        public int TotalGestaoPositivos { get; set; }
        public int TotalGestaoNegativos { get; set; }
        public int TotalGestaoImunizados { get; set; }
        public int TotalGestaoIndeterminados { get; set; }
        public int TotalPopulacao
        {
            get
            {
                return TotalPopulacaoPositivos + TotalPopulacaoNegativos + TotalPopulacaoIndeterminados + TotalPopulacaoImunizados;
            }
        }
        public int TotalPopulacaoPositivos { get; set; }
        public int TotalPopulacaoNegativos { get; set; }
        public int TotalPopulacaoImunizados { get; set; }
        public int TotalPopulacaoIndeterminados { get; set; }

        public List<TotalEstadoMunicipioBairro> Gestao { get; set; }
        public List<TotalEstadoMunicipioBairro> Populacao { get; set; }
    }
}
