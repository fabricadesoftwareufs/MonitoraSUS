using System;
using System.Collections.Generic;
using System.Text;

namespace Model.ViewModel
{
	public class TotalPorResultadoExame
	{
		public String Estado { get; set; }
		public string Municipio { get; set; }
		public int IdEmpresaSaude { get; set; }
		public string Bairro { get; set; }
		public string Resultado { get; set;  }
		public int Total { get; set;  }
	}
}
