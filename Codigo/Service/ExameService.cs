using Microsoft.AspNetCore.Http;
using Model;
using Model.AuxModel;
using Model.ViewModel;
using Newtonsoft.Json;
using Persistence;
using Repository.Interfaces;
using Service.Interface;
using Service.UnitiesOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Util;

namespace Service
{
    public class ExameService : IExameService
    {
        /**
         * Não retirei o contexto devido aos outros metodos.
         * CONTEXTO NÃO PODE ESTAR EXPOSTO!!!
         * Ideal usar apenas services, visto que o mesmo contexto será passado/obtido.
         * Caso necessite de transação mantendo o contexto, utilizar UNIDADES DE TRABALHO.
         */
        private readonly monitorasusContext _context;
        private readonly IPessoaService _pessoaService;
        private readonly IExameRepository _exameRepository;
        private readonly INotificacoesRepository _notificacaoRepository;
        private readonly IPessoaVirusEmpresaSituacaoMunicipioGeoTrabalhaMuniEstadoUnityOfWork _importUnidadeTrabalho;
        private readonly IPessoaExameSituacaoUnidadeTrabalhoUnityOfWork _pessoaExameSituacaoTrabalhaUnidadeTrabalho;

        // Unidades de trabalho
        private readonly IExameSituacaoPessoaUnityOfWork _exameSituacaoUnidadeTrabalho;

        public ExameService(monitorasusContext context,
            IPessoaService pessoaService,
            IExameSituacaoPessoaUnityOfWork exameSituacaoUnidadeTrabalho,
            IPessoaVirusEmpresaSituacaoMunicipioGeoTrabalhaMuniEstadoUnityOfWork importUnidadeTrabalho,
            IPessoaExameSituacaoUnidadeTrabalhoUnityOfWork pessoaExameSituacaoTrabalhaUnidadeTrabalho,
            IExameRepository exameRepository,
            INotificacoesRepository notificacaoRepository)
        {
            _context = context;
            _pessoaService = pessoaService;
            _exameRepository = exameRepository;
            _notificacaoRepository = notificacaoRepository;

            // Unidade de trabalho
            _exameSituacaoUnidadeTrabalho = exameSituacaoUnidadeTrabalho;
            _importUnidadeTrabalho = importUnidadeTrabalho;
            _pessoaExameSituacaoTrabalhaUnidadeTrabalho = pessoaExameSituacaoTrabalhaUnidadeTrabalho;
        }

        public bool Insert(ExameViewModel exameModel)
        {
            try
            {
                _exameSituacaoUnidadeTrabalho.BeginTransaction();
                if (_exameSituacaoUnidadeTrabalho.ExameRepositorio.GetExamesRelizadosData(exameModel.Paciente.Idpessoa, exameModel.Exame.IdVirusBacteria, exameModel.Exame.DataExame, exameModel.Exame.MetodoExame).Count > 0)
                    throw new ServiceException("Notificação DUPLICADA! Já existe um exame registrado desse paciente para esse Vírus/Bactéria na " +
                                                "data informada e método aplicado. Por favor, verifique se os dados da notificação estão corretos.");
                if (exameModel.Paciente.Idpessoa == 0)
                    exameModel.Paciente = _pessoaService.Insert(exameModel.Paciente);
                else
                    exameModel.Paciente = _pessoaService.Update(exameModel.Paciente, false);

                // Inserindo o resultado do exame (situacao da pessoa)
                var situacaoPessoa = _exameSituacaoUnidadeTrabalho.SituacaoPessoaService.GetById(exameModel.Paciente.Idpessoa, exameModel.Exame.IdVirusBacteria);
                if (situacaoPessoa == null)
                    _exameSituacaoUnidadeTrabalho.SituacaoPessoaService.Insert(CreateSituacaoPessoaModelByExame(exameModel, situacaoPessoa, _pessoaService));
                else
                    _exameSituacaoUnidadeTrabalho.SituacaoPessoaService.Update(CreateSituacaoPessoaModelByExame(exameModel, situacaoPessoa, _pessoaService));

                // Repositorio
                _exameSituacaoUnidadeTrabalho.ExameRepositorio.Insert(exameModel);
                // Caso dê tudo certo, commita.
                _exameSituacaoUnidadeTrabalho.Commit();
                return true;
            }
            catch (Exception e)
            {
                _exameSituacaoUnidadeTrabalho.Rollback();
                throw e;
            }
        }

        public bool Delete(int id) => _exameRepository.Delete(id);

        public bool Update(ExameViewModel exameModel)
        {
            try
            {
                // Abrindo transação.
                _pessoaExameSituacaoTrabalhaUnidadeTrabalho.BeginTransaction();
                exameModel.Exame.IdAgenteSaude = exameModel.Usuario.IdPessoa;
                var usuarioDuplicado = _pessoaExameSituacaoTrabalhaUnidadeTrabalho.PessoaService.GetByCpf(exameModel.Paciente.Cpf);
                if (usuarioDuplicado != null)
                {
                    if (!(usuarioDuplicado.Idpessoa == exameModel.Paciente.Idpessoa))
                        throw new ServiceException("Já existe um paciente com esse CPF/RG, tente novamente!");
                }

                var examesRealizados = _pessoaExameSituacaoTrabalhaUnidadeTrabalho.ExameRepository.GetExamesRelizadosData(exameModel.Paciente.Idpessoa, exameModel.Exame.IdVirusBacteria, exameModel.Exame.DataExame, exameModel.Exame.MetodoExame);
                if (examesRealizados.Count > 0)
                {
                    var exame = examesRealizados.FirstOrDefault();
                    if (exame.IdExame != exameModel.Exame.IdExame)
                        throw new ServiceException("Notificação DUPLICADA! Já existe um exame registrado desse paciente para esse Vírus/Bactéria na " +
                                                        "data informada. Por favor, verifique se os dados da notificação estão corretos.");
                }

                var situacao = _pessoaExameSituacaoTrabalhaUnidadeTrabalho.SituacaoPessoaService.GetById(exameModel.Paciente.Idpessoa, exameModel.Exame.IdVirusBacteria);
                if (situacao == null)
                    _pessoaExameSituacaoTrabalhaUnidadeTrabalho.SituacaoPessoaService.Insert(CreateSituacaoPessoaModelByExame(exameModel, situacao, _pessoaService));
                else
                    _pessoaExameSituacaoTrabalhaUnidadeTrabalho.SituacaoPessoaService.Update(CreateSituacaoPessoaModelByExame(exameModel, situacao, _pessoaService));

                _pessoaExameSituacaoTrabalhaUnidadeTrabalho.PessoaService.Update(CreatePessoaModelByExame(exameModel, _pessoaService), false);

                _pessoaExameSituacaoTrabalhaUnidadeTrabalho.ExameRepository.Update(exameModel);

                // Commitando
                _pessoaExameSituacaoTrabalhaUnidadeTrabalho.Commit();
                return true;

            }
            catch (Exception e)
            {
                _pessoaExameSituacaoTrabalhaUnidadeTrabalho.Rollback();
                throw e;
            }
        }

        public ExameModel GetByIdColeta(string codigoColeta) => _exameRepository.GetByIdColeta(codigoColeta);

        public ExameViewModel GetById(int id) => _exameRepository.GetById(id);

        public List<ExameBuscaModel> GetByIdAgente(int idAgente, DateTime dataInicio, DateTime dataFim) => _exameRepository.GetByIdAgente(idAgente, dataInicio, dataFim);

        public List<ExameBuscaModel> GetByIdAgente(int idAgente, int lastRecord) => _exameRepository.GetByIdAgente(idAgente, lastRecord);

        public List<ExameBuscaModel> GetByIdEmpresa(int idEmpresa, DateTime dataInicio, DateTime dataFim) => _exameRepository.GetByIdEmpresa(idEmpresa, dataInicio, dataFim);

        public List<ExameBuscaModel> GetByIdEmpresa(int idEmpresa, int lastRecord) => _exameRepository.GetByIdEmpresa(idEmpresa, lastRecord);

        public List<ExameBuscaModel> GetByIdMunicipio(int idMunicicpio, DateTime dataInicio, DateTime dataFim) => _exameRepository.GetByIdMunicipio(idMunicicpio, dataInicio, dataFim);

        public List<ExameBuscaModel> GetByIdMunicipio(int idMunicicpio, int lastRecord) => _exameRepository.GetByIdMunicipio(idMunicicpio, lastRecord);

        public List<ExameBuscaModel> GetByIdEstado(int idEstado, DateTime dataInicio, DateTime dataFim) => _exameRepository.GetByIdEstado(idEstado, dataInicio, dataFim);

        public List<ExameBuscaModel> GetByIdEstado(int idEstado, int lastRecord) => _exameRepository.GetByIdEstado(idEstado, lastRecord);

        public List<ExameBuscaModel> GetByIdPaciente(int idPaciente) => _exameRepository.GetByIdPaciente(idPaciente);

        public ConfiguracaoNotificarModel BuscarConfiguracaoNotificar(int IdEstado, int IdEmpresaExame) => _notificacaoRepository.BuscarConfiguracaoNotificar(IdEstado, IdEmpresaExame);

        public ConfiguracaoNotificarModel BuscarConfiguracaoNotificar(int IdMunicipio) => _notificacaoRepository.BuscarConfiguracaoNotificar(IdMunicipio);

        public async Task<ExameModel> EnviarSMSResultadoExameAsync(ConfiguracaoNotificarModel configuracaoNotificar, ExameModel exame, PessoaModel pessoa)
        {
            try
            {
                string mensagem = "[MonitoraSUS]Olá, " + pessoa.Nome + ". ";
                if (exame.Resultado.Equals(ExameModel.RESULTADO_POSITIVO))
                    mensagem += configuracaoNotificar.MensagemPositivo;
                else if (exame.Resultado.Equals(ExameModel.RESULTADO_NEGATIVO))
                    mensagem += configuracaoNotificar.MensagemNegativo;
                else if (exame.Resultado.Equals(ExameModel.RESULTADO_RECUPERADO))
                    mensagem += configuracaoNotificar.MensagemCurado;
                else if (exame.Resultado.Equals(ExameModel.RESULTADO_INDETERMINADO))
                    mensagem += configuracaoNotificar.MensagemIndeterminado;
                else if (exame.Resultado.Equals(ExameModel.RESULTADO_AGUARDANDO))
                    return exame;
                else if (exame.Resultado.Equals(ExameModel.RESULTADO_IGMIGG))
                    return exame;

                var cliente = new HttpClient();
                string url = "https://api.smsdev.com.br/send?key=" + configuracaoNotificar.Token + "&type=9&";
                var uri = url + "number=" + pessoa.FoneCelular + "&msg=" + mensagem;
                var resultadoEnvio = await cliente.GetStringAsync(uri);
                var jsonResponse = JsonConvert.DeserializeObject<ResponseSMSModel>(resultadoEnvio);
                exame.IdNotificacao = jsonResponse.Id.ToString();
                exame.StatusNotificacao = ExameModel.NOTIFICADO_ENVIADO;

                // Atualizando via repositorio.
                _exameRepository.Update(exame);

                var configura = _notificacaoRepository.Configura(configuracaoNotificar);
                if (configura != null)
                {
                    configura.QuantidadeSmsdisponivel -= 1;
                    _notificacaoRepository.Update(configura);
                }
                return exame;
            }
            catch (HttpRequestException)
            {
                return exame;
            }
        }

        public async Task<ExameModel> ConsultarSMSExameAsync(ConfiguracaoNotificarModel configuracaoNotificar, ExameModel exame)
        {
            try
            {
                var cliente = new HttpClient();
                string url = "https://api.smsdev.com.br/get?key=" + configuracaoNotificar.Token + "&action=status&";
                var uri = url + "id=" + exame.IdNotificacao;
                var resultadoEnvio = await cliente.GetStringAsync(uri);

                var jsonResponse = JsonConvert.DeserializeObject<ConsultaSMSModel>(resultadoEnvio);
                if (jsonResponse.Descricao.Equals("RECEBIDA"))
                {
                    exame.StatusNotificacao = ExameModel.NOTIFICADO_SIM;
                    _exameRepository.Update(exame);
                }
                else if (jsonResponse.Descricao.Equals("ERRO"))
                {
                    exame.StatusNotificacao = ExameModel.NOTIFICADO_PROBLEMAS;
                    _exameRepository.Update(exame);
                }
            }
            catch (HttpRequestException)
            {
                return exame;
            }
            return exame;
        }

        public List<MonitoraPacienteViewModel> GetByHospital(int idEmpresa, int idVirusBacteria, DateTime dataInicio, DateTime dataFim) => _exameRepository.GetByHospital(idEmpresa, idVirusBacteria, dataInicio, dataFim);

        public List<MonitoraPacienteViewModel> GetByCidadeResidenciaPaciente(string cidade, string siglaEstado,
            int idVirusBacteria, DateTime dataInicio, DateTime dataFim) => _exameRepository.GetByCidadeResidenciaPaciente(cidade, siglaEstado, idVirusBacteria, dataInicio, dataFim);

        public List<MonitoraPacienteViewModel> GetByEstadoResidenciaPaciente(string siglaEstado,
            int idVirusBacteria, DateTime dataInicio, DateTime dataFim) => _exameRepository.GetByEstadoResidenciaPaciente(siglaEstado, idVirusBacteria, dataInicio, dataFim);

        public List<ExameModel> GetExamesRelizadosData(int idPaciente, int idVirusBacteria, DateTime dateExame, string metodoExame) => _exameRepository.GetExamesRelizadosData(idPaciente, idVirusBacteria, dateExame, metodoExame);

        public List<TotalEstadoMunicipioBairro> GetTotaisPopulacaoByEstado(string siglaEstado) => _exameRepository.GetTotaisPopulacaoByEstado(siglaEstado);

        public List<TotalEstadoMunicipioBairro> GetTotaisPopulacaoByMunicipio(string siglaEstado, string cidade) => _exameRepository.GetTotaisPopulacaoByMunicipio(siglaEstado, cidade);

        public List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByEstado(int idEstado) => _exameRepository.GetTotaisRealizadosByEstado(idEstado);

        public List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByMunicipio(int idMunicipio) => _exameRepository.GetTotaisRealizadosByMunicipio(idMunicipio);

        public List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByEmpresa(int idEmpresa) => _exameRepository.GetTotaisRealizadosByEmpresa(idEmpresa);

        public void Import(IFormFile file, UsuarioViewModel agente)
        {
            var secretarioMunicipio = _importUnidadeTrabalho.PessoaTrabalhaMunicipioService.GetByIdPessoa(agente.UsuarioModel.IdPessoa);
            var secretarioEstado = _importUnidadeTrabalho.PessoaTrabalhaEstadoContext.GetByIdPessoa(agente.UsuarioModel.IdPessoa);
            var exames = new List<ExameViewModel>();
            var indices = new IndiceItemArquivoImportacao();
            var listVirusBacteria = _importUnidadeTrabalho.VirusBacteriaService.GetAll();
            MunicipioGeoModel cidadePaciente = new MunicipioGeoModel(),
                              cidadeEmpresa = new MunicipioGeoModel();


            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                indices = IndexaColunasArquivoImportacao(reader.ReadLine());

                if (indices == null)
                    throw new ServiceException("Essa planilha não possui as informações necessárias para fazer a importação, " +
                                                        "por favor verifique a planilha e tente novamente.");

                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLine().Split(';');

                    cidadePaciente = _importUnidadeTrabalho.MunicipioGeoService.GetByName(line[53]);
                    cidadeEmpresa = _importUnidadeTrabalho.MunicipioGeoService.GetByName(line[7]);

                    var exame = new ExameViewModel();
                    exame.Paciente = new PessoaModel
                    {
                        Nome = line[indices.IndiceNomePaciente],
                        Cidade = line[indices.IndiceCidadePaciente],
                        Cpf = line[indices.IndiceTipoDocumento1Paciente].Equals("CPF") && Methods.ValidarCpf(line[indices.IndiceDocumento1Paciente]) ?
                                Methods.RemoveSpecialsCaracts(line[indices.IndiceDocumento1Paciente]) : line[indices.IndiceTipoDocumento2Paciente].Equals("CPF") && Methods.ValidarCpf(line[indices.IndiceDocumento2Paciente]) ?
                                Methods.RemoveSpecialsCaracts(line[indices.IndiceDocumento2Paciente]) : "",

                        Sexo = line[indices.IndiceSexoPaciente].Equals("FEMININO") ? "Feminino" : "Masculino",
                        Cep = line[indices.IndiceCepPaciente].Length > 0 ? Methods.RemoveSpecialsCaracts(line[indices.IndiceCepPaciente]) : "00000000",
                        Rua = line[indices.IndiceRuaPaciente].Length > 0 ? line[indices.IndiceRuaPaciente].Split('-')[0] : "NÃO INFORMADO",
                        Bairro = line[indices.IndiceBairroPaciente].Length > 0 ? line[indices.IndiceBairroPaciente] : "NAO INFORMADO",
                        Estado = line[indices.IndiceEstadoPaciente],
                        Numero = line[indices.IndiceRuaPaciente].Length > 0 && line[indices.IndiceRuaPaciente].Split('-').Length >= 2 ? line[indices.IndiceRuaPaciente].Split('-')[1].Trim() : "",
                        Complemento = line[indices.IndiceRuaPaciente].Length > 0 && line[indices.IndiceRuaPaciente].Split('-').Length == 3 ? line[indices.IndiceRuaPaciente].Split('-')[2].Trim() : "",
                        FoneCelular = line[indices.IndiceFoneCelularPaciente],
                        DataNascimento = Convert.ToDateTime(line[indices.IndiceDataNascimentoPaciente]),
                        IdAreaAtuacao = 0,
                        Longitude = cidadePaciente != null ? cidadePaciente.Longitude.ToString() : "0",
                        Latitude = cidadePaciente != null ? cidadePaciente.Latitude.ToString() : "0",
                        Cns = line[indices.IndiceCnsPaciente],
                        Profissao = "NÃO INFORMADA",
                        OutrasComorbidades = "",
                        OutrosSintomas = ""
                    };

                    exame.Exame.IdVirusBacteria = GetIdVirusBacteriaItemImportacao(line[indices.IndiceTipoExame], listVirusBacteria);
                    exame.Exame.IdAgenteSaude = agente.UsuarioModel.IdPessoa;
                    exame.Exame.DataExame = Convert.ToDateTime(line[indices.IndiceDataExame]);
                    exame.Exame.DataInicioSintomas = line[indices.IndiceDataInicioSintomas].Equals("") ? Convert.ToDateTime(line[indices.IndiceDataExame]) : Convert.ToDateTime(line[indices.IndiceDataInicioSintomas]);
                    exame.Exame.IgG = line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && !line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) ? GetMetodoExame(line[indices.IndiceMetodoExame], IndiceItemArquivoImportacao.METODO_IGG, line[indices.IndiceResultadoExame].Length > 0 ? line[indices.IndiceResultadoExame] : line[indices.IndiceObservacaoExame]) : "N";
                    exame.Exame.IgM = line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) && !line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) ? GetMetodoExame(line[indices.IndiceMetodoExame], IndiceItemArquivoImportacao.METODO_IGM, line[indices.IndiceResultadoExame].Length > 0 ? line[indices.IndiceResultadoExame] : line[indices.IndiceObservacaoExame]) : "N";
                    exame.Exame.Pcr = line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_PCR) ? GetMetodoExame(line[indices.IndiceMetodoExame], IndiceItemArquivoImportacao.METODO_PCR, line[indices.IndiceResultadoExame].Length > 0 ? line[indices.IndiceResultadoExame] : line[indices.IndiceObservacaoExame]) : "N";
                    exame.Exame.IgGIgM = line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) ? GetMetodoExame(line[indices.IndiceMetodoExame], IndiceItemArquivoImportacao.METODO_IGG_IGM, line[indices.IndiceResultadoExame].Length > 0 ? line[indices.IndiceResultadoExame] : line[indices.IndiceObservacaoExame]) : "N";
                    if (secretarioMunicipio != null)
                        exame.Exame.IdMunicipio = secretarioMunicipio.IdMunicipio;
                    else
                        exame.Exame.IdMunicipio = null;
                    exame.Exame.IdEstado = secretarioMunicipio != null ? Convert.ToInt32(_importUnidadeTrabalho.MunicipioService.GetById(secretarioMunicipio.IdMunicipio).Uf) : secretarioEstado.IdEstado;
                    exame.EmpresaExame = new EmpresaExameModel
                    {
                        Cnpj = "NÃO INFORMADO",
                        Nome = line[indices.IndiceNomeEmpresa],
                        Cnes = line[indices.IndiceCnesEmpresa],
                        Cidade = line[indices.IndiceCidadeEmpresa],
                        Latitude = cidadeEmpresa != null ? cidadeEmpresa.Latitude.ToString() : "0",
                        Longitude = cidadeEmpresa != null ? cidadeEmpresa.Longitude.ToString() : "0",
                        Estado = line[indices.IndiceEstadoEmpresa],
                        Rua = "NÃO INFORMADO",
                        Bairro = "NÃO INFORMADO",
                        Cep = "00000000",
                        FoneCelular = "00000000000",
                    };
                    exame.Exame.IdAreaAtuacao = 0;
                    exame.Exame.CodigoColeta = line[indices.IndiceCodigoColeta];
                    exame.Paciente.Cns = line[indices.IndiceCnsPaciente];

                    exames.Add(exame);
                }
            }

            foreach (var item in exames)
            {
                var pessoa = !string.IsNullOrWhiteSpace(item.Paciente.Cpf) ?
                           _importUnidadeTrabalho.PessoaService.GetByCpf(item.Paciente.Cpf) : !string.IsNullOrWhiteSpace(item.Paciente.Cns) ?
                           _importUnidadeTrabalho.PessoaService.GetByCns(item.Paciente.Cns) : new PessoaModel { Idpessoa = -1 };

                if (pessoa == null || pessoa.Idpessoa == -1)
                {
                    pessoa = _importUnidadeTrabalho.PessoaService.Insert(item.Paciente);
                    item.Paciente.Idpessoa = pessoa.Idpessoa;
                    item.Paciente.Cpf = pessoa.Cpf;
                }
                else
                {
                    item.Paciente.Idpessoa = pessoa.Idpessoa;
                    item.Paciente.Cpf = pessoa.Cpf;
                    _importUnidadeTrabalho.PessoaService.Update(item.Paciente, true);
                }


                var empresa = _importUnidadeTrabalho.EmpresaExameService.GetByCNES(item.EmpresaExame.Cnes);

                if (empresa == null)
                    _importUnidadeTrabalho.EmpresaExameService.Insert(item.EmpresaExame);

                item.Exame.IdEmpresaSaude = _importUnidadeTrabalho.EmpresaExameService.GetByCNES(item.EmpresaExame.Cnes).Id;

                var situacaoPessoa = _importUnidadeTrabalho.SituacaoPessoaService.GetById(item.Paciente.Idpessoa, item.Exame.IdVirusBacteria);

                if (situacaoPessoa == null)
                    _importUnidadeTrabalho.SituacaoPessoaService.Insert(CreateSituacaoPessoaModelByExame(item, situacaoPessoa, _pessoaService));
                else
                    _importUnidadeTrabalho.SituacaoPessoaService.Update(CreateSituacaoPessoaModelByExame(item, situacaoPessoa, _pessoaService));


                var exame = _importUnidadeTrabalho.ExameRepository.GetByIdColeta(item.Exame.CodigoColeta);

                var ex = new ExameModel
                {
                    IdExame = exame != null ? exame.IdExame : 0,
                    IdPaciente = item.Paciente.Idpessoa,
                    IdVirusBacteria = item.Exame.IdVirusBacteria,
                    IdAgenteSaude = item.Exame.IdAgenteSaude,
                    DataExame = item.Exame.DataExame,
                    DataInicioSintomas = item.Exame.DataInicioSintomas,
                    IgG = item.Exame.IgG,
                    IgM = item.Exame.IgM,
                    Pcr = item.Exame.Pcr,
                    IgGIgM = item.Exame.IgGIgM,
                    IdMunicipio = item.Exame.IdMunicipio,
                    IdEstado = item.Exame.IdEstado,
                    IdEmpresaSaude = item.Exame.IdEmpresaSaude,
                    IdAreaAtuacao = item.Exame.IdAreaAtuacao,
                    CodigoColeta = item.Exame.CodigoColeta,
                    IdNotificacao = "",
                    OutrosSintomas = "",
                    MetodoExame = "F",
                    StatusNotificacao = exame != null ? exame.StatusNotificacao : "N"
                };

                var check = _importUnidadeTrabalho.ExameRepository.GetExamesRelizadosData(ex.IdPaciente, item.Exame.IdVirusBacteria, item.Exame.DataExame, item.Exame.MetodoExame);
                if (exame != null)
                {
                    if (check.Count > 0)
                    {
                        var status = false;
                        foreach (var index in check)
                            if (index.IdExame == exame.IdExame)
                                status = true;

                        if (status)
                            _importUnidadeTrabalho.ExameRepository.Update(ex);
                    }
                }
                else
                {
                    if (check.Count == 0)
                        _importUnidadeTrabalho.ExameRepository.Update(ex);
                }
            }
        }


        // ============ METODOS PRIVADOS ============
        private IndiceItemArquivoImportacao IndexaColunasArquivoImportacao(string cabecalho)
        {

            var itens = cabecalho.Split(';');

            var indices = new IndiceItemArquivoImportacao();
            for (int i = 0; i < itens.Length; i++)
            {
                if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.UNIDADE_SOLICITANTE)))
                    indices.IndiceNomeEmpresa = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CNES_UNIDADE_SOLCITANTE).ToUpper()))
                    indices.IndiceCnesEmpresa = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.MUNICIPIO_DO_SOLICITANTE).ToUpper()))
                    indices.IndiceCidadeEmpresa = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.ESTADO_DO_SOLICITANTE).ToUpper()))
                    indices.IndiceEstadoEmpresa = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CNS_DO_PACIENTE).ToUpper()))
                    indices.IndiceCnsPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.NOME_PACIENTE).ToUpper()))
                    indices.IndiceNomePaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.SEXO_PACIENTE).ToUpper()))
                    indices.IndiceSexoPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DATA_DE_NASCIMENTO_PACIENTE).ToUpper()))
                    indices.IndiceDataNascimentoPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.TIPO_DOCUMENTO_1).ToUpper()))
                    indices.IndiceTipoDocumento1Paciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DOCUMENTO_1).ToUpper()))
                    indices.IndiceDocumento1Paciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.TIPO_DOCUMENTO_2).ToUpper()))
                    indices.IndiceTipoDocumento2Paciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DOCUMENTO_2).ToUpper()))
                    indices.IndiceDocumento2Paciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.ENDERECO_PACIENTE).ToUpper()))
                    indices.IndiceRuaPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.BAIRRO_PACIENTE).ToUpper()))
                    indices.IndiceBairroPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CEP_PACIENTE).ToUpper()))
                    indices.IndiceCepPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.MUNICIPIO_PACIENTE).ToUpper()))
                    indices.IndiceCidadePaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.ESTADO_PACIENTE).ToUpper()))
                    indices.IndiceEstadoPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CELULAR_PACIENTE).ToUpper()))
                    indices.IndiceFoneCelularPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.TIPO_EXAME).ToUpper()))
                    indices.IndiceTipoExame = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.METODO_EXAME).ToUpper()))
                    indices.IndiceMetodoExame = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CODIGO_DA_AMOSTRA).ToUpper()))
                    indices.IndiceCodigoColeta = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DATA_DA_COLETA).ToUpper()))
                    indices.IndiceDataExame = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DATA_INICIO_SINTOMAS).ToUpper()))
                    indices.IndiceDataInicioSintomas = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.OBSERVACOES_RESULTADO).ToUpper()))
                    indices.IndiceObservacaoExame = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO).ToUpper()))
                    indices.IndiceResultadoExame = i;
            }

            var planilhaValida = false;
            if (indices.IndiceNomeEmpresa != -1 && indices.IndiceCnesEmpresa != -1 && indices.IndiceCidadeEmpresa != -1 && indices.IndiceEstadoEmpresa != -1 && indices.IndiceFoneCelularPaciente != -1 &&
                indices.IndiceCnsPaciente != -1 && indices.IndiceNomePaciente != -1 && indices.IndiceDataNascimentoPaciente != -1 && indices.IndiceTipoDocumento1Paciente != -1 && indices.IndiceDocumento1Paciente != -1 &&
                indices.IndiceTipoDocumento2Paciente != -1 && indices.IndiceDocumento2Paciente != -1 && indices.IndiceRuaPaciente != -1 && indices.IndiceBairroPaciente != -1 && indices.IndiceCepPaciente != -1 &&
                indices.IndiceCidadePaciente != -1 && indices.IndiceEstadoPaciente != -1 && indices.IndiceTipoExame != -1 && indices.IndiceMetodoExame != -1 && indices.IndiceCodigoColeta != -1 && indices.IndiceDataExame != -1 &&
                indices.IndiceDataInicioSintomas != -1 && indices.IndiceResultadoExame != -1 && indices.IndiceObservacaoExame != -1 && indices.IndiceSexoPaciente != -1)
                planilhaValida = true;

            return planilhaValida ? indices : null;
        }

        private SituacaoPessoaVirusBacteriaModel CreateSituacaoPessoaModelByExame(ExameViewModel exame, SituacaoPessoaVirusBacteriaModel situacao, IPessoaService pessoaService)
        {
            if (situacao != null)
                situacao.UltimaSituacaoSaude = exame.Exame.ResultadoStatus;
            else
            {
                situacao = new SituacaoPessoaVirusBacteriaModel();
                situacao.IdVirusBacteria = exame.Exame.IdVirusBacteria;
                situacao.Idpessoa = pessoaService.GetByCpf(Methods.RemoveSpecialsCaracts(exame.Paciente.Cpf)).Idpessoa;
                situacao.UltimaSituacaoSaude = exame.Exame.ResultadoStatus;
                situacao.DataUltimoMonitoramento = null;
            }

            return situacao;
        }

        private string GetMetodoExame(string exame, string metodo, string resultado)
        {
            switch (metodo)
            {
                case "PCR":
                    if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_PCR))
                        return "S";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_NAO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_PCR))
                        return "N";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_SOLICITAR_NOVA_COLETA)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_PCR))
                        return "I";
                    else
                        return "I";

                case "IGG":
                    if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "S";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_NAO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "N";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_SOLICITAR_NOVA_COLETA)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "I";
                    else
                        return "I";

                case "IGM":
                    if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG))
                        return "S";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_NAO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG))
                        return "N";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_SOLICITAR_NOVA_COLETA)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG))
                        return "I";
                    else
                        return "I";

                case "IGG/IGM":
                    if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "S";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_NAO_DETECTAVEL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "N";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_SOLICITAR_NOVA_COLETA)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "I";
                    else
                        return "I";

                default:
                    return "I";
            }
        }

        private int GetIdVirusBacteriaItemImportacao(string exame, List<VirusBacteriaModel> virus)
        {
            string[] e = exame.Split(',');

            foreach (var item in virus)
            {
                if (item.Nome.ToUpper().Contains(e[0]))
                {
                    return item.IdVirusBacteria;
                }
            }

            return virus[0].IdVirusBacteria;
        }

        private PessoaModel CreatePessoaModelByExame(ExameViewModel exameViewModel, IPessoaService pessoaService)
        {
            var user = pessoaService.GetByCpf(exameViewModel.Paciente.Cpf.ToUpper());
            if (user == null)
            {
                exameViewModel.Paciente.Idpessoa = 0;
                exameViewModel.Paciente.IdAreaAtuacao = exameViewModel.Exame.IdAreaAtuacao;
                if (exameViewModel.Exame.AguardandoResultado == true || exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_POSITIVO) ||
                    exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_IGMIGG))
                    exameViewModel.Paciente.SituacaoSaude = PessoaModel.SITUACAO_ISOLAMENTO;
            }
            else
            {
                exameViewModel.Paciente.Idpessoa = user.Idpessoa;
                exameViewModel.Paciente.IdAreaAtuacao = exameViewModel.Exame.IdAreaAtuacao;
                if ((exameViewModel.Paciente.SituacaoSaude.Equals(PessoaModel.SITUACAO_SAUDAVEL) & !exameViewModel.Paciente.SituacaoSaude.Equals(PessoaModel.SITUACAO_SAUDAVEL) && exameViewModel.Exame.AguardandoResultado == false)
                    || (exameViewModel.Paciente.SituacaoSaude.Equals(PessoaModel.SITUACAO_SAUDAVEL) && exameViewModel.Exame.AguardandoResultado == true))
                {
                    exameViewModel.Paciente.SituacaoSaude = PessoaModel.SITUACAO_ISOLAMENTO;
                }
                else if (exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_NEGATIVO) || exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_RECUPERADO)
                  || exameViewModel.Exame.Resultado.Equals(ExameModel.RESULTADO_INDETERMINADO))
                {
                    var virus = (new VirusBacteriaService(_context)).GetById(exameViewModel.Exame.IdVirusBacteria);
                    DateTime dataMinima = DateTime.Now.AddDays(virus.DiasRecuperacao * (-1));
                    var exames = GetByIdPaciente(user.Idpessoa).Where(e => e.Exame.DataExame >= dataMinima).ToList();
                    if (exames.Count() <= 1 && exameViewModel.Paciente.SituacaoSaude.Equals(PessoaModel.SITUACAO_ISOLAMENTO))
                    {
                        exameViewModel.Paciente.SituacaoSaude = PessoaModel.SITUACAO_SAUDAVEL;
                    }
                }
            }
            return exameViewModel.Paciente;
        }
    }
}
