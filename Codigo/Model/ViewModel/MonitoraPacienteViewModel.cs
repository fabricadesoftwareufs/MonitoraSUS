using System;
using System.Collections.Generic;
using System.Text;

namespace Model.ViewModel
{
    public class MonitoraPacienteViewModel
    {

        public MonitoraPacienteViewModel() 
        {
            ExamesPaciente = new List<ExameViewModel>();
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
        public bool Hipertenso { get; set; }
        public bool Diabetes { get; set; }
        public bool Obeso { get; set; }
        public bool Cardiopatia { get; set; }
        public bool Imunodeprimido { get; set; }
        public bool Cancer { get; set; }
        public bool DoencaRespiratoria { get; set; }
        public string OutrasComorbidades { get; set; }

        public bool TemFoneCelularValido
        {
            get
            {
                if (FoneCelular == null)
                    return false;
                if (FoneCelular.Length != 11)
                    return false;
                if (FoneCelular.StartsWith("0"))
                    return false;
                return true;
            }
        }

        public VirusBacteriaModel VirusBacteria { get; set; }
        public List<ExameViewModel> ExamesPaciente { get; set; }
        public PessoaModel Gestor { get; set; }
        public string UltimaSituacao { get; set; }
        public DateTime? DataUltimoMonitoramento { get; set; }
        public string Descricao { get; set; }
    }
}
