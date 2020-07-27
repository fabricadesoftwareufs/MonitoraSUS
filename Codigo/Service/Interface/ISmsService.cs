using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interface
{
    public interface ISmsService
    {
        System.Threading.Tasks.Task<ExameModel> EnviarSMSResultadoExameAsync(PessoaTrabalhaEstadoModel pessoaTrabalhaEstado, 
            PessoaTrabalhaMunicipioModel pessoaTrabalhaMunicipio, ExameModel exame, PessoaModel pessoa);
        System.Threading.Tasks.Task<ExameModel> ConsultarSMSExameAsync(PessoaTrabalhaEstadoModel pessoaTrabalhaEstado, 
            PessoaTrabalhaMunicipioModel pessoaTrabalhaMunicipio, ExameModel exame);
    }
}
