using System;
using System.Collections.Generic;

namespace Persistence
{
    public partial class Virusbacteria
    {
        public Virusbacteria()
        {
            Exame = new HashSet<Exame>();
            Situacaopessoavirusbacteria = new HashSet<Situacaopessoavirusbacteria>();
        }

        public int IdVirusBacteria { get; set; }
        public string Nome { get; set; }
        public int DiasRecuperacao { get; set; }

        public ICollection<Exame> Exame { get; set; }
        public ICollection<Situacaopessoavirusbacteria> Situacaopessoavirusbacteria { get; set; }
    }
}
