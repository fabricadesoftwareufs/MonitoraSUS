using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class UsuarioModel
    {
        public int IdUsuario { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }
        public int TipoUsuario { get; set; }
    }
}
