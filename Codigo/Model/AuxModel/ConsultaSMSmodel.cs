using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Model.AuxModel
{
	[DataContract]
	public class ConsultaSMSModel
    {
		//situacao":"OK","codigo":"1","data_envio":"08\/05\/2020 21:49:00","operadora":"TIM-PORTABILIDADE","qtd_credito":"3","descricao":"RECEBIDA"
		public const string SITUACAO_ENTREGUE = "RECEBIDA";

        public string Situacao { get; set; }
        public int Codigo { get; set; }
        public DateTime Data_envio { get; set; }
        public string Operadora { get; set; }
		public int Qtd_Credito { get; set; }
		public string Descricao { get; set; }
	}
}
