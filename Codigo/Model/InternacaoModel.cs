using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
	public class InternacaoModel
	{
	    public const string O2_VENTILACAO = "V";
		public const string O2_CATETER = "C";
		public const string O2_MASCARA = "M";
		public const string O2_AMBIENTE = "A";

		public int IdInternacao {get; set;}
		public int IdPessoa { get; set; }
		public int IdEmpresa { get; set; }
		public string NomeEmpresa { get; set; }
		public DateTime DataInicio { get; set; }
		public DateTime? DataFim { get; set; }
		public string UsoO2 { get; set; }
		public string UsoO2Descricao {
			get {
				if (UsoO2.Equals(O2_VENTILACAO))
					return "Vetilação Mecânica";
				else if (UsoO2.Equals(O2_CATETER))
					return "Cateter";
				else if (UsoO2.Equals(O2_MASCARA))
					return "Máscara Venturi/Hudson";
				else
					return "Ar Ambiente";
			}
		}

	}
}
