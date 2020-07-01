using System;
using System.Collections.Generic;

namespace Model.ViewModel
{
    public class PesquisaExameViewModel
    {

        public PesquisaExameViewModel()
        {
            Exames = new List<ExameBuscaModel>();
            Negativos = 0;
            Positivos = 0;
			Recuperados = 0;
			IgMIgGs = 0;
			Indeterminados = 0;
			Aguardando = 0;
		}

        public List<ExameBuscaModel> Exames { get; set; }
        public int Negativos { get; set; }
        public int Positivos { get; set; }
        public int Recuperados { get; set; }
        public int Indeterminados { get; set; }
		public int Aguardando { get; set; }
		public int IgMIgGs { get; set; }
		public string Pesquisa { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string Resultado { get; set; }
        public bool RealizouPesquisa { get; set; }
        public string Cidade { get; set; }
    }
}
