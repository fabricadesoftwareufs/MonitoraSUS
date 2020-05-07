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
        [Display(Name = "Imunizados")]
        public int TotalImunizados { get; set; }
        [Display(Name = "Indeterminados")]
        public int TotalIndeterminados { get; set; }
    }
}
