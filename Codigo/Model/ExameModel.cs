using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class ExameModel
    {
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
					return "Positivo";
				else if (IgM.Equals("N") && Pcr.Equals("N") && IgG.Equals("N"))
					return "Negativo";
				else if ((IgM.Equals("N") && IgG.Equals("S")) || (Pcr.Equals("N") && IgG.Equals("S")))
						return "Imunizado";
				else
					return "Indeterminado";
			}
		}


	}
}
