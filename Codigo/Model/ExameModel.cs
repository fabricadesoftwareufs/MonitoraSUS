using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class ExameModel
    {
        public const string RESULTADO_POSITIVO = "Positivo";
        public const string RESULTADO_NEGATIVO = "Negativo";
        public const string RESULTADO_RECUPERADO = "Recuperado";
        public const string RESULTADO_INDETERMINADO = "Indeterminado";
		public const string RESULTADO_IGMIGG = "IgG/IgM Positivo";
		public const string RESULTADO_AGUARDANDO = "Aguardando";

		public const string NOTIFICADO_SIM = "S";
        public const string NOTIFICADO_NAO = "N";
        public const string NOTIFICADO_ENVIADO = "E";
		public const string NOTIFICADO_PROBLEMAS = "P";

		public const string METODO_PCR = "P";
		public const string METODO_CROMATOGRAFIA = "C";
		public const string METODO_FLUORESCENCIA = "F";

		public const string METODO_PCR_DESCRICAO = "RT-PCR";
		public const string METODO_CROMATOGRAFIA_DESCRICAO = "I.Cromatografia";
		public const string METODO_FLUORESCENCIA_DESCRICAO = "Fluorescência";
		public int IdExame { get; set; }
        [Display(Name = "Virus")]
        public int IdVirusBacteria { get; set; }
        [Display(Name = "Paciente")]
        public int IdPaciente { get; set; }
		public int IdAreaAtuacao { get; set; }
		public int IdAgenteSaude { get; set; }
		[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime DataExame { get; set; }
		[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime DataInicioSintomas { get; set; }
		[Display(Name = "Aguardando Resultado")]
		public bool AguardandoResultado { get; set; }
		[Display(Name = "Método")]
		public string MetodoExame { get; set; }
		public string MetodoExameDescricao
		{
			get
			{
				if (MetodoExame.Equals(METODO_PCR))
					return METODO_PCR_DESCRICAO;
				else if (MetodoExame.Equals(METODO_CROMATOGRAFIA))
					return METODO_CROMATOGRAFIA_DESCRICAO;
				else
					return METODO_FLUORESCENCIA_DESCRICAO;
			}
		}
		public string IgG { get; set; }
		public string IgGIgM { get; set; }
        public string IgM { get; set; }
        public string Pcr { get; set; }
        public int IdEstado { get; set; }
        public int? IdMunicipio { get; set; }
        public int? IdEmpresaSaude { get; set; }
        public string IdNotificacao { get; set; }
        public string CodigoColeta { get; set; }
        public string StatusNotificacao { get; set; }
        public string StatusNotificacaoDescricao
        {
            get
            {
				if (StatusNotificacao.Equals(NOTIFICADO_NAO))
					return "NÃO";
				else if (StatusNotificacao.Equals(NOTIFICADO_SIM))
					return "SIM";
				else if (StatusNotificacao.Equals(NOTIFICADO_ENVIADO))
					return "ENVIADA";
				else
					return "ERRO NO ENVIO";
            }
        }

        public string Resultado
        {
            get
            {
				return CalculaResultadoExame(AguardandoResultado,IgGIgM,IgM,IgG,Pcr);
            }
        }

        public string ResultadoStatus
        {
            get
            {
				if (Resultado.Equals(RESULTADO_AGUARDANDO))
					return "N";
				else if (Resultado.Equals(RESULTADO_POSITIVO))
                    return "P";
                else if (Resultado.Equals(RESULTADO_NEGATIVO))
                    return "N";
                else if (Resultado.Equals(RESULTADO_RECUPERADO))
                    return "C";
                else
                    return "I";
            }
        }
		[Display(Name = "Relatou Sintomas")]
		public bool RelatouSintomas { get; set; }
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

		public static string CalculaResultadoExame(bool aguardandoResultado,string iggIgm,string igm, string igg, string pcr) {

			if (aguardandoResultado)
				return RESULTADO_AGUARDANDO;
			else if (iggIgm.Equals("S"))
				return RESULTADO_IGMIGG;
			else if (igm.Equals("S") || pcr.Equals("S"))
				return RESULTADO_POSITIVO;
			else if (igm.Equals("N") && pcr.Equals("N") && igg.Equals("N") && iggIgm.Equals("N"))
				return RESULTADO_NEGATIVO;
			else if ((igm.Equals("N") && igg.Equals("S")) || (pcr.Equals("N") && igg.Equals("S")))
				return RESULTADO_RECUPERADO;
			else
				return RESULTADO_INDETERMINADO;
		} 
	}
}
