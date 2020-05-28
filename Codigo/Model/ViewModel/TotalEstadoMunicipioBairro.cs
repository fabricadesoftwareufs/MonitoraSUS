using System;
using System.ComponentModel.DataAnnotations;

namespace Model.ViewModel
{
    public class TotalEstadoMunicipioBairro
    {
        public String Estado { get; set; }
        [Display(Name = "Município")]
        public string Municipio { get; set; }
        public int IdEmpresaSaude { get; set; }
        public string Bairro { get; set; }
        [Display(Name = "Positivos")]
        public int TotalPositivos { get; set; }
        [Display(Name = "Negativos")]
        public int TotalNegativos { get; set; }
        [Display(Name = "Recuperados")]
        public int TotalRecuperados { get; set; }
		[Display(Name = "Aguardando")]
		public int TotalAguardando { get; set; }
		[Display(Name = "IgG/IgM")]
		public int TotalIgGIgM { get; set; }

		[Display(Name = "Indeterminados")]
        public int TotalIndeterminados { get; set; }
    }
}
