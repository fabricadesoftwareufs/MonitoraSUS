using System;
using System.Collections.Generic;

namespace Persistence
{
    public partial class Municipiosgeo
    {
        public int CodigoIbge { get; set; }
        public string Nome { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public byte Capital { get; set; }
        public int CodigoUf { get; set; }
    }
}
