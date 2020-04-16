using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class EmailModel
    {
        [Required, Display(Name = "Email de destino"), EmailAddress]
        public string Destino { get; set; }
        [Required, Display(Name = "Assunto")]
        public string Assunto { get; set; }
        [Required, Display(Name = "Mensagem")]
        public string Mensagem { get; set; }
    }
}
