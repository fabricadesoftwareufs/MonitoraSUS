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
        public int IdEstado { get; set; }
        public int? IdMunicipio { get; set; }
        public int? IdEmpresaSaude { get; set; }
        public DateTimeOffset DataNotificacao { get; set; }
        public byte FoiNotificado { get; set; }

        public Pessoa IdAgenteSaudeNavigation { get; set; }
        public Empresaexame IdEmpresaSaudeNavigation { get; set; }
        public Estado IdEstadoNavigation { get; set; }
        public Municipio IdMunicipioNavigation { get; set; }
        public Pessoa IdPacienteNavigation { get; set; }
        public Virusbacteria IdVirusBacteriaNavigation { get; set; }
    }
}
