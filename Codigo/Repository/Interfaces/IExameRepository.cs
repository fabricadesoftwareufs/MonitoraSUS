using Model;
using Persistence;
using System;
using System.Collections.Generic;

namespace Repository.Interfaces
{
    public interface IExameRepository
    {
        bool Insert(ExameViewModel exameModel);
        List<ExameModel> GetExamesRelizadosData(int idPaciente, int idVirusBacteria, DateTime dateExame, string metodoExame);

        // Informações do Contexto.
        monitorasusContext GetContext();
    }
}
