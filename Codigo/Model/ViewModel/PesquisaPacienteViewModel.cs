using System;
using System.Collections.Generic;

namespace Model.ViewModel
{
    public class PesquisaPacienteViewModel
    {

        public PesquisaPacienteViewModel() 
        {
            Exames = new List<MonitoraPacienteViewModel>();
			Positivos = 0;
			Recuperados = 0;
			Aguardando = 0;
			IgGIgM = 0;
			Indeterminados = 0;
			Isolamento = 0;
			Hospitalizado = 0;
			UTI = 0;
			Obito = 0;
			Saudavel = 0;
			Estabilizacao = 0;
		}

        public List<MonitoraPacienteViewModel> Exames { get; set; }
		public int Isolamento { get; set; }
		public int Hospitalizado { get; set; }
		public int Estabilizacao { get; set; }
		public int UTI { get; set; }
		public int Obito { get; set; }
		public int Saudavel { get; set; }
		public int Positivos { get; set; }
        public int Recuperados { get; set; }
        public int Indeterminados { get; set; }
		public int Aguardando { get; set; }
		public int IgGIgM { get; set; }
		public string Pesquisa { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int VirusBacteria { get; set; }
        public string Resultado { get; set; }
    }
}
