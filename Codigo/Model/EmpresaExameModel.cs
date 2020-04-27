using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class EmpresaExameModel
    {
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
        [Display(Name = "Email")]
        public string Email { get; set; }
        public byte EmiteLaudoExame { get; set; }
        public byte PossuiLeitosInternacao { get; set; }
        public int NumeroLeitos { get; set; }
        public int NumeroLeitosUti { get; set; }
        public int NumeroLeitosDisponivel { get; set; }
        public int NumeroLeitosUtidisponivel { get; set; }
    }
}
