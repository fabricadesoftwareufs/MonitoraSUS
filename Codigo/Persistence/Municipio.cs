using System;
using System.Collections.Generic;

namespace Persistence
{
    public partial class Municipio
    {
        public Municipio()
        {
            Configuracaonotificar = new HashSet<Configuracaonotificar>();
            Exame = new HashSet<Exame>();
            Pessoatrabalhamunicipio = new HashSet<Pessoatrabalhamunicipio>();
        }

        public int Id { get; set; }
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Uf { get; set; }

        public ICollection<Configuracaonotificar> Configuracaonotificar { get; set; }
        public ICollection<Exame> Exame { get; set; }
        public ICollection<Pessoatrabalhamunicipio> Pessoatrabalhamunicipio { get; set; }
    }
}
