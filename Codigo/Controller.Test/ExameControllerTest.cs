using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Model;
using Model.ViewModel;
using MonitoraSUS.Controllers;
using Moq;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace Controller.Test
{
    public class ExameControllerTest
    {
        private Mock<IVirusBacteriaService> mockVirus;
        private Mock<IExameService> mockExame;
        private Mock<IPessoaService> mockPessoa;
        private Mock<IMunicipioService> mockMunicipio;
        private Mock<IEstadoService> mockEstado;
        private Mock<ISituacaoVirusBacteriaService> mockSituacao;
        private Mock<IPessoaTrabalhaEstadoService> mockTrabalhaEstado;
        private Mock<IPessoaTrabalhaMunicipioService> mockTrabalhaMunicipio;
        private Mock<IAreaAtuacaoService> mockArea;
        private Mock<IUsuarioService> mockUsuario;
        private Mock<IConfiguration> mockConfig;

        [Fact(DisplayName = "Adiciona um Exame com sucesso e o paciente não está cadastrado")]
        public void AddExameComSucesso()
        {
            InstantiateMock();

            mockVirus.Setup(repo => repo.GetAll()).Returns(new List<VirusBacteriaModel>() { });
            mockArea.Setup(repo => repo.GetAll()).Returns(new List<AreaAtuacaoModel>() { });

            mockUsuario.Setup(repo => repo.RetornLoggedUser(It.IsAny<ClaimsIdentity>())).Returns(GetUsuario());

            // mock do TempData e HttpContext com claims
            var httpContext = new DefaultHttpContext()
            {
                User = GetClaims()
            };

            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            tempData["mensagemSucesso"] = "Notificação realizada com SUCESSO!";

        var controller =
                   new ExameController(
                                               mockVirus.Object,
                                               mockExame.Object,
                                               mockPessoa.Object,
                                               mockMunicipio.Object,
                                               mockEstado.Object,
                                               mockConfig.Object,
                                               mockSituacao.Object,
                                               mockTrabalhaEstado.Object,
                                               mockTrabalhaMunicipio.Object,
                                               mockArea.Object,
                                               mockUsuario.Object

                                       )
                   {

                       //config mock claims
                       ControllerContext = new ControllerContext
                       {
                           HttpContext = httpContext
                       },
                       TempData = tempData
                   };

            var novoExame = GetExame();
            // Act
            var resultado = controller.Create(novoExame);
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Create", redirectToActionResult.ActionName);
            mockExame.Verify();
        }

        [Fact(DisplayName = "Tenta adicionar um Exame, pesquisa e o paciente está cadastrado")]
        public void AddExamePesquisaComPacienteCadastrado()
        {
            InstantiateMock();

            mockVirus.Setup(repo => repo.GetAll()).Returns(new List<VirusBacteriaModel>() { });
            mockArea.Setup(repo => repo.GetAll()).Returns(new List<AreaAtuacaoModel>() { });
            mockPessoa.Setup(repo => repo.GetByCpf(It.IsAny<string>())).Returns(GetExame().Paciente); // paciente existe
            mockUsuario.Setup(repo => repo.RetornLoggedUser(It.IsAny<ClaimsIdentity>())).Returns(GetUsuario());

            // mock do TempData e HttpContext com claims
            var httpContext = new DefaultHttpContext()
            {
                User = GetClaims()
            };

            var controller =
                   new ExameController(
                                               mockVirus.Object,
                                               mockExame.Object,
                                               mockPessoa.Object,
                                               mockMunicipio.Object,
                                               mockEstado.Object,
                                               mockConfig.Object,
                                               mockSituacao.Object,
                                               mockTrabalhaEstado.Object,
                                               mockTrabalhaMunicipio.Object,
                                               mockArea.Object,
                                               mockUsuario.Object

                                       )
                   {

                       //config mock claims
                       ControllerContext = new ControllerContext
                       {
                           HttpContext = httpContext
                       }
                   };

            var novoExame = GetExame();
            novoExame.PesquisarCpf = 1; //Pesquisa por cpf

            // Act
            var resultado = controller.Create(novoExame);
            // Assert
            var viewResult = Assert.IsType<ViewResult>(resultado);
            var model = Assert.IsType<ExameViewModel>(viewResult.ViewData.Model);
            Assert.Equal("ABRAAO ALVES", model.Paciente.Nome.ToUpper());
            Assert.Equal(1, model.Paciente.Idpessoa);
            mockExame.Verify();
        }

        [Fact(DisplayName = "Tenta adicionar um Exame, pesquisa e o paciente não está cadastrado")]
        public void AddExamePesquisaComPacienteNaoCadastrado()
        {
            InstantiateMock();

            mockVirus.Setup(repo => repo.GetAll()).Returns(new List<VirusBacteriaModel>() { });
            mockArea.Setup(repo => repo.GetAll()).Returns(new List<AreaAtuacaoModel>() { });
            mockPessoa.Setup(repo => repo.GetByCpf(It.IsAny<string>())).Returns(new PessoaModel {  }); // paciente não existe
            mockUsuario.Setup(repo => repo.RetornLoggedUser(It.IsAny<ClaimsIdentity>())).Returns(GetUsuario());

            // mock do TempData e HttpContext com claims
            var httpContext = new DefaultHttpContext()
            {
                User = GetClaims()
            };

            var controller =
                   new ExameController(
                                               mockVirus.Object,
                                               mockExame.Object,
                                               mockPessoa.Object,
                                               mockMunicipio.Object,
                                               mockEstado.Object,
                                               mockConfig.Object,
                                               mockSituacao.Object,
                                               mockTrabalhaEstado.Object,
                                               mockTrabalhaMunicipio.Object,
                                               mockArea.Object,
                                               mockUsuario.Object

                                       )
                   {

                       //config mock claims
                       ControllerContext = new ControllerContext
                       {
                           HttpContext = httpContext
                       },
                   };

            var novoExame = GetExame();
            novoExame.PesquisarCpf = 1; //Pesquisa por cpf

            // Act
            var resultado = controller.Create(novoExame);
            // Assert
            var viewResult = Assert.IsType<ViewResult>(resultado);
            var model = Assert.IsType<ExameViewModel>(viewResult.ViewData.Model);
            Assert.Null(model.Paciente.Nome);

            mockExame.Verify();
        }


        [Fact(DisplayName = "Tenta adicionar um Exame, mas modelo é invalido")]
        public void ErrorExameModeloInvalido()
        { 
            InstantiateMock();

            mockVirus.Setup(repo => repo.GetAll()).Returns(new List<VirusBacteriaModel>() { });
            mockArea.Setup(repo => repo.GetAll()).Returns(new List<AreaAtuacaoModel>() { });
            mockPessoa.Setup(repo => repo.GetByCpf(It.IsAny<string>())).Returns(new PessoaModel { }); // paciente não existe
            mockUsuario.Setup(repo => repo.RetornLoggedUser(It.IsAny<ClaimsIdentity>())).Returns(GetUsuario());

            // mock do TempData e HttpContext com claims
            var httpContext = new DefaultHttpContext()
            {
                User = GetClaims()
            };

            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            tempData["mensagemErro"] = "Error, modelo invalido";

            var controller =
                   new ExameController(
                                               mockVirus.Object,
                                               mockExame.Object,
                                               mockPessoa.Object,
                                               mockMunicipio.Object,
                                               mockEstado.Object,
                                               mockConfig.Object,
                                               mockSituacao.Object,
                                               mockTrabalhaEstado.Object,
                                               mockTrabalhaMunicipio.Object,
                                               mockArea.Object,
                                               mockUsuario.Object

                                       )
                   {

                       //config mock claims
                       ControllerContext = new ControllerContext
                       {
                           HttpContext = httpContext
                       },
                       TempData = tempData
                   };

            controller.ModelState.AddModelError("ExameModel", "Error");
            var novoExame = GetExame();

            // Act
            var resultado = controller.Create(novoExame) as ViewResult;
            // Assert
            Assert.IsType<ExameViewModel>(resultado.Model);
            Assert.Equal("Error, modelo invalido", resultado.TempData["mensagemErro"]);
        }


        private void InstantiateMock()
        {
            mockVirus = new Mock<IVirusBacteriaService>();
            mockExame = new Mock<IExameService>();
            mockPessoa = new Mock<IPessoaService>();
            mockMunicipio = new Mock<IMunicipioService>();
            mockEstado = new Mock<IEstadoService>();
            mockSituacao = new Mock<ISituacaoVirusBacteriaService>();
            mockTrabalhaEstado = new Mock<IPessoaTrabalhaEstadoService>();
            mockTrabalhaMunicipio = new Mock<IPessoaTrabalhaMunicipioService>();
            mockArea = new Mock<IAreaAtuacaoService>();
            mockUsuario = new Mock<IUsuarioService>();
            mockConfig = new Mock<IConfiguration>();
        }

        private ExameViewModel GetExame()
        {
            return new ExameViewModel
            {
                EmpresaExame = new EmpresaExameModel(),
                Paciente = new PessoaModel
                {
                    Idpessoa = 1,
                    Cpf = "07334824571",
                    Bairro = "Mamede Paes Mendonca",
                    Cep = "49500000",
                    Cidade = "Itabaiana",
                    Cns = "",
                    Complemento = "",
                    Nome = "Abraao Alves",
                    DataNascimento = new DateTime(1970, 6, 6),
                    Email = "abraao1515@gmail.com",
                    Estado = "Sergipe",
                    FoneCelular = "9999999999",
                    FoneFixo = "9999999999",
                    IdAreaAtuacao = 1,
                    Latitude = "-10.6821891",
                    Longitude = "-37.4379664",
                    Numero = "23",
                    Profissao = "Unknown",
                    Rua = "Percilio Andrade",
                    Sexo = "M",
                    Febre = false,
                    Coriza = false,
                    Diabetes = false,
                    Cancer = false,
                    Cardiopatia = false,
                    Diarreia = false,
                    DificuldadeRespiratoria = false,
                    DoencaRenal = false,
                    DoencaRespiratoria = false,
                    DorAbdominal = false,
                    Imunodeprimido = false,
                    Nausea = false,
                    Hipertenso = false,
                    DorGarganta = false,
                    DorOuvido = false,
                    Obeso = false,
                    OutrasComorbidades = "",
                    OutrosSintomas = "",
                    PerdaOlfatoPaladar = false,
                    Tosse = false,
                    SituacaoSaude = "",
                    Epilepsia = false,
                    DataObito = null
                },
                Exame = new ExameModel
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
                    StatusNotificacao = "N"

                }
            };

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

        private ClaimsPrincipal GetClaims()
        =>
             new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.SerialNumber, "1"),  // id usuario
                        new Claim(ClaimTypes.Name, "Abraao Alves"),
                        new Claim(ClaimTypes.StateOrProvince,"Sergipe"),
                        new Claim(ClaimTypes.Locality, "Itabaiana"),
                        new Claim(ClaimTypes.UserData,""),
                        new Claim(ClaimTypes.Email, "ab@gmail.com"),
                        new Claim(ClaimTypes.NameIdentifier, "1"),  // id pessoa
                        new Claim(ClaimTypes.Role, "ADM"),
                        new Claim(ClaimTypes.Dns, "Estado"),        // onde trabalha
                        new Claim(ClaimTypes.Sid, "UFS")            // empresa
                    }, "login"));

        private UsuarioViewModel GetUsuario()
        =>
            new UsuarioViewModel
            {
                UsuarioModel = new UsuarioModel
                {
                    Cpf = "07334824571",
                    IdPessoa = 1,
                    IdUsuario = 1,
                    TipoUsuario = 3
                }
            };

    }
}
