using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.ViewModel
{
    public class MonitoraPacienteViewModel
    {

        public MonitoraPacienteViewModel() 
        {
            ExamesPaciente = new List<ExameBuscaModel>();
			Internacoes = new List<InternacaoModel>();

		}
		[Display(Name = "Data do Exame")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime DataExame { get; set; }
		public int IdExame { get; set; }
		public PessoaModel Paciente { get; set; }
        public VirusBacteriaModel VirusBacteria { get; set; }
        public List<ExameBuscaModel> ExamesPaciente { get; set; }
		public List<InternacaoModel> Internacoes { get; set; }
		public PessoaModel Gestor { get; set; }
        public string UltimoResultado { get; set; }
        public DateTime? DataUltimoMonitoramento { get; set; }
        public string Descricao { get; set; }
	}
}
