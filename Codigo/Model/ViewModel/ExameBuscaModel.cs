using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class ExameBuscaModel
    {
		public ExameBuscaModel()
		{
			Exame = new ExameModel();
		}
		public string NomePaciente { get; set; }
		public string Cpf { get; set; }
		public string Cns { get; set; }
		public string NomeVirusBateria { get; set; }
		public string Cidade { get; set; }
		public string Estado { get; set; }
		public string ResponsavelExame { get; set; }
		public ExameModel Exame { get; set; }
	}
}
