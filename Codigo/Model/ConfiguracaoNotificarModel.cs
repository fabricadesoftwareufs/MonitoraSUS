using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class ConfiguracaoNotificarModel
    {
        public int IdConfiguracaoNotificar { get; set; }
        public bool HabilitadoSms { get; set; }
        public bool HabilitadoWhatsapp { get; set; }
        public string Sid { get; set; }
        public string Token { get; set; }
        public string NumeroSms { get; set; }
        public string NumeroWhatsapp { get; set; }
        public string MensagemPositivo { get; set; }
        public string MensagemNegativo { get; set; }
        public string MensagemImunizado { get; set; }
        public string MensagemIndeterminado { get; set; }
        public int? IdEstado { get; set; }
        public int? IdMunicipio { get; set; }
        public int? IdEmpresaExame { get; set; }
    }
}
