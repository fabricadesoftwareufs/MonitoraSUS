using Model;
using Model.ViewModel;
using System;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IExameService
    {
        bool Insert(ExameModel exameModel);
        bool Update(ExameModel exameModel);
        bool Delete(int id);
        List<ExameModel> GetAll();
        ExameModel GetById(int id);
        List<ExameModel> GetByIdAgente(int id);
        List<ExameModel> GetByIdEstado(int idEstado);
        List<ExameModel> GetByIdEmpresa(int idEempresa);
        List<ExameModel> GetByIdPaciente(int idPaciente);
        List<ExameModel> GetByIdMunicipio(int idMunicipio);
        List<ExameModel> CheckDuplicateExamToday(int idPaciente, int idVirusBacteria, DateTime dateExame);

        List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByEmpresa(int idEempresa);
        List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByEstado(int idEstado);
        List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByMunicipio(int idMunicipio);

		ConfiguracaoNotificarModel BuscarConfiguracaoNotificar(int IdEstado, int IdEmpresaExame);
		ConfiguracaoNotificarModel BuscarConfiguracaoNotificar(int IdMunicipio);
		System.Threading.Tasks.Task<bool> EnviarSMSResultadoExameAsync(ConfiguracaoNotificarModel configuracaoNotificar, ExameModel exame, PessoaModel pessoa);
		System.Threading.Tasks.Task<bool> ConsultarSMSExameAsync(ConfiguracaoNotificarModel configuracaoNotificar, ExameModel exame);
	}
}
