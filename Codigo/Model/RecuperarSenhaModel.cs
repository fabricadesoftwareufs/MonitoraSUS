using System;

namespace Model
{
    public class RecuperarSenhaModel
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime InicioToken { get; set; }
        public DateTime FimToken { get; set; }
        public byte EhValido { get; set; }
        public int IdUsuario { get; set; }
    }
}
