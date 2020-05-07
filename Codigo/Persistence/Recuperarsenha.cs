using System;
using System.Collections.Generic;

namespace Persistence
{
    public partial class Recuperarsenha
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime InicioToken { get; set; }
        public DateTime FimToken { get; set; }
        public byte EhValido { get; set; }
        public int IdUsuario { get; set; }

        public Usuario IdUsuarioNavigation { get; set; }
    }
}
