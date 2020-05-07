using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class ExameViewModel
    {
        public ExameViewModel()
        {
            IgM = "N";
            IgG = "N";
            Pcr = "N";
            IdAgenteSaude = new PessoaModel();
            IdVirusBacteria = new VirusBacteriaModel();
            IdPaciente = new PessoaModel();
            IdPaciente.Sexo = "M";
            DataExame = DateTime.Now;
            DataInicioSintomas = DateTime.Now;
            IdEstado = 0;
            MunicipioId = 0;
            PesquisarCpf = 0;
            Resultado = "";
            FoiNotificado = false;
        }
        public int IdExame { get; set; }
        [Display(Name = "Virus ou Bacteria")]
        public VirusBacteriaModel IdVirusBacteria { get; set; }
        [Display(Name = "Paciente")]
        public PessoaModel IdPaciente { get; set; }
        [Display(Name = "Agente de Saude")]
        public PessoaModel IdAgenteSaude { get; set; }
        [Display(Name = "Data do Exame")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataExame { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataInicioSintomas { get; set; }
        public string IgG { get; set; }
        public string IgM { get; set; }
        public string Pcr { get; set; }
        public int IdEstado { get; set; }
        public int? MunicipioId { get; set; }
        public int? IdEmpresaSaude { get; set; }
        public int PesquisarCpf { get; set; }
        [Display(Name = "Resultado")]
        public string Resultado { get; set; }
        public bool FoiNotificado {get;set;}

    }
}
