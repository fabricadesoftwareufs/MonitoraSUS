using System;
using System.Collections.Generic;
using System.Text;

namespace Model.AuxModel
{
    public class IndiceItemArquivoImportacao
	{
		public static string UNIDADE_SOLICITANTE	  = "UNIDADE SOLICITANTE";
		public static string CNES_UNIDADE_SOLCITANTE  = "CNES UNIDADE SOLICITANTE";
		public static string MUNICIPIO_DO_SOLICITANTE = "MUNICIPIO DO SOLICITANTE";
		public static string ESTADO_DO_SOLICITANTE	  = "ESTADO DO SOLICITANTE";
		public static string CNS_DO_PACIENTE		  = "CNS DO PACIENTE";
		public static string NOME_PACIENTE			  = "PACIENTE";
		public static string SEXO_PACIENTE			  = "SEXO";
		public static string DATA_DE_NASCIMENTO_PACIENTE = "DATA DE NASCIMENTO";
		public static string TIPO_DOCUMENTO_1		  = "TIPO DOC. PACIENTE 1";
		public static string TIPO_DOCUMENTO_2		  = "TIPO DOC. PACIENTE 2";
		public static string DOCUMENTO_1			  = "DOCUMENTO PACIENTE 1";
		public static string DOCUMENTO_2			  = "DOCUMENTO PACIENTE 2";
		public static string ENDERECO_PACIENTE		  = "ENDEREÇO";
		public static string BAIRRO_PACIENTE		  = "BAIRRO";
		public static string CEP_PACIENTE			  = "CEP DE RESIDÊNCIA";
		public static string MUNICIPIO_PACIENTE		  = "MUNICIPIO DE RESIDÊNCIA";
		public static string ESTADO_PACIENTE		  = "ESTADO DE RESIDÊNCIA";
		public static string CELULAR_PACIENTE		  = "TELEFONE DE CONTATO";
		public static string TIPO_EXAME				  = "EXAME";
		public static string METODO_EXAME			  = "METODOLOGIA";
		public static string CODIGO_DA_AMOSTRA		  = "CÓDIGO DA AMOSTRA";
		public static string DATA_DA_COLETA			  = "DATA DA COLETA";
		public static string DATA_INICIO_SINTOMAS	  = "DATA INÍCIO SINTOMAS";
		public static string OBSERVACOES_RESULTADO	  = "OBSERVAÇÕES DO RESULTADO";
		public static string RESULTADO				  = "RESULTADO";
		public static string RESULTADO_DETECTAVEL     = "DETECTÁVEL";
		public static string RESULTADO_NAO_DETECTAVEL = "NÃO DETECTÁVEL";
		public static string RESULTADO_SOLICITAR_NOVA_COLETA = "SOLICITAR NOVA COLETA";
		public static string METODO_IGG = "IGG";
		public static string METODO_IGM = "IGM";
		public static string METODO_PCR = "PCR";
		public static string METODO_IGG_IGM = "IGG/IGM";

		public IndiceItemArquivoImportacao()
		{
			IndiceNomePaciente = -1;
			IndiceCidadePaciente = -1;
			IndiceTipoDocumento1Paciente = -1;
			IndiceTipoDocumento2Paciente = -1;
			IndiceDocumento1Paciente = -1;
			IndiceDocumento2Paciente = -1;
			IndiceSexoPaciente = -1;
			IndiceCepPaciente = -1;
			IndiceRuaPaciente = -1;
			IndiceBairroPaciente = -1;
			IndiceEstadoPaciente = -1;
			IndiceFoneCelularPaciente = -1;
			IndiceDataNascimentoPaciente = -1;
			IndiceCnsPaciente = -1;
			IndiceDataExame = -1;
			IndiceDataInicioSintomas = -1;
			IndiceMetodoExame = -1;
			IndiceNomeEmpresa = -1;
			IndiceCnesEmpresa = -1;
			IndiceCidadeEmpresa = -1;
			IndiceEstadoEmpresa = -1;
			IndiceCodigoColeta = -1;
			IndiceResultadoExame = -1;
			IndiceObservacaoExame = -1;
			IndiceTipoExame = -1;
		}

        public int IndiceNomePaciente { get; set; }
		public  int IndiceCidadePaciente { get; set; }
		public int IndiceTipoDocumento1Paciente { get; set; }
		public int IndiceDocumento1Paciente { get; set; }
		public int IndiceTipoDocumento2Paciente { get; set; }
		public int IndiceDocumento2Paciente { get; set; }
		public int IndiceSexoPaciente { get; set; }
		public int IndiceCepPaciente { get; set; }
		public int IndiceRuaPaciente { get; set; }
		public int IndiceBairroPaciente { get; set; }
		public int IndiceEstadoPaciente { get; set; }
		public int IndiceFoneCelularPaciente { get; set; }
		public int IndiceDataNascimentoPaciente { get; set; }
		public int IndiceCnsPaciente { get; set; }
		public int IndiceDataExame { get; set; }
		public int IndiceDataInicioSintomas { get; set; }
		public int IndiceCodigoColeta { get; set; }
		public int IndiceMetodoExame { get; set; }
		public int IndiceTipoExame { get; set; }
		public int IndiceNomeEmpresa { get; set; }
		public int IndiceCnesEmpresa { get; set; }
		public int IndiceCidadeEmpresa { get; set; }
		public int IndiceEstadoEmpresa { get; set; }
		public int IndiceObservacaoExame { get; set; }
		public int IndiceResultadoExame { get; set; }
    }
}
