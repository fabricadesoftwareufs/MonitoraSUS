using Model;
using System.Collections.Generic;
using Xunit;

namespace Controller.Test
{
    public class ExameControllerTest
    {
        [Fact(DisplayName = "Adiciona um Exame com sucesso e Retorna o redirect de quando o sts do modelo é válido")]
        public void AddExameComSucesso()
        {
            
        }

        private List<ExameModel> GetExames()
        => new List<ExameModel>
        {
            new ExameModel
            {
                IgG = "N",
                IgGIgM = "N",
                IgM = "N",
                Pcr = "N",
                IdEstado = 1,
                IdMunicipio = 1,
                IdEmpresaSaude = 1,
                IdNotificacao = null,
                CodigoColeta = "ABC",
                StatusNotificacao = "N",
            }, new ExameModel
            {
                IgG = "S",
                IgGIgM = "S",
                IgM = "S",
                Pcr = "S",
                IdEstado = 4,
                IdMunicipio = 4,
                IdEmpresaSaude = 4,
                IdNotificacao = null,
                CodigoColeta = "ABCS",
                StatusNotificacao = "N",
            },
            new ExameModel
            {
                IgG = "S",
                IgGIgM = "S",
                IgM = "S",
                Pcr = "N",
                IdEstado = 2,
                IdMunicipio = 2,
                IdEmpresaSaude = 2,
                IdNotificacao = null,
                CodigoColeta = "AB",
                StatusNotificacao = "N",
            }
        };
    }
}
