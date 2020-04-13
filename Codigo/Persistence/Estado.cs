using System;
using System.Collections.Generic;

namespace Persistence
{
    public partial class Estado
    {
        public Estado()
        {
            Exame = new HashSet<Exame>();
            Pessoatrabalhaestado = new HashSet<Pessoatrabalhaestado>();
        }

        public int Id { get; set; }
        public int CodigoUf { get; set; }
        public string Nome { get; set; }
        public string Uf { get; set; }
        public int Regiao { get; set; }

        public ICollection<Exame> Exame { get; set; }
        public ICollection<Pessoatrabalhaestado> Pessoatrabalhaestado { get; set; }
    }
}
