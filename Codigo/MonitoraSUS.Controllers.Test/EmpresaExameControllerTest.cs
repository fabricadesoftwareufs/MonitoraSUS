using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Model;
using Moq;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MonitoraSUS.Controllers.Test
{
    public class EmpresaExameControllerTest
    {
        private Mock<IEmpresaExameService> mockEmpresaExame;
        private EmpresaExameController controller;

        private void SetUp()
        {
            mockEmpresaExame = new Mock<IEmpresaExameService>();
            var mockConfig = new Mock<IConfiguration>();
            var mockExame = new Mock<IExameService>();
            var mockPessoa = new Mock<IPessoaService>();
            var mockPessoaEstado = new Mock<IPessoaTrabalhaEstadoService>();
            var mockPessoaMunicipio = new Mock<IPessoaTrabalhaMunicipioService>();
            var mockEstado = new Mock<IEstadoService>();
            var mockMunicipio = new Mock<IMunicipioService>();
            
            var mockUsuario = new Mock<IUsuarioService>();

            //para que ocorra o cadastro com sucesso, não pode ter outra empresa com o mesmo cnpj 
            mockEmpresaExame.Setup(repo => repo.GetByCnpj(It.IsAny<string>())).Returns(new List<EmpresaExameModel>() { });

            mockEmpresaExame.Setup(repo => repo.Insert(It.IsAny<EmpresaExameModel>())).Verifiable();

            // mock do TempData
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            controller =
                    new EmpresaExameController(mockConfig.Object,
                                                mockEmpresaExame.Object,
                                                mockExame.Object,
                                                mockPessoa.Object,
                                                mockPessoaEstado.Object,
                                                mockPessoaMunicipio.Object,
                                                mockEstado.Object,
                                                mockMunicipio.Object,
                                                mockUsuario.Object
                                              )
                    {
                        TempData = tempData
                    };

        }

        [Fact(DisplayName = "Adiciona Empresa com sucesso e Retorna o redirect de quando o sts do modelo é válido")]
        public void AdicionaEmpresaComSucesso()
        {
            SetUp();

            var novaEmpresa = GetEmpresaExame();

            // Act
            var resultado = controller.Create(novaEmpresa) as ViewResult;
            Assert.IsType<ViewResult>(resultado);
            Assert.Equal("Organização Cadastrada com sucesso!", resultado.TempData["MensagemSucesso"]);
            mockEmpresaExame.Verify();
        }

        private EmpresaExameModel GetEmpresaExame()
            => new EmpresaExameModel
            {
                Bairro = "Mamade Paes Mendonca",
                Cep = "4950000",
                Cidade = "Itabaiana",
                Cnes = "Cnes",
                Cnpj = "10368753000108",
                Complemento = "xxx",
                EhPublico = true,
                Email = "ita@gmail.com",
                EmiteLaudoExame = true,
                Estado = "Sergipe",
                FazMonitoramento = true,
                FoneCelular = "7977777777",
                FoneFixo = "79999999900",
                Id = 5,
                Latitude = "16.444444",
                Longitude = "35.666666",
                Nome = "Ita Laboratorio",
                Numero = "165",
                NumeroLeitos = 100,
                NumeroLeitosDisponivel = 10,
                NumeroLeitosUti = 5,
                NumeroLeitosUtidisponivel = 1,
                PossuiLeitosInternacao = true,
                Rua = "Percilio Andrade"
            };

        private List<EmpresaExameModel> GetEmpresas()
          =>
              new List<EmpresaExameModel>()
              {
                new EmpresaExameModel
                {
                    Id = 1,
                    Cnpj = "99999999999",
                    Nome = "MUNICIPIO OU ESTADO",
                    Cep = "99999999",
                    Rua = "RUA",
                    Bairro = "BAIRRO",
                    Cidade = "CIDADE",
                    Estado = "ESTADO",
                    Numero = "999",
                    Complemento = null,
                    Latitude = "0",
                    Longitude = "0",
                    FoneCelular = "9999999999",
                    FoneFixo = "9999999999",
                    Email = "marcosdosea@yahoo.com.br",
                    EhPublico = false,
                    FazMonitoramento = false,
                    EmiteLaudoExame = false,
                    NumeroLeitos = 0,
                    NumeroLeitosDisponivel = 0,
                    NumeroLeitosUti = 0,
                    NumeroLeitosUtidisponivel = 0,
                    Cnes = ""
                },
                new EmpresaExameModel
                {
                    Id = 1,
                    Cnpj = "99999999999",
                    Nome = "MUNICIPIO OU ESTADO",
                    Cep = "99999999",
                    Rua = "RUA",
                    Bairro = "BAIRRO",
                    Cidade = "CIDADE",
                    Estado = "ESTADO",
                    Numero = "999",
                    Complemento = null,
                    Latitude = "0",
                    Longitude = "0",
                    FoneCelular = "9999999999",
                    FoneFixo = "9999999999",
                    Email = "marcosdosea@yahoo.com.br",
                    EhPublico = false,
                    FazMonitoramento = false,
                    EmiteLaudoExame = false,
                    NumeroLeitos = 0,
                    NumeroLeitosDisponivel = 0,
                    NumeroLeitosUti = 0,
                    NumeroLeitosUtidisponivel = 0,
                    Cnes = ""
                },
                new EmpresaExameModel
                {
                    Id = 3,
                    Cnpj = "13031547000104",
                    Nome = "Fundação Universidade Federal de Sergipe",
                    Cep = "49510200",
                    Rua = "Avenida Vereador Olimpio Grande",
                    Bairro = "Porto",
                    Cidade = "Itabaiana",
                    Estado = "SE",
                    Numero = "sn",
                    Complemento = null,
                    Latitude = "-10.6821891",
                    Longitude = "-37.4379664",
                    FoneCelular = "9999999999",
                    FoneFixo = "9999999999",
                    Email = "marcosdosea@yahoo.com.br",
                    EhPublico = false,
                    FazMonitoramento = false,
                    EmiteLaudoExame = false,
                    NumeroLeitos = 0,
                    NumeroLeitosDisponivel = 0,
                    NumeroLeitosUti = 0,
                    NumeroLeitosUtidisponivel = 0,
                    Cnes = ""
                },
                new EmpresaExameModel
                {
                    Id = 2,
                    Cnpj = "32799603000191",
                    Nome = "Sao Marcos laboratorios",
                    Cep = "49050420",
                    Rua = "Rua João Rocha Sobrinho",
                    Bairro = "Pereira Lobo",
                    Cidade = "Recife",
                    Estado = "PE",
                    Numero = "100",
                    Complemento = null,
                    Latitude = "-10.92542",
                    Longitude = "-37.0686706",
                    FoneCelular = "79999002396",
                    FoneFixo = "7932213164",
                    Email = "saomarcosmc@yahoo.com.br",
                    EhPublico = true,
                    FazMonitoramento = false,
                    EmiteLaudoExame = true,
                    NumeroLeitos = 0,
                    NumeroLeitosDisponivel = 0,
                    NumeroLeitosUti = 0,
                    NumeroLeitosUtidisponivel = 0,
                    Cnes = ""
                },
              };

    }
}
