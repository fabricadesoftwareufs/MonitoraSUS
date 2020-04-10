using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        public string Cpf { get; set; }
        [Required]
        public string Senha { get; set; }
    }
}
