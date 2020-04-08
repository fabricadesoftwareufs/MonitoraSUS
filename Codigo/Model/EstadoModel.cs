using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class EstadoModel
    {
        public int Id { get; set; }
        public int CodigoUf { get; set; }
        public string Nome { get; set; }
        public string Uf { get; set; }
        public int Regiao { get; set; }
    }
}
