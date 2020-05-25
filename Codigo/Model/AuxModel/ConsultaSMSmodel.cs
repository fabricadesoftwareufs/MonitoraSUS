using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Model.AuxModel
{
    [DataContract]
    public class ConsultaSMSModel
    {
        //situacao":"OK","codigo":"1","data_envio":"08\/05\/2020 21:49:00","operadora":"TIM-PORTABILIDADE","qtd_credito":"3","descricao":"RECEBIDA"
        public const string SITUACAO_ENTREGUE = "RECEBIDA";

		[JsonProperty("situacao")]
		public string Situacao { get; set; }

		[JsonProperty("codigo")]
		public string Codigo { get; set; }

		[JsonProperty("data_envio")]
		public string DataEnvio { get; set; }

		[JsonProperty("operadora")]
		public string Operadora { get; set; }

		[JsonProperty("qtd_credito")]
		public string QtdCredito { get; set; }

		[JsonProperty("descricao")]
		public string Descricao { get; set; }

	}
}
