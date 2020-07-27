using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Model;
using Model.ViewModel;
using Moq;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace MonitoraSUS.Controllers.Test
{
    public class ExameControllerTest
    {
        private Mock<IExameService> mockExameService;
        private Mock<IPessoaService> mockPessoaService;
        private ExameController controller;
        
        private void SetUp()
        {
            var mockVirusService = new Mock<IVirusBacteriaService>();
            mockExameService = new Mock<IExameService>();
            mockPessoaService = new Mock<IPessoaService>();
            var mockMunicipioService = new Mock<IMunicipioService>();
            var mockEstadoService = new Mock<IEstadoService>();
            var mockSituacaoService = new Mock<ISituacaoVirusBacteriaService>();
            var mockTrabalhaEstadoService = new Mock<IPessoaTrabalhaEstadoService>();
            var mockTrabalhaMunicipioService = new Mock<IPessoaTrabalhaMunicipioService>();
            var mockAreaAtuacaoService = new Mock<IAreaAtuacaoService>();
            var mockUsuarioService = new Mock<IUsuarioService>();
            var mockSmsService = new Mock<ISmsService>();
            var mockImportarExameService = new Mock<IImportarExameService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockVirusService.Setup(repo => repo.GetAll()).Returns(new List<VirusBacteriaModel>() { });
            mockAreaAtuacaoService.Setup(repo => repo.GetAll()).Returns(new List<AreaAtuacaoModel>() { });
            mockUsuarioService.Setup(repo => repo.RetornLoggedUser(It.IsAny<ClaimsIdentity>())).Returns(GetUsuario());
            controller = new ExameController(
                                                 mockVirusService.Object,
                                                 mockExameService.Object,
                                                 mockPessoaService.Object,
                                                 mockMunicipioService.Object,
                                                 mockEstadoService.Object,
                                                 mockConfiguration.Object,
                                                 mockSituacaoService.Object,
                                                 mockTrabalhaEstadoService.Object,
                                                 mockTrabalhaMunicipioService.Object,
                                                 mockAreaAtuacaoService.Object,
                                                 mockUsuarioService.Object,
                                                 mockSmsService.Object,
                                                 mockImportarExameService.Object
                                        );
            DefaultHttpContext httpContext = new DefaultHttpContext()
            {
                User = GetClaims()
            };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;
        }

        [Fact(DisplayName = "Adiciona um Exame com sucesso e o paciente não está cadastrado")]
        public void AddExameComSucesso()
        {
            SetUp();
            
            //Arrange
            mockExameService.Setup(exameService => exameService.Insert(It.IsAny<ExameViewModel>())).Verifiable();

            // Act
            var resultado = controller.Create(GetExame());
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Create", redirectToActionResult.ActionName);
            mockExameService.Verify();
        }

        [Fact(DisplayName = "Tenta adicionar um Exame, pesquisa e o paciente está cadastrado")]
        public void AddExamePesquisaComPacienteCadastrado()
        {
            SetUp();
            
            //Arrange
            mockPessoaService.Setup(repo => repo.GetByCpf(It.IsAny<string>())).Returns(GetExame().Paciente); // paciente existe
            var novoExame = GetExame();
            novoExame.PesquisarCpf = 1; //Pesquisa por cpf
            // Act
            var resultado = controller.Create(novoExame);
            // Assert
            var viewResult = Assert.IsType<ViewResult>(resultado);
            var model = Assert.IsType<ExameViewModel>(viewResult.ViewData.Model);
            Assert.Equal("ABRAAO ALVES", model.Paciente.Nome.ToUpper());
            Assert.Equal(1, model.Paciente.Idpessoa);
            mockExameService.Verify();
        }

        [Fact(DisplayName = "Tenta adicionar um Exame, pesquisa e o paciente não está cadastrado")]
        public void AddExamePesquisaComPacienteNaoCadastrado()
        {
            SetUp();
            
            //Arrange
            mockPessoaService.Setup(repo => repo.GetByCpf(It.IsAny<string>())).Returns(new PessoaModel { }); // paciente não existe
            var novoExame = GetExame();
            novoExame.PesquisarCpf = 1; //Pesquisa por cpf

            // Act
            var resultado = controller.Create(novoExame);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(resultado);
            var model = Assert.IsType<ExameViewModel>(viewResult.ViewData.Model);
            Assert.Null(model.Paciente.Nome);
            mockExameService.Verify();
        }


        [Fact(DisplayName = "Tenta adicionar um Exame, mas modelo é invalido")]
        public void ErrorExameModeloInvalido()
        {
            SetUp();
            //Arrange
            mockPessoaService.Setup(repo => repo.GetByCpf(It.IsAny<string>())).Returns(new PessoaModel { }); // paciente não existe

            var novoExame = GetExame();
            novoExame.Exame = null;
            novoExame.Paciente = null;
            controller.ModelState.AddModelError("erro", "Modelo invalido");
            
            // Act
            var resultado = controller.Create(novoExame);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(resultado);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
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
