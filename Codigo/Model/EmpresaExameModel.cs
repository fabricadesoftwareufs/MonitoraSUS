using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class EmpresaExameModel
    {
        public const int EMPRESA_ESTADO_MUNICIPIO = 1;
        public const string SITUACAO_CADASTRO_SOLICITADA = "S";

        [Display(Name = "Código")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Cnpj")]
        public string Cnpj { get; set; }
        [Required]
        [Display(Name = "Nome")]
        public string Nome { get; set; }
        [Required]
        [Display(Name = "Cep")]
        public string Cep { get; set; }
        [Required]
        [Display(Name = "Rua")]
        public string Rua { get; set; }
        [Required]
        [Display(Name = "Bairro")]
        public string Bairro { get; set; }
        [Required]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public string Estado { get; set; }
        [Display(Name = "Numero")]
        public string Numero { get; set; }
        [Display(Name = "Complemento")]
        public string Complemento { get; set; }
        [Required]
        public string Latitude { get; set; }
        [Required]
        public string Longitude { get; set; }
        [Required]
        [Display(Name = "Telefone Celular")]
        public string FoneCelular { get; set; }
        [Display(Name = "Telefone Fixo")]
        public string FoneFixo { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Emite Laudo de Exames ? ")]
        public bool EmiteLaudoExame { get; set; }
        [Required]
        [Display(Name = "Possui Leitos de Internação ? ")]
        public bool PossuiLeitosInternacao { get; set; }
        [Required]
        [Display(Name = "Número de Leitos")]
        public int NumeroLeitos { get; set; }
        [Required]
        [Display(Name = "Número de Leitos de UTI")]
        public int NumeroLeitosUti { get; set; }
        [Required]
        [Display(Name = "Possui Leitos Disponível")]
        public int NumeroLeitosDisponivel { get; set; }
        [Required]
        [Display(Name = "Número de Leitos de UTI Disponível")]
        public int NumeroLeitosUtidisponivel { get; set; }
		[Required]
		[Display(Name = "É Instituição Pública?")]
		public bool EhPublico { get; set; }
		[Required]
		[Display(Name = "Faz monitoramento de Pacientes?")]
		public bool FazMonitoramento { get; set; }
		[Display(Name = "CNES")]
		public string Cnes { get; set; }
	}
}
