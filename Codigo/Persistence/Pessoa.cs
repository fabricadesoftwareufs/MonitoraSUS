using System;
using System.Collections.Generic;

namespace Persistence
{
    public partial class Pessoa
    {
        public Pessoa()
        {
            ExameIdAgenteSaudeNavigation = new HashSet<Exame>();
            ExameIdPacienteNavigation = new HashSet<Exame>();
            Pessoatrabalhaestado = new HashSet<Pessoatrabalhaestado>();
            Pessoatrabalhamunicipio = new HashSet<Pessoatrabalhamunicipio>();
            SituacaopessoavirusbacteriaIdGestorNavigation = new HashSet<Situacaopessoavirusbacteria>();
            SituacaopessoavirusbacteriaIdpessoaNavigation = new HashSet<Situacaopessoavirusbacteria>();
            Usuario = new HashSet<Usuario>();
        }

        public int Idpessoa { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Sexo { get; set; }
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
        public DateTime DataNascimento { get; set; }
        public byte Hipertenso { get; set; }
        public byte Diabetes { get; set; }
        public byte Obeso { get; set; }
        public byte Cardiopatia { get; set; }
        public byte Imunodeprimido { get; set; }
        public byte Cancer { get; set; }
        public byte DoencaRespiratoria { get; set; }
        public string OutrasComorbidades { get; set; }
        public string SituacaoSaude { get; set; }

        public ICollection<Exame> ExameIdAgenteSaudeNavigation { get; set; }
        public ICollection<Exame> ExameIdPacienteNavigation { get; set; }
        public ICollection<Pessoatrabalhaestado> Pessoatrabalhaestado { get; set; }
        public ICollection<Pessoatrabalhamunicipio> Pessoatrabalhamunicipio { get; set; }
        public ICollection<Situacaopessoavirusbacteria> SituacaopessoavirusbacteriaIdGestorNavigation { get; set; }
        public ICollection<Situacaopessoavirusbacteria> SituacaopessoavirusbacteriaIdpessoaNavigation { get; set; }
        public ICollection<Usuario> Usuario { get; set; }
    }
}
