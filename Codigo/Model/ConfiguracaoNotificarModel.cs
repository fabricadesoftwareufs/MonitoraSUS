namespace Model
{
    public class ConfiguracaoNotificarModel
    {
        public int IdConfiguracaoNotificar { get; set; }
        public byte HabilitadoSms { get; set; }
        public byte HabilitadoWhatsapp { get; set; }
        public string Sid { get; set; }
        public string Token { get; set; }
        public string MensagemPositivo { get; set; }
        public string MensagemNegativo { get; set; }
        public string MensagemCurado { get; set; }
        public string MensagemIndeterminado { get; set; }
        public int? IdEstado { get; set; }
        public int? IdMunicipio { get; set; }
        public int? IdEmpresaExame { get; set; }
        public int QuantidadeSmsdisponivel { get; set; }

    }
}
