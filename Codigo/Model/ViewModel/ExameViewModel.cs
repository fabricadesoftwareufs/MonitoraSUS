using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class ExameViewModel
    {
		public ExameViewModel()
		{
			VirusBacteria = new VirusBacteriaModel();
			Paciente = new PessoaModel();
			AreaAtuacao = new AreaAtuacaoModel();
			Exame = new ExameModel()
			{
				IgM = "N",
				IgG = "N",
				Pcr = "N",
				IgGIgM = "N",
				DataExame = DateTime.Now,
				DataInicioSintomas = DateTime.Now,
				AguardandoResultado = false,
				MetodoExame = "F",
				StatusNotificacao = ExameModel.NOTIFICADO_NAO
			};
			PesquisarCpf = 0;
		}
		public AreaAtuacaoModel AreaAtuacao { get; set; }
		public VirusBacteriaModel VirusBacteria { get; set; }
        public PessoaModel Paciente { get; set; }
		public ExameModel Exame { get; set; }
		public int PesquisarCpf { get; set; }
		
	}
}
