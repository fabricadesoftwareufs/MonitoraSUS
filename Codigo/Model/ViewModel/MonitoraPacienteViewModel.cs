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
            ExamesPaciente = new List<ExameViewModel>();
			Internacoes = new List<InternacaoModel>();

		}
		[Display(Name = "Data do Exame")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime DataExame { get; set; }
		public int IdExame { get; set; }
		public int Idpessoa { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Cep { get; set; }
        public string Rua { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string FoneCelular { get; set; }
        public string FoneFixo { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public bool Hipertenso { get; set; }
        public bool Diabetes { get; set; }
        public bool Obeso { get; set; }
        public bool Cardiopatia { get; set; }
        public bool Imunodeprimido { get; set; }
        public bool Cancer { get; set; }
        public bool DoencaRespiratoria { get; set; }
		[Display(Name = "Doença Renal")]
		public bool DoencaRenal { get; set; }
		public bool Epilepsia { get; set; }
		public string OutrasComorbidades { get; set; }
		public string SituacaoSaude { get; set; }
		[Display(Name = "Data Óbito")]
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime ?DataObito { get; set; }
		public string SituacaoSaudeDescricao
		{
			get
			{
				return new PessoaModel() { SituacaoSaude = this.SituacaoSaude }.SituacaoSaudeDescricao;
			}
		}


		public bool TemFoneCelularValido
        {
            get
            {
				return new PessoaModel() { FoneCelular = this.FoneCelular }.TemFoneCelularValido;
            }
        }

        public VirusBacteriaModel VirusBacteria { get; set; }
        public List<ExameViewModel> ExamesPaciente { get; set; }
		public List<InternacaoModel> Internacoes { get; set; }

		public PessoaModel Gestor { get; set; }
        public string UltimoResultado { get; set; }
        public DateTime? DataUltimoMonitoramento { get; set; }
        public string Descricao { get; set; }
		public bool Febre { get; set; }
		public bool Tosse { get; set; }
		public bool Coriza { get; set; }
		[Display(Name = "Dificuldade Respiratória")]
		public bool DificuldadeRespiratoria { get; set; }
		[Display(Name = "Dor na Garganta")]
		public bool DorGarganta { get; set; }
		[Display(Name = "Diarréia")]
		public bool Diarreia { get; set; }
		[Display(Name = "Dor no Ouvido")]
		public bool DorOuvido { get; set; }
		[Display(Name = "Náusea")]
		public bool Nausea { get; set; }
		[Display(Name = "Dor Abdominal")]
		public bool DorAbdominal { get; set; }
		[Display(Name = "Perda Olfato/Paladar")]
		public bool PerdaOlfatoPaladar { get; set; }

		public string OutrosSintomas { get; set; }
		public string Cns { get; set; }
		public string Profissao { get; set; }
		public int IdAreaAtuacao { get; set; }
	}
}
