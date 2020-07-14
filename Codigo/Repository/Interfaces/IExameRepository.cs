using Model;
using Model.ViewModel;
using Persistence;
using System;
using System.Collections.Generic;

namespace Repository.Interfaces
{
    public interface IExameRepository
    {
        bool Delete(int id);
        List<MonitoraPacienteViewModel> GetByCidadeResidenciaPaciente(string cidade, string siglaEstado, int idVirusBacteria, DateTime dataInicio, DateTime dataFim);
        List<MonitoraPacienteViewModel> GetByEstadoResidenciaPaciente(string siglaEstado, int idVirusBacteria, DateTime dataInicio, DateTime dataFim);
        List<MonitoraPacienteViewModel> GetByHospital(int idEmpresa, int idVirusBacteria, DateTime dataInicio, DateTime dataFim);
        ExameViewModel GetById(int id);
        List<ExameBuscaModel> GetByIdAgente(int idAgente, DateTime dataInicio, DateTime dataFim);
        List<ExameBuscaModel> GetByIdAgente(int idAgente, int lastRecord);
        ExameModel GetByIdColeta(string codigoColeta);
        List<ExameBuscaModel> GetByIdEmpresa(int idEempresa, DateTime dataInicio, DateTime dataFim);
        List<ExameBuscaModel> GetByIdEmpresa(int idEempresa, int lastRecord);
        List<ExameBuscaModel> GetByIdEstado(int idEstado, DateTime dataInicio, DateTime dataFim);
        List<ExameBuscaModel> GetByIdEstado(int idEstado, int lastRecord);
        List<ExameBuscaModel> GetByIdMunicipio(int idMunicicpio, DateTime dataInicio, DateTime dataFim);
        List<ExameBuscaModel> GetByIdMunicipio(int idMunicicpio, int lastRecord);
        List<ExameBuscaModel> GetByIdPaciente(int idPaciente);
        List<ExameModel> GetExamesRelizadosData(int idPaciente, int idVirusBacteria, DateTime dateExame, string metodoExame);
        List<TotalEstadoMunicipioBairro> GetTotaisPopulacaoByEstado(string siglaEstado);
        List<TotalEstadoMunicipioBairro> GetTotaisPopulacaoByMunicipio(string siglaEstado, string cidade);
        List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByEmpresa(int idEempresa);
        List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByEstado(int idEstado);
        List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByMunicipio(int idMunicipio);
        bool Insert(ExameViewModel exameModel);
        bool Update(ExameViewModel exameModel);
    }
}
