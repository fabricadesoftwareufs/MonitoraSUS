using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// Tipo de Usuario : 0- comum, 1- agente, 2- secretario
    /// </summary>
    public class UsuarioModel
    {
        public int IdUsuario { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }
        public int TipoUsuario { get; set; }   
        public int IdPessoa { get; set; }
    }
}
