using System;
using System.Collections.Generic;
using System.Text;
using Model;

namespace Service.Interface
{
    public interface IInternacaoService
    {

        bool Insert(InternacaoModel internacaoModel);
        bool update(InternacaoModel internacaoModel);
        bool Delete(int idInternacao);
        List<InternacaoModel> GetAll();
        InternacaoModel GetById(int idInternacao);
        List<InternacaoModel> GetByIdPaciente(int idpaciente);
    }
}
