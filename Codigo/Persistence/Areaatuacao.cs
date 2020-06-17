using System;
using System.Collections.Generic;

namespace Persistence
{
    public partial class Areaatuacao
    {
        public Areaatuacao()
        {
            Exame = new HashSet<Exame>();
            Pessoa = new HashSet<Pessoa>();
        }

        public int IdAreaAtuacao { get; set; }
        public string Descricao { get; set; }

        public ICollection<Exame> Exame { get; set; }
        public ICollection<Pessoa> Pessoa { get; set; }
    }
}
