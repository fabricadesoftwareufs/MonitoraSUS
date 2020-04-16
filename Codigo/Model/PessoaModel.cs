using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class PessoaModel
    {
        public int Idpessoa { get; set; }
        [Required]
        [Display(Name = "CPF")]
        [StringLength(11)]
        public string Cpf { get; set; }
        [Required]
        [Display(Name = "Nome")]
        [StringLength(100)]
        public string Nome { get; set; }
        [Required]
        [Display(Name = "Sexo")]
        public string Sexo { get; set; }
        public string Cep { get; set; }
        public string Rua { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string FoneCelular { get; set; }
        public string FoneFixo { get; set; }
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataNascimento { get; set; }
        public bool Hipertenso { get; set; }
        public bool Diabetes { get; set; }
        public bool Obeso { get; set; }
        public bool Cardiopatia { get; set; }
        public bool Imunodeprimido { get; set; }
        public bool Cancer { get; set; }
        public bool DoencaRespiratoria { get; set; }
    }
}
