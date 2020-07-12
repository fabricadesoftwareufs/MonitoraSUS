using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using Model;
using Model.ViewModel;
using Moq;
using Persistence;
using Repository.Interfaces;
using Service;
using Service.Interface;
using System;
using Xunit;

namespace Test.Service
{
    public class ExameServiceTest
    {
        private Mock<monitorasusContext> _mockContext;
        private Mock<IExameRepository> _mockExameRepo;
        private Mock<IPessoaService> _mockPessoa;
        private Mock<ISituacaoVirusBacteriaService> _mockSituacao;

        [Fact(DisplayName = "Adiciona um Exame com sucesso e o paciente está cadastrado")]
        public void InserirExameComSucessoPacienteExiste()
        {
            // Configurando os mocks
            IniciarMocks();
            //_mockPessoa.Setup(mp => mp.GetByCpf(It.IsAny<string>())).Returns(GetExame().Paciente); // paciente existe
            //_mockExameRepo.Setup(x => x.Insert(GetExame())).Returns(true);

            var exameService = new ExameService(_mockContext.Object,
                _mockPessoa.Object, _mockSituacao.Object, _mockExameRepo.Object);
            var exame = GetExame();
            var result = exameService.Insert(exame);

            Assert.True(result);
        }

        // Metodos privados
        private void IniciarMocks()
        {
            _mockContext = new Mock<monitorasusContext>();
            _mockExameRepo = new Mock<IExameRepository>();
            _mockPessoa = new Mock<IPessoaService>();
            _mockSituacao = new Mock<ISituacaoVirusBacteriaService>();
        }
        private ExameViewModel GetExame()
            => new ExameViewModel
            {
                EmpresaExame = new EmpresaExameModel(),
                Paciente = new PessoaModel
                {
                    Idpessoa = 1,
                    Cpf = "06579861517",
                    Bairro = "Centro",
                    Cep = "49500154",
                    Cidade = "Itabaiana",
                    Cns = "",
                    Complemento = "",
                    Nome = "Gabriel Santana Cruz",
                    DataNascimento = new DateTime(1996, 10, 15),
                    Email = "gabriel.sistemasjr@gmail.com",
                    Estado = "Sergipe",
                    FoneCelular = "9999999999",
                    FoneFixo = "9999999999",
                    IdAreaAtuacao = 1,
                    Latitude = "-10.6821891",
                    Longitude = "-37.4379664",
                    Numero = "1390",
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

        private UsuarioViewModel GetUsuario()
            => new UsuarioViewModel
            {
                UsuarioModel = new UsuarioModel
                {
                    Cpf = "06579861517",
                    IdPessoa = 1,
                    IdUsuario = 1,
                    TipoUsuario = 3
                }
            };
    }
}
