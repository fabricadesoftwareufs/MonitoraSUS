using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class MunicipioGeoModel
    {
        public int CodigoIbge { get; set; }
        public string Nome { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public byte Capital { get; set; }
        public int CodigoUf { get; set; }
    }
}
