using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class PessoaModel
    {
        public const string SITUACAO_SAUDAVEL = "S";
        public const string SITUACAO_ISOLAMENTO = "I";
        public const string SITUACAO_HOSPITALIZADO_INTERNAMENTO = "H";
        public const string SITUACAO_ESTABILIZACAO = "E";
        public const string SITUACAO_UTI = "U";
        public const string SITUACAO_OBITO = "O";
        public PessoaModel()
        {
            SituacaoSaude = SITUACAO_SAUDAVEL;
            Cns = "";
        }

        public int Idpessoa { get; set; }
        [Display(Name = "CPF")]
        [Util.CPF]
        public string Cpf { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [Display(Name = "Nome")]
        [StringLength(60, ErrorMessage = "Máximo são 60 caracteres")]
        public string Nome { get; set; }
        [Required]
        [Display(Name = "Sexo")]
        public string Sexo { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [Util.Cep]
        public string Cep { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(60, ErrorMessage = "Máximo são 60 caracteres")]
        public string Rua { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(60, ErrorMessage = "Máximo são 60 caracteres")]
        public string Bairro { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(100, ErrorMessage = "Máximo são 100 caracteres")]
        public string Cidade { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(50, ErrorMessage = "Máximo são 50 caracteres")]
        public string Estado { get; set; }
        [StringLength(100, ErrorMessage = "Máximo são 20 caracteres")]
        public string Numero { get; set; }
        [StringLength(100, ErrorMessage = "Máximo são 100 caracteres")]
        public string Complemento { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [Util.TelefoneCelular]
        public string FoneCelular { get; set; }
        [Util.TelefoneFixo]
        public string FoneFixo { get; set; }
        [StringLength(60, ErrorMessage = "Máximo são 60 caracteres")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [DataType(DataType.Date, ErrorMessage = "Data inválida")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataNascimento { get; set; }
        public bool Hipertenso { get; set; }
        public bool Diabetes { get; set; }
        public bool Obeso { get; set; }
        public bool Cardiopatia { get; set; }
        public bool Imunodeprimido { get; set; }
        public bool Cancer { get; set; }
        [Display(Name = "Doença Respiratória")]
        public bool DoencaRespiratoria { get; set; }
        [Display(Name = "Doença Renal")]
        public bool DoencaRenal { get; set; }
        public bool Epilepsia { get; set; }
        [StringLength(100, ErrorMessage = "Máximo são 100 caracteres")]
        [Display(Name = "Outras Comorbidades")]
        public string OutrasComorbidades { get; set; }
        public string SituacaoSaude { get; set; }
        [Display(Name = "Data Óbito")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DataObito { get; set; }

        public string SituacaoSaudeDescricao
        {
            get
            {
                if (SituacaoSaude.Equals(SITUACAO_ISOLAMENTO))
                    return "Isolamento";
                if (SituacaoSaude.Equals(SITUACAO_HOSPITALIZADO_INTERNAMENTO))
                    return "Internamento Clínico";
                if (SituacaoSaude.Equals(SITUACAO_ESTABILIZACAO))
                    return "Estabilização";
                if (SituacaoSaude.Equals(SITUACAO_UTI))
                    return "UTI";
                if (SituacaoSaude.Equals(SITUACAO_OBITO))
                    return "Óbito";
                else
                    return "Recuperado";
            }
        }
        public bool Febre { get; set; }
        public bool Tosse { get; set; }
        public bool Coriza { get; set; }
        [Display(Name = "Dificuldade Respiratória")]
        public bool DificuldadeRespiratoria { get; set; }
        [Display(Name = "Dor na Garganta")]
        public bool DorGarganta { get; set; }
        [Display(Name = "Diarréia")]
        public bool Diarreia { get; set; }
        [Display(Name = "Dor no Ouvido")]
        public bool DorOuvido { get; set; }
        [Display(Name = "Náusea")]
        public bool Nausea { get; set; }
        [Display(Name = "Dor Abdominal")]
        public bool DorAbdominal { get; set; }
        [Display(Name = "Perda Olfato/Paladar")]
        public bool PerdaOlfatoPaladar { get; set; }
        [StringLength(100, ErrorMessage = "Máximo de 100 caracteres")]
        public string OutrosSintomas { get; set; }
        public int IdAreaAtuacao { get; set; }
        [Display(Name = "CNS")]
        [StringLength(15, ErrorMessage = "Máximo de 15 números")]
        public string Cns { get; set; }
        [StringLength(50, ErrorMessage = "Máximo de 50 caracteres")]
        [Display(Name = "Profissão")]
        public string Profissao { get; set; }
    }
}
