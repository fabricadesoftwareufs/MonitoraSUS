using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Model.AuxModel
{
	[DataContract]
	public class ConsultaSMSModel
    {
		public const string SITUACAO_ENTREGUE = "RECEBIDA";

        public string Situacao { get; set; }
        public int Codigo { get; set; }
        public DateTime Data_envio { get; set; }
        public string Operadora { get; set; }
		public string Descricao { get; set; }

	}
}
