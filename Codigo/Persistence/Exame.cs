using System;
using System.Collections.Generic;

namespace Persistence
{
    public partial class Exame
    {
        public int IdExame { get; set; }
        public int IdVirusBacteria { get; set; }
        public int IdPaciente { get; set; }
        public int IdAgenteSaude { get; set; }
        public DateTime DataExame { get; set; }
        public DateTime DataInicioSintomas { get; set; }
        public string IgG { get; set; }
        public string IgM { get; set; }
        public string Pcr { get; set; }
        public string IgMigG { get; set; }
        public int IdEstado { get; set; }
        public int? IdMunicipio { get; set; }
        public int? IdEmpresaSaude { get; set; }
        public DateTimeOffset DataNotificacao { get; set; }
        public string CodigoColeta { get; set; }
        public string StatusNotificacao { get; set; }
        public string IdNotificacao { get; set; }
        public byte AguardandoResultado { get; set; }
        public string MetodoExame { get; set; }
        public byte RelatouSintomas { get; set; }
        public byte Febre { get; set; }
        public byte Tosse { get; set; }
        public byte Coriza { get; set; }
        public byte DificuldadeRespiratoria { get; set; }
        public byte DorGarganta { get; set; }
        public byte Diarreia { get; set; }
        public byte DorOuvido { get; set; }
        public byte Nausea { get; set; }
        public byte DorAbdominal { get; set; }
        public byte PerdaOlfatoPaladar { get; set; }
        public string OutroSintomas { get; set; }
        public int IdAreaAtuacao { get; set; }

        public Pessoa IdAgenteSaudeNavigation { get; set; }
        public Areaatuacao IdAreaAtuacaoNavigation { get; set; }
        public Empresaexame IdEmpresaSaudeNavigation { get; set; }
        public Estado IdEstadoNavigation { get; set; }
        public Municipio IdMunicipioNavigation { get; set; }
        public Pessoa IdPacienteNavigation { get; set; }
        public Virusbacteria IdVirusBacteriaNavigation { get; set; }
    }
}
