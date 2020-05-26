using System.Collections.Generic;

namespace Model.ViewModel
{
    public class TotalTestesGestaoPopulacao
    {
        public int TotalGestao
        {
            get
            {
                return TotalGestaoIndeterminados + TotalGestaoNegativos + TotalGestaoPositivos + TotalGestaoCurados;
            }
        }
        public int TotalGestaoPositivos { get; set; }
        public int TotalGestaoNegativos { get; set; }
        public int TotalGestaoCurados { get; set; }
        public int TotalGestaoIndeterminados { get; set; }
        public int TotalPopulacao
        {
            get
            {
                return TotalPopulacaoPositivos + TotalPopulacaoNegativos + TotalPopulacaoIndeterminados + TotalPopulacaoCurados;
            }
        }
        public int TotalPopulacaoPositivos { get; set; }
        public int TotalPopulacaoNegativos { get; set; }
        public int TotalPopulacaoCurados { get; set; }
        public int TotalPopulacaoIndeterminados { get; set; }

        public List<TotalEstadoMunicipioBairro> Gestao { get; set; }
        public List<TotalEstadoMunicipioBairro> Populacao { get; set; }
    }
}
