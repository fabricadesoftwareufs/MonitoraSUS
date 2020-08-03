using System;
using System.Collections.Generic;
using System.Text;

namespace Model.AuxModel
{
    public class IndiceItemArquivoImportacao
	{		
		public static string UNIDADE_SOLICITANTE_GAL			 = "UNIDADE SOLICITANTE";
		public static string CNES_UNIDADE_SOLCITANTE_GAL		 = "CNES UNIDADE SOLICITANTE";
		public static string MUNICIPIO_DO_SOLICITANTE_GAL		 = "MUNICIPIO DO SOLICITANTE";
		public static string ESTADO_DO_SOLICITANTE_GAL			 = "ESTADO DO SOLICITANTE";
		public static string CNS_DO_PACIENTE_GAL				 = "CNS DO PACIENTE";
		public static string NOME_PACIENTE_GAL					 = "PACIENTE";
		public static string SEXO_PACIENTE_GAL					 = "SEXO";
		public static string DATA_DE_NASCIMENTO_PACIENTE_GAL	 = "DATA DE NASCIMENTO";
		public static string TIPO_DOCUMENTO_1_GAL				 = "TIPO DOC. PACIENTE 1";
		public static string TIPO_DOCUMENTO_2_GAL				 = "TIPO DOC. PACIENTE 2";
		public static string DOCUMENTO_1_GAL					 = "DOCUMENTO PACIENTE 1";
		public static string DOCUMENTO_2_GAL					 = "DOCUMENTO PACIENTE 2";
		public static string ENDERECO_PACIENTE_GAL				 = "ENDEREÇO";
		public static string BAIRRO_PACIENTE_GAL				 = "BAIRRO";
		public static string CEP_PACIENTE_GAL					 = "CEP DE RESIDÊNCIA";
		public static string MUNICIPIO_PACIENTE_GAL				 = "MUNICIPIO DE RESIDÊNCIA";
		public static string ESTADO_PACIENTE_GAL				 = "ESTADO DE RESIDÊNCIA";
		public static string CELULAR_PACIENTE_GAL				 = "TELEFONE DE CONTATO";
		public static string TIPO_EXAME_GAL						 = "EXAME";
		public static string METODO_EXAME_GAL					 = "METODOLOGIA";
		public static string CODIGO_DA_AMOSTRA_GAL				 = "CÓDIGO DA AMOSTRA";
		public static string DATA_DA_COLETA_GAL					 = "DATA DA COLETA";
		public static string DATA_INICIO_SINTOMAS_GAL			 = "DATA INÍCIO SINTOMAS";
		public static string OBSERVACOES_RESULTADO_GAL			 = "OBSERVAÇÕES DO RESULTADO";
		public static string RESULTADO_GAL						 = "RESULTADO";
		public static string RESULTADO_DETECTAVEL_GAL			 = "DETECTÁVEL";
		public static string RESULTADO_NAO_DETECTAVEL_GAL		 = "NÃO DETECTÁVEL";
		public static string RESULTADO_SOLICITAR_NOVA_COLETA_GAL = "SOLICITAR NOVA COLETA";
		public static string STATUS_EXAME						 = "STATUS EXAME";

		public static string NUMERO_DE_REGISTRO_UFS				= "Nº DE REGISTRO";
		public static string DATA_DA_COLETA_UFS					= "DATA DA COLETA";
		public static string NOME_PACIENTE_UFS					= "QUAL O SEU NOME COMPLETO?";
		public static string DATA_DE_NASCIMENTO_PACIENTE_UFS	= "QUAL A SUA DATA DE NASCIMENTO?";
		public static string CPF_PACIENTE_UFS					= "QUAL O SEU CPF?";
		public static string CEP_PACIENTE_UFS					= "QUAL O CEP DA SUA RESIDÊNCIA?";
		public static string ENDERECO_PACIENTE_UFS				= "QUAL A RUA, AVENIDA, TRAVESSA?";
		public static string NUMERO_RESIDENCIA_PACIENTE_UFS		= "QUAL O NÚMERO DA SUA RESIDÊNCIA";
		public static string COMPLEMENTO_PACIENTE_UFS			= "TEM COMPLEMENTO?";
		public static string MUNICIPIO_PACIENTE_UFS				= "QUAL O MUNICÍPIO EM QUE VOCÊ MORA?";
		public static string ESTADO_PACIENTE_UFS				= "QUAL O ESTADO EM QUE VOCÊ MORA?";
		public static string BAIRRO_PACIENTE_UFS				= "QUAL O BAIRRO EM QUE VOCÊ MORA?";
		public static string CELULAR_PACIENTE_UFS				= "QUAL O SEU NÚMERO DE CELULAR?";
		public static string PROFISSAO_PACIENTE_UFS				= "QUAL A SUA PROFISSÃO?";
		public static string REALIZOU_TESTE_COVID_UFS			= "JÁ REALIZOU TESTE PARA COVID-19?";
		public static string DOENCAS_CRONICAS_PACIENTE_UFS		= "EM RELAÇÃO ÀS DOENÇAS CRÔNICAS, POR FAVOR SELECIONE AS OPÇÕES ABAIXO QUE SE ENQUADRAM NO SEU PERFIL:";
		public static string DOENCAS_BACTERIANAS_PACIENTE_UFS	= "EM RELAÇÃO À DOENÇAS BACTERIANAS, VIRAIS OU PARASITÁRIAS ATIVAS, POR FAVOR SELECIONE AS OPÇÕES ABAIXO QUE SE ENQUADRAM NO SEU PERFIL:";
		public static string SINAIS_E_SINTOMAS_PACIENTE_UFS		= "SINAIS E SINTOMAS DE SÍNDROME GRIPAL";

		public static string SINTOMA_FEBRE					  = "FEBRE";
		public static string SINTOMA_TOSSE					  = "TOSSE SECA";
		public static string SINTOMA_CORIZA					  = "CORIZA";
		public static string SINTOMA_DIFICULDADE_RESPIRATORIA = "DIFICULDADE PARA RESPIRAR";
		public static string SINTOMA_DOR_DE_GARGANTA		  = "DOR DE GARGANTA";
		public static string SINTOMA_DOR_DE_OUVIDO		      = "DOR DE OUVIDO";
		public static string SINTOMA_DIARREIA				  = "DIARREIA";
		public static string SINTOMA_NAUSEAS				  = "NÁUSEAS";
		public static string SINTOMA_DORES_E_DESCONFORTO	  = "DORES E DESCONFORTOS (MUSCULAR)";
		public static string SINTOMA_PERDA_OLFATO			  = "PERDA DE PALADAR E/OU OLFATO";

		public static string DOENCA_RESPIRATORIA   = "DOENÇA RESPIRATÓRIA"; 
		public static string DOENCA_HIPERTENSAO	   = "HIPERTENSÃO ARTERIAL SISTÊMICA (HAS)";		
		public static string DOENCA_DIABETES	   = "DIABETES MELLITUS (DM) TIPO I OU II Diabetes mellitus (DM) tipo I ou II";
		public static string DOENCA_CARDIOPATIA	   = "DOENÇA CARDÍACA (CARDIOPATIA)";
		public static string DOENCA_OBESIDADE	   = "OBESIDADE";
		public static string DOENCA_RENAL		   = "DOENÇA RENAL";
		public static string DEONCA_EPILESIA	   = "EPILEPSIA";
		public static string DEONCA_IMUNODEPRIMIDO = "IMUNODEPRIMIDO";
		public static string DEONCA_CANCER		   = "CÂNCER";

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
			IndiceStatusExame = -1;
			IndiceObservacaoExame = -1;
			IndiceTipoExame = -1;
			IndiceCpfPaciente = -1;
			IndiceRealizouTeste = -1;
			IndicenNumeroResidenciaPaciente = -1;
			IndiceComplementoPaciente = -1;
			IndiceProfissaoPaciente = -1;
			IndicesDoencaPacienteUfs = new List<int>();
			IndicesSintomasPacienteUfs = new List<int>();
			EhPlanilhaGal = false;
		}

        public int IndiceNomePaciente { get; set; }
		public  int IndiceCidadePaciente { get; set; }
		public int IndiceTipoDocumento1Paciente { get; set; }
		public int IndiceDocumento1Paciente { get; set; }
		public int IndiceTipoDocumento2Paciente { get; set; }
		public int IndiceDocumento2Paciente { get; set; }
		public int IndiceCpfPaciente { get; set; }
		public int IndiceSexoPaciente { get; set; }
		public int IndiceCepPaciente { get; set; }
		public int IndiceRuaPaciente { get; set; }
		public int IndicenNumeroResidenciaPaciente { get; set; }
		public int IndiceBairroPaciente { get; set; }
		public int IndiceComplementoPaciente { get; set; }
		public int IndiceEstadoPaciente { get; set; }
		public int IndiceFoneCelularPaciente { get; set; }
		public int IndiceDataNascimentoPaciente { get; set; }
		public int IndiceCnsPaciente { get; set; }
		public int IndiceDataExame { get; set; }
		public int IndiceDataInicioSintomas { get; set; }
		public int IndiceCodigoColeta { get; set; }
		public int IndiceMetodoExame { get; set; }
		public int IndiceRealizouTeste { get; set; }
		public int IndiceTipoExame { get; set; }
		public int IndiceNomeEmpresa { get; set; }
		public int IndiceCnesEmpresa { get; set; }
		public int IndiceCidadeEmpresa { get; set; }
		public int IndiceEstadoEmpresa { get; set; }
		public int IndiceStatusExame { get; set; }
		public int IndiceObservacaoExame { get; set; }
		public int IndiceResultadoExame { get; set; }
		public int IndiceProfissaoPaciente { get; set; }

		public bool EhPlanilhaGal { get; set; }
		public List<int> IndicesSintomasPacienteUfs  { get; set;}
		public List<int> IndicesDoencaPacienteUfs { get; set; }
    }
}
