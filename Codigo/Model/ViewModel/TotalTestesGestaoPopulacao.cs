using System.Collections.Generic;

namespace Model.ViewModel
{
    public class TotalTestesGestaoPopulacao
    {
        public int TotalGestao
        {
            get
            {
                return TotalGestaoIndeterminados + TotalGestaoNegativos + 
					TotalGestaoPositivos + TotalGestaoRecuperados +
					TotalGestaoIgGIgM + TotalGestaoAguardando;
            }
        }
        public int TotalGestaoPositivos { get; set; }
        public int TotalGestaoNegativos { get; set; }
        public int TotalGestaoRecuperados { get; set; }
        public int TotalGestaoIndeterminados { get; set; }
		public int TotalGestaoAguardando { get; set; }
		public int TotalGestaoIgGIgM { get; set; }
		public int TotalPopulacao
        {
            get
            {
                return TotalPopulacaoPositivos + TotalPopulacaoNegativos + 
					TotalPopulacaoIndeterminados + TotalPopulacaoRecuperados +
					TotalPopulacaoAguardando + TotalPopulacaoIgGIGM;
            }
        }
        public int TotalPopulacaoPositivos { get; set; }
        public int TotalPopulacaoNegativos { get; set; }
        public int TotalPopulacaoRecuperados { get; set; }
        public int TotalPopulacaoIndeterminados { get; set; }
		public int TotalPopulacaoAguardando { get; set; }
		public int TotalPopulacaoIgGIGM { get; set; }
		public List<TotalEstadoMunicipioBairro> Gestao { get; set; }
        public List<TotalEstadoMunicipioBairro> Populacao { get; set; }
    }
}
