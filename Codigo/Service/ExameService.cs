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

        public async System.Threading.Tasks.Task<bool> EnviarSMSResultadoExameAsync(ConfiguracaoNotificarModel configuracaoNotificar, ExameModel exame, PessoaModel pessoa)
        {
            try
            {
                string mensagem = "[MonitoraSUS] Paciente: " + pessoa.Nome + ". ";
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
            }
            catch (HttpRequestException)
            {
                return false;
            }
            return true;
        }

		public async System.Threading.Tasks.Task<bool> ConsultarSMSExameAsync(ConfiguracaoNotificarModel configuracaoNotificar, ExameModel exame)
		{
			try
			{
				var cliente = new HttpClient();
				string url = "https://api.smsdev.com.br/get?key=" + configuracaoNotificar.Token + "&action=status&";
				var uri = url + "id=" + exame.IdNotificacao;
				var resultadoEnvio = await cliente.GetStringAsync(uri);
				//ConsultaSMSModel consulta = JsonConvert.DeserializeObject<ConsultaSMSModel>(resultadoEnvio);
				//if (consulta.Situacao.Equals(ConsultaSMSModel.SITUACAO_ENTREGUE)) {
				if (resultadoEnvio.Contains("RECEBIDA")) { 
					exame.StatusNotificacao = ExameModel.NOTIFICADO_SIM;
					Update(exame);
				}
			}
			catch (HttpRequestException)
			{
				return false;
			}
			return true;
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
