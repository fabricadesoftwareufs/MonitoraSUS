using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.ViewModel
{
    public class PessoaViewModel
    {
        public PessoaViewModel()
        {
            Pessoa = new PessoaModel();
        }
        [Required(ErrorMessage ="CPF requerido")]
        [Util.CPF]
        public string Cpf { get; set; }
        public PessoaModel Pessoa { get; set; }
        public String RecaptchaResponse { get; set; }
        public String AreaAtuacao { get; set; }
        public int IdMunicipio { get; set; }
        public int IdEstado { get; set; }
    }
}
