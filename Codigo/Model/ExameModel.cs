using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class ExameModel
    {
		public const string RESULTADO_POSITIVO = "Positivo";
		public const string RESULTADO_NEGATIVO = "Negativo";
		public const string RESULTADO_IMUNIZADO = "Imunizado";
		public const string RESULTADO_INDETERMINADO = "Indeterminado";

		public int IdExame { get; set; }
        public int IdVirusBacteria { get; set; }
        public int IdPaciente { get; set; }
        public int IdAgenteSaude { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataExame { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataInicioSintomas { get; set; }
        public string IgG { get; set; }
        public string IgM { get; set; }
        public string Pcr { get; set; }
        public int IdEstado { get; set; }
        public int? IdMunicipio { get; set; }
        public int? IdEmpresaSaude { get; set; }

		public string Resultado
		{
			get
			{
				if (IgM.Equals("S") || Pcr.Equals("S"))
					return RESULTADO_POSITIVO;
				else if (IgM.Equals("N") && Pcr.Equals("N") && IgG.Equals("N"))
					return RESULTADO_NEGATIVO;
				else if ((IgM.Equals("N") && IgG.Equals("S")) || (Pcr.Equals("N") && IgG.Equals("S")))
						return RESULTADO_IMUNIZADO;
				else
					return RESULTADO_INDETERMINADO;
			}
		}


	}
}
