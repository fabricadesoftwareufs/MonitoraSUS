using Model;
using Model.AuxModel;
using Newtonsoft.Json;
using Persistence;
using Service.Interface;
using System.Linq;
using System.Net.Http;

namespace Service
{
    public class SmsService : ISmsService
    {
        private readonly monitorasusContext _context;

        public SmsService(monitorasusContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task<ExameModel> EnviarSMSResultadoExameAsync(PessoaTrabalhaEstadoModel trabalhaEstado,
            PessoaTrabalhaMunicipioModel trabalhaMunicipio, ExameModel exame, PessoaModel pessoa)
        {
            ConfiguracaoNotificarModel configuracaoNotificar = BuscarConfiguracaoSMS(trabalhaEstado, trabalhaMunicipio, exame);
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
                ResponseSMSModel jsonResponse = JsonConvert.DeserializeObject<ResponseSMSModel>(resultadoEnvio);
                exame.IdNotificacao = jsonResponse.Id.ToString();
                exame.StatusNotificacao = ExameModel.NOTIFICADO_ENVIADO;

                _context.Update(exame);
                _context.SaveChanges();

                
                Configuracaonotificar configura = _context.Configuracaonotificar.Where(s => s.IdConfiguracaoNotificar == configuracaoNotificar.IdConfiguracaoNotificar).FirstOrDefault();
                if (configura != null)
                {
                    configura.QuantidadeSmsdisponivel -= 1;
                    _context.Update(configura);
                }
                return exame;
            }
            catch (HttpRequestException)
            {
                return exame;
            }
        }

        private ConfiguracaoNotificarModel BuscarConfiguracaoSMS(PessoaTrabalhaEstadoModel trabalhaEstado, PessoaTrabalhaMunicipioModel trabalhaMunicipio, ExameModel exame)
        {
            ConfiguracaoNotificarModel configuracaoNotificar = null;
            if (trabalhaEstado != null)
            {
                configuracaoNotificar = BuscarConfiguracaoNotificar(trabalhaEstado.IdEstado, trabalhaEstado.IdEmpresaExame);
            }
            else if (trabalhaMunicipio != null)
            {
                configuracaoNotificar = BuscarConfiguracaoNotificar(trabalhaMunicipio.IdMunicipio);
            }
            if (configuracaoNotificar == null)
            {
                throw new ServiceException("Não possui créditos para notificações por SMS. Por favor entre em contato pelo email fabricadesoftware@ufs.br para saber como usar esse serviço no MonitoraSUS.");
            }
            else if (configuracaoNotificar.QuantidadeSmsdisponivel == 0 && exame.StatusNotificacao.Equals(ExameModel.NOTIFICADO_NAO))
            {

                throw new ServiceException("Não possui créditos para enviar SMS. " +
                        "Por favor entre em contato pelo email fabricadesoftware@ufs.br se precisar novos créditos.");
            }

            return configuracaoNotificar;
        }

        public async System.Threading.Tasks.Task<ExameModel> ConsultarSMSExameAsync(PessoaTrabalhaEstadoModel trabalhaEstado,
            PessoaTrabalhaMunicipioModel trabalhaMunicipio, ExameModel exame)
        {
            ConfiguracaoNotificarModel configuracaoNotificar = BuscarConfiguracaoSMS(trabalhaEstado, trabalhaMunicipio, exame);
            try
            {
                var cliente = new HttpClient();
                string url = "https://api.smsdev.com.br/get?key=" + configuracaoNotificar.Token + "&action=status&";
                var uri = url + "id=" + exame.IdNotificacao;
                var resultadoEnvio = await cliente.GetStringAsync(uri);

                ConsultaSMSModel jsonResponse = JsonConvert.DeserializeObject<ConsultaSMSModel>(resultadoEnvio);
                if (jsonResponse.Descricao.Equals("RECEBIDA"))
                {
                    exame.StatusNotificacao = ExameModel.NOTIFICADO_SIM;
                    _context.Update(exame);
                    _context.SaveChanges();
                }
                else if (jsonResponse.Descricao.Equals("ERRO"))
                {
                    exame.StatusNotificacao = ExameModel.NOTIFICADO_PROBLEMAS;
                    _context.Update(exame);
                    _context.SaveChanges();
                }
            }
            catch (HttpRequestException)
            {
                return exame;
            }
            return exame;
        }

        private ConfiguracaoNotificarModel BuscarConfiguracaoNotificar(int IdEstado, int IdEmpresaExame)
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
                    MensagemCurado = conf.MensagemImunizado,
                    MensagemIndeterminado = conf.MensagemIndeterminado,
                    MensagemPositivo = conf.MensagemPositivo,
                    MensagemNegativo = conf.MensagemNegativo,
                    QuantidadeSmsdisponivel = conf.QuantidadeSmsdisponivel,
                    Sid = conf.Sid,
                    Token = conf.Token
                }).FirstOrDefault();

        private ConfiguracaoNotificarModel BuscarConfiguracaoNotificar(int IdMunicipio)
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
                    MensagemCurado = conf.MensagemImunizado,
                    MensagemIndeterminado = conf.MensagemIndeterminado,
                    MensagemPositivo = conf.MensagemPositivo,
                    MensagemNegativo = conf.MensagemNegativo,
                    QuantidadeSmsdisponivel = conf.QuantidadeSmsdisponivel,
                    Sid = conf.Sid,
                    Token = conf.Token
                }).FirstOrDefault();
    }
}
