using System;
using System.Collections.Generic;

namespace Persistence
{
    public partial class Empresaexame
    {
        public Empresaexame()
        {
            Configuracaonotificar = new HashSet<Configuracaonotificar>();
            Exame = new HashSet<Exame>();
            Internacao = new HashSet<Internacao>();
            Pessoatrabalhaestado = new HashSet<Pessoatrabalhaestado>();
        }

        public int Id { get; set; }
        public string Cnpj { get; set; }
        public string Nome { get; set; }
        public string Cep { get; set; }
        public string Rua { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string FoneCelular { get; set; }
        public string FoneFixo { get; set; }
        public string Email { get; set; }
        public byte EhPublico { get; set; }
        public byte FazMonitoramento { get; set; }
        public byte EmiteLaudoExame { get; set; }
        public byte PossuiLeitosInternacao { get; set; }
        public int NumeroLeitos { get; set; }
        public int NumeroLeitosUti { get; set; }
        public int NumeroLeitosDisponivel { get; set; }
        public int NumeroLeitosUtidisponivel { get; set; }
        public string Cnes { get; set; }

        public ICollection<Configuracaonotificar> Configuracaonotificar { get; set; }
        public ICollection<Exame> Exame { get; set; }
        public ICollection<Internacao> Internacao { get; set; }
        public ICollection<Pessoatrabalhaestado> Pessoatrabalhaestado { get; set; }
    }
}
