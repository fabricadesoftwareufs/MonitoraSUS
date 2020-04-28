using Model;
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
<<<<<<< HEAD
        List<ExameModel> GetByIdPaciente(int idPaciente);
=======
>>>>>>> 9c8df4baf41ba7459f1a3cd8fed596a17c95511f
        List<ExameModel> CheckDuplicateExamToday(int idPaciente,int idVirusBacteria, DateTime dateExame);
    }
}
