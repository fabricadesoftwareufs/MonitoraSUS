using Model;
using Model.AuxModel;
using Model.ViewModel;
using Newtonsoft.Json;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Service
{
    public class ExameService : IExameService
    {
        private readonly monitorasusContext _context;
		
        public ExameService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Insert(ExameModel exameModel)
        {
            _context.Add(ModelToEntity(exameModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

        public bool Delete(int id)
        {
            var exame = _context.Exame.Find(id);
            _context.Exame.Remove(exame);
            return _context.SaveChanges() == 1 ? true : false;
        }

        public bool Update(ExameModel exameModel)
        {
            _context.Update(ModelToEntity(exameModel));
            return _context.SaveChanges() == 1 ? true : false;
        }

        public List<ExameModel> GetByIdAgente(int idAgente)
         => _context.Exame
                .Where(exameModel => exameModel.IdAgenteSaude == idAgente)
                .Select(exame => new ExameModel
                {
                    IdVirusBacteria = exame.IdVirusBacteria,
                    IdExame = exame.IdExame,
                    IdPaciente = exame.IdPaciente,
                    IdAgenteSaude = exame.IdAgenteSaude,
                    DataExame = exame.DataExame,
                    DataInicioSintomas = exame.DataInicioSintomas,
                    IgG = exame.IgG,
                    IgM = exame.IgM,
                    Pcr = exame.Pcr,
                    IdEstado = exame.IdEstado,
                    IdMunicipio = exame.IdMunicipio,
                    IdEmpresaSaude = exame.IdEmpresaSaude,
                    EhProfissionalSaude = Convert.ToBoolean(exame.EhProfissionalSaude),
                    CodigoColeta = exame.CodigoColeta,
                    StatusNotificacao = exame.StatusNotificacao,
                    IdNotificacao = exame.IdNotificacao,
                }).ToList();

        public List<ExameModel> GetAll()
             => _context.Exame
                .Select(exame => new ExameModel
                {
                    IdVirusBacteria = exame.IdVirusBacteria,
                    IdExame = exame.IdExame,
                    IdPaciente = exame.IdPaciente,
                    IdAgenteSaude = exame.IdAgenteSaude,
                    DataExame = exame.DataExame,
                    DataInicioSintomas = exame.DataInicioSintomas,
                    IgG = exame.IgG,
                    IgM = exame.IgM,
                    Pcr = exame.Pcr,
                    IdEstado = exame.IdEstado,
                    IdMunicipio = exame.IdMunicipio,
                    IdEmpresaSaude = exame.IdEmpresaSaude,
                    EhProfissionalSaude = Convert.ToBoolean(exame.EhProfissionalSaude),
                    CodigoColeta = exame.CodigoColeta,
                    StatusNotificacao = exame.StatusNotificacao,
                    IdNotificacao = exame.IdNotificacao,
                }).ToList();


        public ExameModel GetById(int id)
          => _context.Exame
                .Where(exameModel => exameModel.IdExame == id)
                .Select(exame => new ExameModel
                {
                    IdVirusBacteria = exame.IdVirusBacteria,
                    IdExame = exame.IdExame,
                    IdPaciente = exame.IdPaciente,
                    IdAgenteSaude = exame.IdAgenteSaude,
                    DataExame = exame.DataExame,
                    DataInicioSintomas = exame.DataInicioSintomas,
                    IgG = exame.IgG,
                    IgM = exame.IgM,
                    Pcr = exame.Pcr,
                    IdEstado = exame.IdEstado,
                    IdMunicipio = exame.IdMunicipio,
                    IdEmpresaSaude = exame.IdEmpresaSaude,
                    EhProfissionalSaude = Convert.ToBoolean(exame.EhProfissionalSaude),
                    CodigoColeta = exame.CodigoColeta,
                    StatusNotificacao = exame.StatusNotificacao,
                    IdNotificacao = exame.IdNotificacao,
                }).FirstOrDefault();

        private Exame ModelToEntity(ExameModel exameModel)
        {
            return new Exame
            {
                IdExame = exameModel.IdExame,
                IdAgenteSaude = exameModel.IdAgenteSaude,
                IdPaciente = exameModel.IdPaciente,
                IdVirusBacteria = exameModel.IdVirusBacteria,
                IgG = exameModel.IgG,
                IgM = exameModel.IgM,
                Pcr = exameModel.Pcr,
                IdEstado = exameModel.IdEstado,
                IdMunicipio = exameModel.IdMunicipio,
                DataInicioSintomas = exameModel.DataInicioSintomas,
                DataExame = exameModel.DataExame,
                IdEmpresaSaude = exameModel.IdEmpresaSaude,
                EhProfissionalSaude = Convert.ToByte(exameModel.EhProfissionalSaude),
                CodigoColeta = exameModel.CodigoColeta,
                StatusNotificacao = exameModel.StatusNotificacao,
                IdNotificacao = exameModel.IdNotificacao,
				DataNotificacao = DateTime.Now
            };
        }

        public List<ExameModel> GetByIdEmpresa(int idEempresa)
       => _context.Exame
              .Where(exameModel => exameModel.IdEmpresaSaude == idEempresa)
              .Select(exame => new ExameModel
              {
                  IdVirusBacteria = exame.IdVirusBacteria,
                  IdExame = exame.IdExame,
                  IdPaciente = exame.IdPaciente,
                  IdAgenteSaude = exame.IdAgenteSaude,
                  DataExame = exame.DataExame,
                  DataInicioSintomas = exame.DataInicioSintomas,
                  IgG = exame.IgG,
                  IgM = exame.IgM,
                  Pcr = exame.Pcr,
                  IdEstado = exame.IdEstado,
                  IdMunicipio = exame.IdMunicipio,
                  IdEmpresaSaude = exame.IdEmpresaSaude,
                  EhProfissionalSaude = Convert.ToBoolean(exame.EhProfissionalSaude),
                  CodigoColeta = exame.CodigoColeta,
                  StatusNotificacao = exame.StatusNotificacao,
                  IdNotificacao = exame.IdNotificacao,
              }).ToList();

       

        public ConfiguracaoNotificarModel BuscarConfiguracaoNotificar(int IdEstado, int IdEmpresaExame)
        => _context.Configuracaonotificar
                .Where(c => c.IdEstado == IdEstado && c.IdEmpresaExame == IdEmpresaExame)
                .Select(conf => new ConfiguracaoNotificarModel
                {
                    HabilitadoSms = conf.HabilitadoSms,
                    HabilitadoWhatsapp = conf.HabilitadoWhatsapp,
                    IdConfiguracaoNotificar = conf.IdConfiguracaoNotificar,
                    IdEmpresaExame = conf.IdEmpresaExame,
                    IdEstado = conf.IdEstado,
                    IdMunicipio = conf.IdMunicipio,
                    MensagemImunizado = conf.MensagemImunizado,
                    MensagemIndeterminado = conf.MensagemIndeterminado,
                    MensagemPositivo = conf.MensagemPositivo,
                    MensagemNegativo = conf.MensagemNegativo,
                    QuantidadeSmsdisponivel = conf.QuantidadeSmsdisponivel,
                    Sid = conf.Sid,
                    Token = conf.Token
                }).FirstOrDefault();

        public ConfiguracaoNotificarModel BuscarConfiguracaoNotificar(int IdMunicipio)
        => _context.Configuracaonotificar
                .Where(c => c.IdMunicipio == IdMunicipio)
                .Select(conf => new ConfiguracaoNotificarModel
                {
                    HabilitadoSms = conf.HabilitadoSms,
                    HabilitadoWhatsapp = conf.HabilitadoWhatsapp,
                    IdConfiguracaoNotificar = conf.IdConfiguracaoNotificar,
                    IdEmpresaExame = conf.IdEmpresaExame,
                    IdEstado = conf.IdEstado,
                    IdMunicipio = conf.IdMunicipio,
                    MensagemImunizado = conf.MensagemImunizado,
                    MensagemIndeterminado = conf.MensagemIndeterminado,
                    MensagemPositivo = conf.MensagemPositivo,
                    MensagemNegativo = conf.MensagemNegativo,
                    QuantidadeSmsdisponivel = conf.QuantidadeSmsdisponivel,
                    Sid = conf.Sid,
                    Token = conf.Token
                }).FirstOrDefault();

        public async System.Threading.Tasks.Task<ExameModel> EnviarSMSResultadoExameAsync(ConfiguracaoNotificarModel configuracaoNotificar, ExameModel exame, PessoaModel pessoa)
        {
            try
            {
                string mensagem = "[MonitoraSUS]Olá, " + pessoa.Nome + ". ";
                if (exame.Resultado.Equals(ExameModel.RESULTADO_POSITIVO))
                    mensagem += configuracaoNotificar.MensagemPositivo;
                else if (exame.Resultado.Equals(ExameModel.RESULTADO_NEGATIVO))
                    mensagem += configuracaoNotificar.MensagemNegativo;
                else if (exame.Resultado.Equals(ExameModel.RESULTADO_IMUNIZADO))
                    mensagem += configuracaoNotificar.MensagemImunizado;
                else if (exame.Resultado.Equals(ExameModel.RESULTADO_INDETERMINADO))
                    mensagem += configuracaoNotificar.MensagemIndeterminado;

                var cliente = new HttpClient();
                string url = "https://api.smsdev.com.br/send?key=" + configuracaoNotificar.Token + "&type=9&";
                var uri = url + "number=" + pessoa.FoneCelular + "&msg=" + mensagem;
                var resultadoEnvio = await cliente.GetStringAsync(uri);
				ResponseSMSModel jsonResponse = JsonConvert.DeserializeObject<ResponseSMSModel>(resultadoEnvio);
				exame.IdNotificacao = jsonResponse.Id.ToString();
				exame.StatusNotificacao = ExameModel.NOTIFICADO_ENVIADO;
                Update(exame);
				Configuracaonotificar configura = _context.Configuracaonotificar.Where(s => s.IdConfiguracaoNotificar == configuracaoNotificar.IdConfiguracaoNotificar).FirstOrDefault();
				if (configura != null)
				{
					configura.QuantidadeSmsdisponivel-=1;
					_context.Update(configura);
				}
				return exame;
			}
            catch (HttpRequestException)
            {
                return exame;
            }
        }

		public async System.Threading.Tasks.Task<ExameModel> ConsultarSMSExameAsync(ConfiguracaoNotificarModel configuracaoNotificar, ExameModel exame)
		{
			try
			{
				var cliente = new HttpClient();
				string url = "https://api.smsdev.com.br/get?key=" + configuracaoNotificar.Token + "&action=status&";
				var uri = url + "id=" + exame.IdNotificacao;
				var resultadoEnvio = await cliente.GetStringAsync(uri);

				ConsultaSMSModel jsonResponse = JsonConvert.DeserializeObject<ConsultaSMSModel>(resultadoEnvio);
				if (jsonResponse.Descricao.Equals("RECEBIDA")) {
					exame.StatusNotificacao = ExameModel.NOTIFICADO_SIM;
					Update(exame);
				}
				else if (jsonResponse.Descricao.Equals("ERRO"))
				{
					exame.StatusNotificacao = ExameModel.NOTIFICADO_PROBLEMAS;
					Update(exame);
				}
			}
			catch (HttpRequestException)
			{
				return exame;
			}
			return exame;
		}

		public List<ExameModel> GetByIdPaciente(int idPaciente)
         => _context.Exame
                .Where(exameModel => exameModel.IdPaciente == idPaciente)
                .Select(exame => new ExameModel
                {
                    IdVirusBacteria = exame.IdVirusBacteria,
                    IdExame = exame.IdExame,
                    IdPaciente = exame.IdPaciente,
                    IdAgenteSaude = exame.IdAgenteSaude,
                    DataExame = exame.DataExame,
                    DataInicioSintomas = exame.DataInicioSintomas,
                    IgG = exame.IgG,
                    IgM = exame.IgM,
                    Pcr = exame.Pcr,
                    IdEstado = exame.IdEstado,
                    IdMunicipio = exame.IdMunicipio,
                    IdEmpresaSaude = exame.IdEmpresaSaude,
                    EhProfissionalSaude = Convert.ToBoolean(exame.EhProfissionalSaude),
                    CodigoColeta = exame.CodigoColeta,
                    StatusNotificacao = exame.StatusNotificacao,
                    IdNotificacao = exame.IdNotificacao,
                }).ToList();

        public List<ExameModel> GetByIdMunicipio(int idMunicicpio)
         => _context.Exame
                .Where(exameModel => exameModel.IdMunicipio == idMunicicpio)
                .Select(exame => new ExameModel
                {
                    IdVirusBacteria = exame.IdVirusBacteria,
                    IdExame = exame.IdExame,
                    IdPaciente = exame.IdPaciente,
                    IdAgenteSaude = exame.IdAgenteSaude,
                    DataExame = exame.DataExame,
                    DataInicioSintomas = exame.DataInicioSintomas,
                    IgG = exame.IgG,
                    IgM = exame.IgM,
                    Pcr = exame.Pcr,
                    IdEstado = exame.IdEstado,
                    IdMunicipio = exame.IdMunicipio,
                    IdEmpresaSaude = exame.IdEmpresaSaude,
                    EhProfissionalSaude = Convert.ToBoolean(exame.EhProfissionalSaude),
                    CodigoColeta = exame.CodigoColeta,
                    StatusNotificacao = exame.StatusNotificacao,
                    IdNotificacao = exame.IdNotificacao,
                }).ToList();


		public List<MonitoraPacienteViewModel> GetByCidadeResidenciaPaciente(string cidade, string siglaEstado,
			int idVirusBacteria, DateTime dataInicio, DateTime dataFim)
		{
			var monitoraPacientes = _context.Exame
				 .Where(e => e.IdPacienteNavigation.Cidade.ToUpper().Equals(cidade.ToUpper()) &&
						 e.IdPacienteNavigation.Estado.ToUpper().Equals(siglaEstado.ToUpper()) &&
						 e.IdVirusBacteria == idVirusBacteria &&
						 e.DataExame >= dataInicio && e.DataExame <= dataFim).OrderByDescending(e => e.DataExame)
				 .Select(exame => new MonitoraPacienteViewModel
				 {
					 Bairro = exame.IdPacienteNavigation.Bairro,
					 Cancer = Convert.ToBoolean(exame.IdPacienteNavigation.Cancer),
					 Cardiopatia = Convert.ToBoolean(exame.IdPacienteNavigation.Cardiopatia),
					 Cep = exame.IdPacienteNavigation.Cep,
					 Cidade = exame.IdPacienteNavigation.Cidade,
					 Complemento = exame.IdPacienteNavigation.Complemento,
					 Cpf = exame.IdPacienteNavigation.Cpf,
					 DataNascimento = exame.IdPacienteNavigation.DataNascimento,
					 Diabetes = Convert.ToBoolean(exame.IdPacienteNavigation.Diabetes),
					 DoencaRespiratoria = Convert.ToBoolean(exame.IdPacienteNavigation.DoencaRespiratoria),
					 Email = exame.IdPacienteNavigation.Email,
					 Estado = exame.IdPacienteNavigation.Estado,
					 FoneCelular = exame.IdPacienteNavigation.FoneCelular,
					 FoneFixo = exame.IdPacienteNavigation.FoneFixo,
					 Hipertenso = Convert.ToBoolean(exame.IdPacienteNavigation.Hipertenso),
					 Idpessoa = exame.IdPacienteNavigation.Idpessoa,
					 Imunodeprimido = Convert.ToBoolean(exame.IdPacienteNavigation.Imunodeprimido),
					 Latitude = exame.IdPacienteNavigation.Latitude,
					 Longitude = exame.IdPacienteNavigation.Longitude,
					 Nome = exame.IdPacienteNavigation.Nome,
					 Numero = exame.IdPacienteNavigation.Numero,
					 Obeso = Convert.ToBoolean(exame.IdPacienteNavigation.Obeso),
					 OutrasComorbidades = exame.IdPacienteNavigation.OutrasComorbidades,
					 Rua = exame.IdPacienteNavigation.Rua,
					 Sexo = exame.IdPacienteNavigation.Sexo,
					 SituacaoSaude = exame.IdPacienteNavigation.SituacaoSaude,
					 DataExame = exame.DataExame,
					 IdExame = exame.IdExame
				 }).ToList();
			List<MonitoraPacienteViewModel> listaMonitoramentoNaoNegativos = BuscarNaoNegativos(idVirusBacteria, monitoraPacientes);
			return listaMonitoramentoNaoNegativos;
		}

		private List<MonitoraPacienteViewModel> BuscarNaoNegativos(int idVirusBacteria, List<MonitoraPacienteViewModel> monitoraPacientes)
		{
			var listaMonitoramentoNaoNegativos = new List<MonitoraPacienteViewModel>();
			if (monitoraPacientes.Count() > 0)
			{
				var virus = _context.Virusbacteria.Where(v => v.IdVirusBacteria == idVirusBacteria).First();
				var virusBacteria = new VirusBacteriaModel() { IdVirusBacteria = virus.IdVirusBacteria, DiasRecuperacao = virus.DiasRecuperacao, Nome = virus.Nome };
				HashSet<int> idPacientes = new HashSet<int>();
				foreach (MonitoraPacienteViewModel paciente in monitoraPacientes)
				{
					var situacaoVirus = _context.Situacaopessoavirusbacteria.Where(s => s.Idpessoa == paciente.Idpessoa && s.IdVirusBacteria == idVirusBacteria).FirstOrDefault();
					if (situacaoVirus != null)
					{
						if (situacaoVirus.UltimaSituacaoSaude != "N")
						{
							if (!idPacientes.Contains(paciente.Idpessoa))
							{
								idPacientes.Add(paciente.Idpessoa);
								paciente.UltimaSituacao = UltimaSituacaoExameDescricao(situacaoVirus.UltimaSituacaoSaude);
								paciente.Descricao = situacaoVirus.Descricao;
								paciente.DataUltimoMonitoramento = situacaoVirus.DataUltimoMonitoramento;
								if (situacaoVirus.IdGestor == null)
									paciente.Gestor = new PessoaModel() { Nome = "-" };
								else
									paciente.Gestor = new PessoaModel() { Nome = _context.Pessoa.Where(p => p.Idpessoa == situacaoVirus.IdGestor).First().Nome };
								paciente.VirusBacteria = virusBacteria;
								listaMonitoramentoNaoNegativos.Add(paciente);
							}
						}
					}
				}
			}

			return listaMonitoramentoNaoNegativos;
		}

		public List<MonitoraPacienteViewModel> GetByEstadoResidenciaPaciente(string siglaEstado,
			int idVirusBacteria, DateTime dataInicio, DateTime dataFim)
		{
			var monitoraPacientes = _context.Exame
				 .Where(e => e.IdPacienteNavigation.Estado.ToUpper().Equals(siglaEstado.ToUpper()) &&
						 e.IdVirusBacteria == idVirusBacteria &&
						 e.DataExame >= dataInicio && e.DataExame <= dataFim).OrderByDescending(e=> e.DataExame)
				 .Select(exame => new MonitoraPacienteViewModel
				 {
					 Bairro = exame.IdPacienteNavigation.Bairro,
					 Cancer = Convert.ToBoolean(exame.IdPacienteNavigation.Cancer),
					 Cardiopatia = Convert.ToBoolean(exame.IdPacienteNavigation.Cardiopatia),
					 Cep = exame.IdPacienteNavigation.Cep,
					 Cidade = exame.IdPacienteNavigation.Cidade,
					 Complemento = exame.IdPacienteNavigation.Complemento,
					 Cpf = exame.IdPacienteNavigation.Cpf,
					 DataNascimento = exame.IdPacienteNavigation.DataNascimento,
					 Diabetes = Convert.ToBoolean(exame.IdPacienteNavigation.Diabetes),
					 DoencaRespiratoria = Convert.ToBoolean(exame.IdPacienteNavigation.DoencaRespiratoria),
					 Email = exame.IdPacienteNavigation.Email,
					 Estado = exame.IdPacienteNavigation.Estado,
					 FoneCelular = exame.IdPacienteNavigation.FoneCelular,
					 FoneFixo = exame.IdPacienteNavigation.FoneFixo,
					 Hipertenso = Convert.ToBoolean(exame.IdPacienteNavigation.Hipertenso),
					 Idpessoa = exame.IdPacienteNavigation.Idpessoa,
					 Imunodeprimido = Convert.ToBoolean(exame.IdPacienteNavigation.Imunodeprimido),
					 Latitude = exame.IdPacienteNavigation.Latitude,
					 Longitude = exame.IdPacienteNavigation.Longitude,
					 Nome = exame.IdPacienteNavigation.Nome,
					 Numero = exame.IdPacienteNavigation.Numero,
					 Obeso = Convert.ToBoolean(exame.IdPacienteNavigation.Obeso),
					 OutrasComorbidades = exame.IdPacienteNavigation.OutrasComorbidades,
					 Rua = exame.IdPacienteNavigation.Rua,
					 Sexo = exame.IdPacienteNavigation.Sexo,
					 SituacaoSaude = exame.IdPacienteNavigation.SituacaoSaude,
					 DataExame = exame.DataExame,
					 IdExame = exame.IdExame
				 }).ToList();
			List<MonitoraPacienteViewModel> listaMonitoramentoNaoNegativos = BuscarNaoNegativos(idVirusBacteria, monitoraPacientes);
			return listaMonitoramentoNaoNegativos;
		}

		private string UltimaSituacaoExameDescricao(string situacao)
		{
			switch (situacao)
			{
				case "P":
					return "Positivo";
				case "C":
					return "Imunizado";
				case "I":
					return "Indeterminado";

				default: return "Indeterminado";
			}
		}

		public List<ExameModel> CheckDuplicateExamToday(int idPaciente, int idVirusBacteria, DateTime dateExame)
        {
            var exames = _context.Exame.Where(exameModel => exameModel.IdVirusBacteria == idVirusBacteria &&
                         exameModel.IdPaciente == idPaciente && dateExame.ToString("dd/MM/yyyy").Equals(exameModel.DataExame.ToString("dd/MM/yyyy")))
                         .Select(exame => new ExameModel
                         {
                             IdExame = exame.IdExame,
                         }).ToList();


            return exames;
        }

		public List<TotalEstadoMunicipioBairro> GetTotaisPopulacaoByEstado(string siglaEstado)
            => ConvertToEstadoMunicipioBairro(
            _context.Exame
                 .Where(exameModel => exameModel.IdPacienteNavigation.Estado.Equals(siglaEstado))
                 .Select(exame => new ExameCompletoModel
                 {
                     IdVirusBacteria = exame.IdVirusBacteria,
                     IdExame = exame.IdExame,
                     IdPaciente = exame.IdPaciente,
                     IdAgenteSaude = exame.IdAgenteSaude,
                     DataExame = exame.DataExame,
                     DataInicioSintomas = exame.DataInicioSintomas,
                     IgG = exame.IgG,
                     IgM = exame.IgM,
                     Pcr = exame.Pcr,
                     IdEstado = exame.IdEstado,
                     IdMunicipio = exame.IdMunicipio,
                     IdEmpresaSaude = exame.IdEmpresaSaude,
                     UF = exame.IdPacienteNavigation.Estado,
                     Municipio = exame.IdPacienteNavigation.Cidade,
                     Bairro = ""
                 }).ToList().GroupBy(e => new { Estado = e.UF, Municipio = e.Municipio, Resultado = e.Resultado })
                 .Select(g => new TotalPorResultadoExame
                 {
                     Estado = g.Key.Estado,
                     Municipio = g.Key.Municipio,
                     IdEmpresaSaude = EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO,
                     Resultado = g.Key.Resultado,
                     Bairro = "",
                     Total = g.Count()
                 }).ToList());
		public List<TotalEstadoMunicipioBairro> GetTotaisPopulacaoByMunicipio(string siglaEstado, string cidade)
			=> ConvertToEstadoMunicipioBairro(
			_context.Exame
				 .Where(exameModel => exameModel.IdPacienteNavigation.Estado.Equals(siglaEstado) &&
					exameModel.IdPacienteNavigation.Cidade.Equals(cidade))
				 .Select(exame => new ExameCompletoModel
				 {
					 IdVirusBacteria = exame.IdVirusBacteria,
					 IdExame = exame.IdExame,
					 IdPaciente = exame.IdPaciente,
					 IdAgenteSaude = exame.IdAgenteSaude,
					 DataExame = exame.DataExame,
					 DataInicioSintomas = exame.DataInicioSintomas,
					 IgG = exame.IgG,
					 IgM = exame.IgM,
					 Pcr = exame.Pcr,
					 IdEstado = exame.IdEstado,
					 IdMunicipio = exame.IdMunicipio,
					 IdEmpresaSaude = exame.IdEmpresaSaude,
					 UF = exame.IdPacienteNavigation.Estado,
					 Municipio = exame.IdPacienteNavigation.Cidade,
					 Bairro = exame.IdPacienteNavigation.Bairro.ToUpper().Trim()
				 }).ToList().GroupBy(e => new { Estado = e.UF, Municipio = e.Municipio, Bairro = e.Bairro, Resultado = e.Resultado })
				 .Select(g => new TotalPorResultadoExame
				 {
					 Estado = g.Key.Estado,
					 Municipio = g.Key.Municipio,
					 IdEmpresaSaude = EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO,
					 Resultado = g.Key.Resultado,
					 Bairro = g.Key.Bairro,
					 Total = g.Count()
				 }).ToList());


		public List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByEstado(int idEstado)
            => ConvertToEstadoMunicipioBairro(
            _context.Exame
                 .Where(exameModel => exameModel.IdEstado == idEstado && exameModel.IdEmpresaSaude == EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO)
                 .Select(exame => new ExameCompletoModel
                 {
                     IdVirusBacteria = exame.IdVirusBacteria,
                     IdExame = exame.IdExame,
                     IdPaciente = exame.IdPaciente,
                     IdAgenteSaude = exame.IdAgenteSaude,
                     DataExame = exame.DataExame,
                     DataInicioSintomas = exame.DataInicioSintomas,
                     IgG = exame.IgG,
                     IgM = exame.IgM,
                     Pcr = exame.Pcr,
                     IdEstado = exame.IdEstado,
                     IdMunicipio = exame.IdMunicipio,
                     IdEmpresaSaude = exame.IdEmpresaSaude,
                     UF = exame.IdPacienteNavigation.Estado,
                     Municipio = exame.IdPacienteNavigation.Cidade,
                     Bairro = ""
                 }).ToList().GroupBy(e => new { Estado = e.UF, Municipio = e.Municipio, Resultado = e.Resultado })
                 .Select(g => new TotalPorResultadoExame
                 {
                     Estado = g.Key.Estado,
                     Municipio = g.Key.Municipio,
                     IdEmpresaSaude = EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO,
                     Resultado = g.Key.Resultado,
                     Bairro = "",
                     Total = g.Count()
                 }).ToList());

        public List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByMunicipio(int idMunicipio)
            => ConvertToEstadoMunicipioBairro(
            _context.Exame
                 .Where(exameModel => exameModel.IdMunicipio == idMunicipio)
                 .Select(exame => new ExameCompletoModel
                 {
                     IdVirusBacteria = exame.IdVirusBacteria,
                     IdExame = exame.IdExame,
                     IdPaciente = exame.IdPaciente,
                     IdAgenteSaude = exame.IdAgenteSaude,
                     DataExame = exame.DataExame,
                     DataInicioSintomas = exame.DataInicioSintomas,
                     IgG = exame.IgG,
                     IgM = exame.IgM,
                     Pcr = exame.Pcr,
                     IdEstado = exame.IdEstado,
                     IdMunicipio = exame.IdMunicipio,
                     IdEmpresaSaude = exame.IdEmpresaSaude,
					 UF = exame.IdPacienteNavigation.Estado,
					 Municipio = exame.IdPacienteNavigation.Cidade,
					 Bairro = exame.IdPacienteNavigation.Bairro,
				 }).ToList().GroupBy(e => new { Estado = e.UF, Municipio = e.Municipio, Bairro = e.Bairro, Resultado = e.Resultado })
                 .Select(g => new TotalPorResultadoExame
                 {
                     Estado = g.Key.Estado,
                     Municipio = g.Key.Municipio,
                     IdEmpresaSaude = EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO,
                     Resultado = g.Key.Resultado,
                     Bairro = g.Key.Bairro,
                     Total = g.Count()
                 }).ToList());

        public List<TotalEstadoMunicipioBairro> GetTotaisRealizadosByEmpresa(int idEempresa)
        => ConvertToEstadoMunicipioBairro(
            _context.Exame
                 .Where(exameModel => exameModel.IdEmpresaSaude == idEempresa)
                 .Select(exame => new ExameCompletoModel
                 {
                     IdVirusBacteria = exame.IdVirusBacteria,
                     IdExame = exame.IdExame,
                     IdPaciente = exame.IdPaciente,
                     IdAgenteSaude = exame.IdAgenteSaude,
                     DataExame = exame.DataExame,
                     DataInicioSintomas = exame.DataInicioSintomas,
                     IgG = exame.IgG,
                     IgM = exame.IgM,
                     Pcr = exame.Pcr,
                     IdEstado = exame.IdEstado,
                     IdMunicipio = exame.IdMunicipio,
                     IdEmpresaSaude = exame.IdEmpresaSaude,
					 UF = exame.IdPacienteNavigation.Estado,
					 Municipio = exame.IdPacienteNavigation.Cidade,
					 Bairro = ""
                 }).ToList().GroupBy(e => new { Estado = e.UF, Municipio = e.Municipio, Resultado = e.Resultado })
                 .Select(g => new TotalPorResultadoExame
                 {
                     Estado = g.Key.Estado,
                     Municipio = g.Key.Municipio,
                     IdEmpresaSaude = idEempresa,
                     Resultado = g.Key.Resultado,
                     Bairro = "",
                     Total = g.Count()
                 }).ToList());

        private List<TotalEstadoMunicipioBairro> ConvertToEstadoMunicipioBairro(List<TotalPorResultadoExame> resultados)
        {
            List<TotalEstadoMunicipioBairro> totalEstadoMunicipioBairros = new List<TotalEstadoMunicipioBairro>();
            foreach (TotalPorResultadoExame totalPorResultado in resultados)
            {
                bool achou = false;
                foreach (TotalEstadoMunicipioBairro totalEMB in totalEstadoMunicipioBairros)
                {
                    if (totalEMB.Estado.Equals(totalPorResultado.Estado) && totalEMB.Municipio.Equals(totalPorResultado.Municipio) &&
                       (totalEMB.IdEmpresaSaude == totalPorResultado.IdEmpresaSaude) && totalEMB.Bairro.Equals(totalPorResultado.Bairro))
                    {
                       AtualizarTotais(totalPorResultado, totalEMB);
                       achou = true;
                       break;
                    }
                }
                if (!achou)
                {
                    TotalEstadoMunicipioBairro totalEMB = new TotalEstadoMunicipioBairro()
                    {
                        Bairro = totalPorResultado.Bairro,
                        Estado = totalPorResultado.Estado,
                        Municipio = totalPorResultado.Municipio,
                        IdEmpresaSaude = totalPorResultado.IdEmpresaSaude,
                        TotalImunizados = 0,
                        TotalIndeterminados = 0,
                        TotalNegativos = 0,
                        TotalPositivos = 0
                    };
                    AtualizarTotais(totalPorResultado, totalEMB);
                    totalEstadoMunicipioBairros.Add(totalEMB);
                }
            }
            return totalEstadoMunicipioBairros;
        }


        private void AtualizarTotais(TotalPorResultadoExame totalPorResultado, TotalEstadoMunicipioBairro totalEMB)
        {
            if (totalPorResultado.Resultado.Equals(ExameModel.RESULTADO_POSITIVO))
                totalEMB.TotalPositivos += totalPorResultado.Total;
            if (totalPorResultado.Resultado.Equals(ExameModel.RESULTADO_NEGATIVO))
                totalEMB.TotalNegativos += totalPorResultado.Total;
            if (totalPorResultado.Resultado.Equals(ExameModel.RESULTADO_IMUNIZADO))
                totalEMB.TotalImunizados += totalPorResultado.Total;
            if (totalPorResultado.Resultado.Equals(ExameModel.RESULTADO_INDETERMINADO))
                totalEMB.TotalIndeterminados += totalPorResultado.Total;
        }

        public List<ExameModel> GetByIdEstado(int idEstado)
        => _context.Exame
               .Where(exameModel => exameModel.IdEstado == idEstado)
               .Select(exame => new ExameModel
               {
                   IdVirusBacteria = exame.IdVirusBacteria,
                   IdExame = exame.IdExame,
                   IdPaciente = exame.IdPaciente,
                   IdAgenteSaude = exame.IdAgenteSaude,
                   DataExame = exame.DataExame,
                   DataInicioSintomas = exame.DataInicioSintomas,
                   IgG = exame.IgG,
                   IgM = exame.IgM,
                   Pcr = exame.Pcr,
                   IdEstado = exame.IdEstado,
                   IdMunicipio = exame.IdMunicipio,
                   IdEmpresaSaude = exame.IdEmpresaSaude,
                   EhProfissionalSaude = Convert.ToBoolean(exame.EhProfissionalSaude),
                   CodigoColeta = exame.CodigoColeta,
                   StatusNotificacao = exame.StatusNotificacao,
                   IdNotificacao = exame.IdNotificacao,
               }).ToList();
    }
}
