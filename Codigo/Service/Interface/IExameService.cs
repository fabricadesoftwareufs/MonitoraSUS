using Model;
using Model.ViewModel;
using System;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IExameService
    {
        bool Insert(ExameViewModel exameModel);
        bool Update(ExameViewModel exameModel);
        bool Delete(int id);
		ExameViewModel GetById(int id);
		List<ExameBuscaModel> GetByIdAgente(int idAgente, int lastRecord);
		List<ExameBuscaModel> GetByIdEstado(int idEstado, int lastRecord);
		List<ExameBuscaModel> GetByIdEmpresa(int idEempresa, int lastRecord);
		List<ExameBuscaModel> GetByIdMunicipio(int idMunicipio, int lastRecord);
		List<ExameBuscaModel> GetByIdAgente(int idAgente, DateTime dataInicio, DateTime dataFim);
        List<ExameBuscaModel> GetByIdEstado(int idEstado, DateTime dataInicio, DateTime dataFim);
        List<ExameBuscaModel> GetByIdEmpresa(int idEempresa, DateTime dataInicio, DateTime dataFim);
		List<ExameBuscaModel> GetByIdMunicipio(int idMunicipio, DateTime dataInicio, DateTime dataFim);
		List<ExameBuscaModel> GetByIdPaciente(int idPaciente);
        List<ExameModel> GetExamesRelizadosData(int idPaciente, int idVirusBacteria, DateTime dateExame, string metodoExame);
		ExameModel GetByIdColeta(string codigoColeta);

		List<MonitoraPacienteViewModel> GetByEstadoResidenciaPaciente(string siglaEstado,
			int idVirusBacteria, DateTime dataInicio, DateTime dataFim);
		List<MonitoraPacienteViewModel> GetByCidadeResidenciaPaciente(string cidade, string siglaEstado,
			int idVirusBacteria, DateTime dataInicio, DateTime dataFim);
		List<MonitoraPacienteViewModel> GetByHospital(int idEmpresa, int idVirusBacteria, DateTime dataInicio, DateTime dataFim);
		List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByEmpresa(int idEempresa);
        List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByEstado(int idEstado);
        List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByMunicipio(int idMunicipio);
        List<TotalEstadoMunicipioBairro> GetTotaisPopulacaoByEstado(string siglaEstado);
        List<TotalEstadoMunicipioBairro> GetTotaisPopulacaoByMunicipio(string siglaEstado, string cidade);
		void CorrigeLocalizacao(PessoaModel pessoa, string googleKey);
	}
}
