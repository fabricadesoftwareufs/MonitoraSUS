using Microsoft.AspNetCore.Http;
using Model;
using Model.AuxModel;
using Model.ViewModel;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Util;

namespace Service
{
    public class ImportarExameService : IImportarExameService
    {

        private readonly monitorasusContext _context;

        public ImportarExameService(monitorasusContext context)
        {
            _context = context;
        }

        public void Import(IFormFile file, UsuarioViewModel agente)
        {
            var _pessoaTrabalhaMunicipioService = new PessoaTrabalhaMunicipioService(_context);
            var _pessoaTrabalhaEstadoContext = new PessoaTrabalhaEstadoService(_context);
            var _municipioGeoService = new MunicipioGeoService(_context);
            var _virusBacteriaService = new VirusBacteriaService(_context);
            var _pessoaService = new PessoaService(_context);
            var _empresaExameService = new EmpresaExameService(_context);
            var _situacaoPessoaService = new SituacaoVirusBacteriaService(_context);
            var _municipioService = new MunicipioService(_context);
            var _estadoService = new EstadoService(_context);
            var _exameService = new ExameService(_context);
            var trabalhaMunicipio = _pessoaTrabalhaMunicipioService.GetByIdPessoa(agente.UsuarioModel.IdPessoa);
            var trabalhaEstado = _pessoaTrabalhaEstadoContext.GetByIdPessoa(agente.UsuarioModel.IdPessoa);
            var examesPlanilha = new List<ExameViewModel>();
            var indices = new IndiceItemArquivoImportacao();
            var listVirusBacteria = _virusBacteriaService.GetAll();
            string[] line = { };
            Dictionary<string, MunicipioGeoModel> mapCidade = new Dictionary<string, MunicipioGeoModel>();
            Dictionary<string, EstadoModel> mapEstado = new Dictionary<string, EstadoModel>();
            Dictionary<string, EstadoModel> mapSiglaEstado = new Dictionary<string, EstadoModel>();
            MunicipioModel municipioAgente = null;
            if (trabalhaMunicipio != null)
                municipioAgente = _municipioService.GetById(trabalhaMunicipio.IdMunicipio);

            using (var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF7))
            {
                var cabecalho = reader.ReadLine();
                indices = IndexaColunasArquivoGal(cabecalho) ?? IndexaColunasArquivoUFS(cabecalho);

                if (indices == null)
                    throw new ServiceException("Essa planilha não possui as informações necessárias para fazer a importação, " +
                                                        "por favor verifique a planilha e tente novamente.");

                if (indices.EhPlanilhaGal)
                {
                    line = reader.ReadLine().Split(';');
                    while (reader.Peek() >= 0 && String.Concat(line).Length > 0)
                    {
                        if (!line[indices.IndiceStatusExame].ToUpper().Equals("RESULTADO LIBERADO"))
                        {
                            line = reader.ReadLine().Split(';');
                            continue;
                        }
                        EstadoModel estadoPaciente = null;
                        if (line[indices.IndiceEstadoPaciente].Length > 2)
                        {
                            if (mapEstado.GetValueOrDefault(line[indices.IndiceEstadoPaciente]) == null)
                                mapEstado.Add(line[indices.IndiceEstadoPaciente], _estadoService.GetByName(line[indices.IndiceEstadoPaciente]));
                            estadoPaciente = mapEstado.GetValueOrDefault(line[indices.IndiceEstadoPaciente]);
                        }
                        else if (line[indices.IndiceEstadoPaciente].Length == 2)
                        {
                            if (mapSiglaEstado.GetValueOrDefault(line[indices.IndiceEstadoPaciente]) == null)
                                mapSiglaEstado.Add(line[indices.IndiceEstadoPaciente], _estadoService.GetByUf(line[indices.IndiceEstadoPaciente]));
                            estadoPaciente = mapSiglaEstado.GetValueOrDefault(line[indices.IndiceEstadoPaciente]);
                        }
                        if (mapCidade.GetValueOrDefault(line[indices.IndiceCidadePaciente]) == null)
                            mapCidade.Add(line[indices.IndiceCidadePaciente], _municipioGeoService.GetByName(line[indices.IndiceCidadePaciente], estadoPaciente.CodigoUf));
                        MunicipioGeoModel cidadePaciente = mapCidade.GetValueOrDefault(line[indices.IndiceCidadePaciente]);
                        if (mapCidade.GetValueOrDefault(line[indices.IndiceCidadeEmpresa]) == null)
                            mapCidade.Add(line[indices.IndiceCidadeEmpresa], _municipioGeoService.GetByName(line[indices.IndiceCidadeEmpresa], estadoPaciente.CodigoUf));
                        MunicipioGeoModel cidadeEmpresa = mapCidade.GetValueOrDefault(line[indices.IndiceCidadeEmpresa]);

                        var exame = new ExameViewModel
                        {
                            Usuario = agente.UsuarioModel,
                            Paciente = new PessoaModel
                            {
                                Nome = line[indices.IndiceNomePaciente],
                                Cidade = line[indices.IndiceCidadePaciente],
                                Cep = line[indices.IndiceCepPaciente].Length > 0 ? Methods.RemoveSpecialsCaracts(line[indices.IndiceCepPaciente]) : "00000000",
                                Bairro = line[indices.IndiceBairroPaciente].Length > 0 && line[indices.IndiceBairroPaciente].Length < 60 ? line[indices.IndiceBairroPaciente] : "NAO INFORMADO",
                                Estado = line[indices.IndiceEstadoPaciente].Length > 2 ? mapEstado.GetValueOrDefault(line[indices.IndiceEstadoPaciente]).Uf : line[indices.IndiceEstadoPaciente].ToUpper(),
                                FoneCelular = line[indices.IndiceFoneCelularPaciente],
                                DataNascimento = !line[indices.IndiceDataNascimentoPaciente].Equals("") ? Convert.ToDateTime(line[indices.IndiceDataNascimentoPaciente]) : DateTime.MinValue,
                                Longitude = cidadePaciente != null ? cidadePaciente.Longitude.ToString() : "0",
                                Latitude = cidadePaciente != null ? cidadePaciente.Latitude.ToString() : "0",
                                IdAreaAtuacao = 0,
                                OutrasComorbidades = "",
                                OutrosSintomas = "",
                                Cpf = line[indices.IndiceTipoDocumento1Paciente].Equals("CPF") && Methods.ValidarCpf(line[indices.IndiceDocumento1Paciente]) ?
                                   Methods.RemoveSpecialsCaracts(line[indices.IndiceDocumento1Paciente]) : line[indices.IndiceTipoDocumento2Paciente].Equals("CPF") && Methods.ValidarCpf(line[indices.IndiceDocumento2Paciente]) ?
                                   Methods.RemoveSpecialsCaracts(line[indices.IndiceDocumento2Paciente]) : "",
                                Sexo = line[indices.IndiceSexoPaciente].Equals("FEMININO") ? "F" : "M",
                                Rua = line[indices.IndiceRuaPaciente].Length > 0 && line[indices.IndiceRuaPaciente].Length < 60 ? line[indices.IndiceRuaPaciente].Split('-')[0] : "NÃO INFORMADO",
                                Numero = line[indices.IndiceRuaPaciente].Length > 0 && line[indices.IndiceRuaPaciente].Split('-').Length >= 2 ?
                                (Methods.SoContemNumeros(line[indices.IndiceRuaPaciente].Split('-')[1].Trim()) ? line[indices.IndiceRuaPaciente].Split('-')[1].Trim() : "") : "",
                                Complemento = line[indices.IndiceRuaPaciente].Length > 0 && line[indices.IndiceRuaPaciente].Split('-').Length == 3 ?
                                (line[indices.IndiceRuaPaciente].Split('-')[2].Trim().Length < 100 ? line[indices.IndiceRuaPaciente].Split('-')[2].Trim() : "") : "",
                                Cns = line[indices.IndiceCnsPaciente],
                                Profissao = "NÃO INFORMADA",

                            },

                            Exame = new ExameModel
                            {
                                IdAgenteSaude = agente.UsuarioModel.IdPessoa,
                                DataExame = Convert.ToDateTime(line[indices.IndiceDataExame]),
                                IdEstado = trabalhaMunicipio != null ? Convert.ToInt32(municipioAgente.Uf) : trabalhaEstado.IdEstado,
                                IdAreaAtuacao = 0,
                                CodigoColeta = line[indices.IndiceCodigoColeta],
                                MetodoExame = "P",
                                IdVirusBacteria = GetIdVirusBacteriaItemImportacao(line[indices.IndiceTipoExame], listVirusBacteria),
                                DataInicioSintomas = line[indices.IndiceDataInicioSintomas].Equals("") ? Convert.ToDateTime(line[indices.IndiceDataExame]) : Convert.ToDateTime(line[indices.IndiceDataInicioSintomas]),
                                IgG = line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && !line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) ? GetMetodoExameImportacao(line[indices.IndiceMetodoExame], IndiceItemArquivoImportacao.METODO_IGG, line[indices.IndiceResultadoExame].Length > 0 ? line[indices.IndiceResultadoExame] : line[indices.IndiceObservacaoExame]) : "N",
                                IgM = line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) && !line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) ? GetMetodoExameImportacao(line[indices.IndiceMetodoExame], IndiceItemArquivoImportacao.METODO_IGM, line[indices.IndiceResultadoExame].Length > 0 ? line[indices.IndiceResultadoExame] : line[indices.IndiceObservacaoExame]) : "N",
                                Pcr = line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_PCR) ? GetMetodoExameImportacao(line[indices.IndiceMetodoExame], IndiceItemArquivoImportacao.METODO_PCR, line[indices.IndiceResultadoExame].Length > 0 ? line[indices.IndiceResultadoExame] : line[indices.IndiceObservacaoExame]) : "N",
                                IgGIgM = line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && line[indices.IndiceMetodoExame].ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) ? GetMetodoExameImportacao(line[indices.IndiceMetodoExame], IndiceItemArquivoImportacao.METODO_IGG_IGM, line[indices.IndiceResultadoExame].Length > 0 ? line[indices.IndiceResultadoExame] : line[indices.IndiceObservacaoExame]) : "N",
                                AguardandoResultado = false,
                                StatusNotificacao = "N",
                                
                            },

                            EmpresaExame = new EmpresaExameModel
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
                            },
                        };

                        if (trabalhaMunicipio != null)
                            exame.Exame.IdMunicipio = trabalhaMunicipio.IdMunicipio;
                        else
                            exame.Exame.IdMunicipio = null;

                        examesPlanilha.Add(exame);

                        line = reader.ReadLine().Split(';');
                    }
                }
                else
                {
                    line = reader.ReadLine().Split(';');
                    //MunicipioGeoModel cidadePaciente;
                    while (reader.Peek() >= 0 && String.Concat(line).Length > 0)
                    {
                        EstadoModel estadoPaciente = null;
                        if (line[indices.IndiceEstadoPaciente].Length > 2)
                        {
                            if (mapEstado.GetValueOrDefault(line[indices.IndiceEstadoPaciente]) == null)
                                mapEstado.Add(line[indices.IndiceEstadoPaciente], _estadoService.GetByName(line[indices.IndiceEstadoPaciente]));
                            estadoPaciente = mapEstado.GetValueOrDefault(line[indices.IndiceEstadoPaciente]);
                        }
                        else if (line[indices.IndiceEstadoPaciente].Length == 2)
                        {
                            if (mapSiglaEstado.GetValueOrDefault(line[indices.IndiceEstadoPaciente]) == null)
                                mapSiglaEstado.Add(line[indices.IndiceEstadoPaciente], _estadoService.GetByUf(line[indices.IndiceEstadoPaciente]));
                            estadoPaciente = mapSiglaEstado.GetValueOrDefault(line[indices.IndiceEstadoPaciente]);
                        }
                        if (mapCidade.GetValueOrDefault(line[indices.IndiceCidadePaciente]) == null)
                            mapCidade.Add(line[indices.IndiceCidadePaciente], _municipioGeoService.GetByName(line[indices.IndiceCidadePaciente], estadoPaciente.CodigoUf));
                        MunicipioGeoModel cidadePaciente = mapCidade.GetValueOrDefault(line[indices.IndiceCidadePaciente]);
                        var exame = new ExameViewModel
                        {
                            Usuario = agente.UsuarioModel,
                            Paciente = new PessoaModel
                            {
                                Nome = line[indices.IndiceNomePaciente],
                                Cidade = line[indices.IndiceCidadePaciente],
                                Cep = line[indices.IndiceCepPaciente].Length > 0 ? Methods.RemoveSpecialsCaracts(line[indices.IndiceCepPaciente]) : "00000000",
                                Bairro = line[indices.IndiceBairroPaciente].Length > 0 && line[indices.IndiceBairroPaciente].Length < 60 ? line[indices.IndiceBairroPaciente] : "NAO INFORMADO",
                                Estado = line[indices.IndiceEstadoPaciente].Length > 2 ? estadoPaciente.Uf : line[indices.IndiceEstadoPaciente].ToUpper(),
                                FoneCelular = line[indices.IndiceFoneCelularPaciente],
                                DataNascimento = !line[indices.IndiceDataNascimentoPaciente].Equals("") ? Convert.ToDateTime(line[indices.IndiceDataNascimentoPaciente]) : DateTime.MinValue,
                                Longitude = cidadePaciente != null ? cidadePaciente.Longitude.ToString() : "0",
                                Latitude = cidadePaciente != null ? cidadePaciente.Latitude.ToString() : "0",
                                IdAreaAtuacao = 0,
                                OutrasComorbidades = "",
                                OutrosSintomas = "",
                                Sexo = "M",
                                Rua = line[indices.IndiceRuaPaciente].Length > 0 && line[indices.IndiceRuaPaciente].Length < 60 ? line[indices.IndiceRuaPaciente] : "NÃO INFORMADO",
                                Numero = line[indices.IndicenNumeroResidenciaPaciente].Length > 0 ? line[indices.IndicenNumeroResidenciaPaciente] : "",
                                Complemento = line[indices.IndiceComplementoPaciente].Length > 0 ? line[indices.IndiceComplementoPaciente] : "",
                                Cpf = line[indices.IndiceCpfPaciente].Length > 0 && Methods.ValidarCpf(line[indices.IndiceCpfPaciente]) ? line[indices.IndiceCpfPaciente] : "",
                                Profissao = line[indices.IndiceProfissaoPaciente],
                                Hipertenso = VerificaSintomaOuDoencaImportacao(line, indices.IndicesDoencaPacienteUfs, IndiceItemArquivoImportacao.DOENCA_HIPERTENSAO),
                                Diabetes = VerificaSintomaOuDoencaImportacao(line, indices.IndicesDoencaPacienteUfs, IndiceItemArquivoImportacao.DOENCA_DIABETES),
                                Obeso = VerificaSintomaOuDoencaImportacao(line, indices.IndicesDoencaPacienteUfs, IndiceItemArquivoImportacao.DOENCA_OBESIDADE),
                                Cardiopatia = VerificaSintomaOuDoencaImportacao(line, indices.IndicesDoencaPacienteUfs, IndiceItemArquivoImportacao.DOENCA_CARDIOPATIA),
                                Imunodeprimido = VerificaSintomaOuDoencaImportacao(line, indices.IndicesDoencaPacienteUfs, IndiceItemArquivoImportacao.DEONCA_IMUNODEPRIMIDO),
                                Cancer = VerificaSintomaOuDoencaImportacao(line, indices.IndicesDoencaPacienteUfs, IndiceItemArquivoImportacao.DEONCA_CANCER),
                                DoencaRespiratoria = VerificaSintomaOuDoencaImportacao(line, indices.IndicesDoencaPacienteUfs, IndiceItemArquivoImportacao.DOENCA_RESPIRATORIA),
                                DoencaRenal = VerificaSintomaOuDoencaImportacao(line, indices.IndicesDoencaPacienteUfs, IndiceItemArquivoImportacao.DOENCA_RENAL),
                                Epilepsia = VerificaSintomaOuDoencaImportacao(line, indices.IndicesDoencaPacienteUfs, IndiceItemArquivoImportacao.DEONCA_EPILESIA),
                            },

                            Exame = new ExameModel
                            {
                                IdAgenteSaude = agente.UsuarioModel.IdPessoa,
                                DataExame = Convert.ToDateTime(line[indices.IndiceDataExame]),
                                IdEstado = trabalhaMunicipio != null ? Convert.ToInt32(municipioAgente.Uf) : trabalhaEstado.IdEstado,
                                IdAreaAtuacao = 0,
                                CodigoColeta = line[indices.IndiceCodigoColeta],
                                MetodoExame = "F",
                                PerdaOlfatoPaladar = VerificaSintomaOuDoencaImportacao(line, indices.IndicesSintomasPacienteUfs, IndiceItemArquivoImportacao.SINTOMA_PERDA_OLFATO),
                                Febre = VerificaSintomaOuDoencaImportacao(line, indices.IndicesSintomasPacienteUfs, IndiceItemArquivoImportacao.SINTOMA_FEBRE),
                                Tosse = VerificaSintomaOuDoencaImportacao(line, indices.IndicesSintomasPacienteUfs, IndiceItemArquivoImportacao.SINTOMA_TOSSE),
                                Coriza = VerificaSintomaOuDoencaImportacao(line, indices.IndicesSintomasPacienteUfs, IndiceItemArquivoImportacao.SINTOMA_CORIZA),
                                DificuldadeRespiratoria = VerificaSintomaOuDoencaImportacao(line, indices.IndicesSintomasPacienteUfs, IndiceItemArquivoImportacao.SINTOMA_DIFICULDADE_RESPIRATORIA),
                                DorGarganta = VerificaSintomaOuDoencaImportacao(line, indices.IndicesSintomasPacienteUfs, IndiceItemArquivoImportacao.SINTOMA_DOR_DE_GARGANTA),
                                Diarreia = VerificaSintomaOuDoencaImportacao(line, indices.IndicesSintomasPacienteUfs, IndiceItemArquivoImportacao.SINTOMA_DIARREIA),
                                DorOuvido = VerificaSintomaOuDoencaImportacao(line, indices.IndicesSintomasPacienteUfs, IndiceItemArquivoImportacao.SINTOMA_DOR_DE_OUVIDO),
                                Nausea = VerificaSintomaOuDoencaImportacao(line, indices.IndicesSintomasPacienteUfs, IndiceItemArquivoImportacao.SINTOMA_NAUSEAS),
                                DorAbdominal = VerificaSintomaOuDoencaImportacao(line, indices.IndicesSintomasPacienteUfs, IndiceItemArquivoImportacao.SINTOMA_DORES_E_DESCONFORTO),
                                IdVirusBacteria = GetIdVirusBacteriaItemImportacao("COVID-19", listVirusBacteria),
                                DataInicioSintomas = Convert.ToDateTime(line[indices.IndiceDataExame]),
                                IgG = "N",
                                IgM = line[indices.IndiceRealizouTeste].ToUpper().Contains("POSITIVO") ? "S" : "N",
                                Pcr = "N",
                                IgGIgM = "N",
                                IdEmpresaSaude = trabalhaEstado.IdEmpresaExame,
                                AguardandoResultado = false,
                                StatusNotificacao = "N",
                            },
                        };

                        if (trabalhaMunicipio != null)
                            exame.Exame.IdMunicipio = trabalhaMunicipio.IdMunicipio;
                        else
                            exame.Exame.IdMunicipio = null;

                        examesPlanilha.Add(exame);

                        line = reader.ReadLine().Split(';');
                    }
                }
            }

            foreach (var exameView in examesPlanilha)
            {
                var exameGravado = _context.Exame
                    .Where(e => e.CodigoColeta.Equals(exameView.Exame.CodigoColeta))
                    .Select(ex => new
                    {
                        IdExame = ex.IdExame,
                        IdEmpresa = ex.IdEmpresaSaude,
                        IdPaciente = ex.IdPaciente
                    }).FirstOrDefault();

                if (exameGravado == null || (exameGravado != null && exameGravado.IdEmpresa < 1))
                {
                    if (exameView.Exame.IdEmpresaSaude < 1)
                    {
                        var empresa = _empresaExameService.GetByCNES(exameView.EmpresaExame.Cnes);
                        if (empresa == null)
                            empresa = _empresaExameService.Insert(exameView.EmpresaExame);
                        exameView.Exame.IdEmpresaSaude = empresa.Id;
                    }
                } 
                else
                {
                    exameView.Exame.IdEmpresaSaude = exameGravado.IdEmpresa;
                    exameView.Exame.IdPaciente = exameGravado.IdPaciente;
                    exameView.Paciente.Idpessoa = exameGravado.IdPaciente;
                    exameView.Exame.IdExame = exameGravado.IdExame;
                }

                if (exameGravado == null)
                    _exameService.Insert(exameView);
                else
                    _exameService.Update(exameView);
            }
        }
        private IndiceItemArquivoImportacao IndexaColunasArquivoUFS(string cabecalho)
        {
            var itens = cabecalho.Split(';');
            var indices = new IndiceItemArquivoImportacao();

            for (int i = 0; i < itens.Length; i++)
            {
                if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.NOME_PACIENTE_UFS)))
                    indices.IndiceNomePaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DATA_DE_NASCIMENTO_PACIENTE_UFS)))
                    indices.IndiceDataNascimentoPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CPF_PACIENTE_UFS)))
                    indices.IndiceCpfPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.ENDERECO_PACIENTE_UFS)))
                    indices.IndiceRuaPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.BAIRRO_PACIENTE_UFS)))
                    indices.IndiceBairroPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CEP_PACIENTE_UFS)))
                    indices.IndiceCepPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.MUNICIPIO_PACIENTE_UFS)))
                    indices.IndiceCidadePaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.ESTADO_PACIENTE_UFS)))
                    indices.IndiceEstadoPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CELULAR_PACIENTE_UFS)))
                    indices.IndiceFoneCelularPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.REALIZOU_TESTE_COVID_UFS)))
                    indices.IndiceRealizouTeste = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.NUMERO_DE_REGISTRO_UFS)))
                    indices.IndiceCodigoColeta = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DATA_DA_COLETA_UFS)))
                    indices.IndiceDataExame = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.NUMERO_RESIDENCIA_PACIENTE_UFS)))
                    indices.IndicenNumeroResidenciaPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.COMPLEMENTO_PACIENTE_UFS)))
                    indices.IndiceComplementoPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.PROFISSAO_PACIENTE_UFS)))
                    indices.IndiceProfissaoPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DOENCAS_CRONICAS_PACIENTE_UFS)) ||
                         Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DOENCAS_BACTERIANAS_PACIENTE_UFS)))
                    indices.IndicesDoencaPacienteUfs.Add(i);
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.SINAIS_E_SINTOMAS_PACIENTE_UFS).ToUpper()))
                    indices.IndicesSintomasPacienteUfs.Add(i);
            }

            var planilhaValida = false;
            if (indices.IndiceNomePaciente != -1 && indices.IndiceDataNascimentoPaciente != -1 && indices.IndiceCpfPaciente != -1 && indices.IndiceRuaPaciente != -1 &&
                indices.IndiceBairroPaciente != -1 && indices.IndiceCepPaciente != -1 && indices.IndiceCidadePaciente != -1 && indices.IndiceEstadoPaciente != -1 &&
                indices.IndiceFoneCelularPaciente != -1 && indices.IndiceRealizouTeste != -1 && indices.IndiceCodigoColeta != -1 && indices.IndiceDataExame != -1 &&
                indices.IndicenNumeroResidenciaPaciente != -1 && indices.IndiceComplementoPaciente != -1 && indices.IndiceProfissaoPaciente != -1)
            {
                planilhaValida = true;
                indices.EhPlanilhaGal = false;
            }

            return planilhaValida ? indices : null;
        }

        private IndiceItemArquivoImportacao IndexaColunasArquivoGal(string cabecalho)
        {
            var itens = cabecalho.Split(';');
            var indices = new IndiceItemArquivoImportacao();

            for (int i = 0; i < itens.Length; i++)
            {
                if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.UNIDADE_SOLICITANTE_GAL)))
                    indices.IndiceNomeEmpresa = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CNES_UNIDADE_SOLCITANTE_GAL)))
                    indices.IndiceCnesEmpresa = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.MUNICIPIO_DO_SOLICITANTE_GAL)))
                    indices.IndiceCidadeEmpresa = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.ESTADO_DO_SOLICITANTE_GAL)))
                    indices.IndiceEstadoEmpresa = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CNS_DO_PACIENTE_GAL)))
                    indices.IndiceCnsPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.NOME_PACIENTE_GAL)))
                    indices.IndiceNomePaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.SEXO_PACIENTE_GAL)))
                    indices.IndiceSexoPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DATA_DE_NASCIMENTO_PACIENTE_GAL)))
                    indices.IndiceDataNascimentoPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.TIPO_DOCUMENTO_1_GAL)))
                    indices.IndiceTipoDocumento1Paciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DOCUMENTO_1_GAL)))
                    indices.IndiceDocumento1Paciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.TIPO_DOCUMENTO_2_GAL)))
                    indices.IndiceTipoDocumento2Paciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DOCUMENTO_2_GAL)))
                    indices.IndiceDocumento2Paciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.ENDERECO_PACIENTE_GAL)))
                    indices.IndiceRuaPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.BAIRRO_PACIENTE_GAL)))
                    indices.IndiceBairroPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CEP_PACIENTE_GAL)))
                    indices.IndiceCepPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.MUNICIPIO_PACIENTE_GAL)))
                    indices.IndiceCidadePaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.ESTADO_PACIENTE_GAL)))
                    indices.IndiceEstadoPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CELULAR_PACIENTE_GAL)))
                    indices.IndiceFoneCelularPaciente = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.TIPO_EXAME_GAL)))
                    indices.IndiceTipoExame = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.METODO_EXAME_GAL)))
                    indices.IndiceMetodoExame = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.CODIGO_DA_AMOSTRA_GAL)))
                    indices.IndiceCodigoColeta = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DATA_DA_COLETA_GAL)))
                    indices.IndiceDataExame = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.DATA_INICIO_SINTOMAS_GAL)))
                    indices.IndiceDataInicioSintomas = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.OBSERVACOES_RESULTADO_GAL)))
                    indices.IndiceObservacaoExame = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_GAL).ToUpper()))
                    indices.IndiceResultadoExame = i;
                else if (Methods.RemoveSpecialsCaracts(itens[i].Trim()).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.STATUS_EXAME).ToUpper()))
                    indices.IndiceStatusExame = i;
            }

            var planilhaValida = false;
            if (indices.IndiceNomeEmpresa != -1 && indices.IndiceCnesEmpresa != -1 && indices.IndiceCidadeEmpresa != -1 && indices.IndiceEstadoEmpresa != -1 && indices.IndiceFoneCelularPaciente != -1 &&
                indices.IndiceCnsPaciente != -1 && indices.IndiceNomePaciente != -1 && indices.IndiceDataNascimentoPaciente != -1 && indices.IndiceTipoDocumento1Paciente != -1 && indices.IndiceDocumento1Paciente != -1 &&
                indices.IndiceTipoDocumento2Paciente != -1 && indices.IndiceDocumento2Paciente != -1 && indices.IndiceRuaPaciente != -1 && indices.IndiceBairroPaciente != -1 && indices.IndiceCepPaciente != -1 &&
                indices.IndiceCidadePaciente != -1 && indices.IndiceEstadoPaciente != -1 && indices.IndiceTipoExame != -1 && indices.IndiceMetodoExame != -1 && indices.IndiceCodigoColeta != -1 && indices.IndiceDataExame != -1 &&
                indices.IndiceDataInicioSintomas != -1 && indices.IndiceResultadoExame != -1 && indices.IndiceObservacaoExame != -1 && indices.IndiceSexoPaciente != -1)
            {
                planilhaValida = true;
                indices.EhPlanilhaGal = true;
            }

            return planilhaValida ? indices : null;
        }


        private string GetMetodoExameImportacao(string exame, string metodo, string resultado)
        {
            switch (metodo)
            {
                case "PCR":
                    if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_DETECTAVEL_GAL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_PCR))
                        return "S";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_NAO_DETECTAVEL_GAL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_PCR))
                        return "N";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_SOLICITAR_NOVA_COLETA_GAL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_PCR))
                        return "I";
                    else
                        return "I";

                case "IGG":
                    if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_DETECTAVEL_GAL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "S";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_NAO_DETECTAVEL_GAL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "N";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_SOLICITAR_NOVA_COLETA_GAL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "I";
                    else
                        return "I";

                case "IGM":
                    if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_DETECTAVEL_GAL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG))
                        return "S";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_NAO_DETECTAVEL_GAL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG))
                        return "N";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_SOLICITAR_NOVA_COLETA_GAL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM) && !exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG))
                        return "I";
                    else
                        return "I";

                case "IGG/IGM":
                    if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_DETECTAVEL_GAL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "S";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_NAO_DETECTAVEL_GAL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
                        return "N";
                    else if (Methods.RemoveSpecialsCaracts(resultado).ToUpper().Equals(Methods.RemoveSpecialsCaracts(IndiceItemArquivoImportacao.RESULTADO_SOLICITAR_NOVA_COLETA_GAL)) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGG) && exame.ToUpper().Contains(IndiceItemArquivoImportacao.METODO_IGM))
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
        private bool VerificaSintomaOuDoencaImportacao(string[] linha, List<int> indices, string doencaOuSintoma)
        {
            var possui = false;
            foreach (int i in indices)
            {
                if (Methods.RemoveSpecialsCaracts(linha[i]).ToUpper().Contains(Methods.RemoveSpecialsCaracts(doencaOuSintoma)))
                    possui = true;
            }

            return possui;
        }

        private Exame ModelToEntityImportacao(ExameViewModel item, ExameModel exame)
        {
            return new Exame
            {
                IdExame = exame != null ? exame.IdExame : 0,
                IdPaciente = item.Paciente.Idpessoa,
                IdVirusBacteria = item.Exame.IdVirusBacteria,
                IdAgenteSaude = item.Exame.IdAgenteSaude,
                DataExame = item.Exame.DataExame,
                DataInicioSintomas = item.Exame.DataInicioSintomas,
                DataNotificacao = DateTime.Now,
                IgG = item.Exame.IgG,
                IgM = item.Exame.IgM,
                Pcr = item.Exame.Pcr,
                IgMigG = item.Exame.IgGIgM,
                IdMunicipio = item.Exame.IdMunicipio,
                IdEstado = item.Exame.IdEstado,
                IdEmpresaSaude = item.Exame.IdEmpresaSaude,
                IdAreaAtuacao = item.Exame.IdAreaAtuacao,
                CodigoColeta = item.Exame.CodigoColeta,
                PerdaOlfatoPaladar = Convert.ToByte(item.Exame.PerdaOlfatoPaladar),
                Febre = Convert.ToByte(item.Exame.Febre),
                Tosse = Convert.ToByte(item.Exame.Tosse),
                Coriza = Convert.ToByte(item.Exame.Coriza),
                DificuldadeRespiratoria = Convert.ToByte(item.Exame.DificuldadeRespiratoria),
                DorGarganta = Convert.ToByte(item.Exame.DorGarganta),
                Diarreia = Convert.ToByte(item.Exame.Diarreia),
                DorOuvido = Convert.ToByte(item.Exame.DorOuvido),
                Nausea = Convert.ToByte(item.Exame.Nausea),
                DorAbdominal = Convert.ToByte(item.Exame.DorAbdominal),
                IdNotificacao = "",
                OutroSintomas = "",
                MetodoExame = item.Exame.MetodoExame,
                StatusNotificacao = exame != null ? exame.StatusNotificacao : "N"
            };
        }
    }
}
