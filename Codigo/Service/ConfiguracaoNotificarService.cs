using Model;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class ConfiguracaoNotificarService : IConfiguracaoNotificarService
    {
        private readonly monitorasusContext _context;
        
        public ConfiguracaoNotificarService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Delete(int id)
        {
            try
            {
                var configuracaoNotificar = _context.Configuracaonotificar.Find(id);
                if(configuracaoNotificar != null)
                    _context.Configuracaonotificar.Remove(configuracaoNotificar);
                return _context.SaveChanges() == 1 ? true : false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception ({0}) occurred.",
                ex.GetType().Name);
                Console.WriteLine("   Message:\n{0}", ex.Message);
                Console.WriteLine("   Stack Trace:\n   {0}", ex.StackTrace);
                throw ex.InnerException;
            }
        }

        public List<ConfiguracaoNotificarModel> GetAll()
            => _context.Configuracaonotificar
            .Select(model => new ConfiguracaoNotificarModel
            {
                IdConfiguracaoNotificar = model.IdConfiguracaoNotificar,
                HabilitadoSms = Convert.ToBoolean(model.HabilitadoSms),
                HabilitadoWhatsapp = Convert.ToBoolean(model.HabilitadoWhatsapp),
                IdEmpresaExame = model.IdEmpresaExame,
                IdEstado = model.IdEstado,
                IdMunicipio = model.IdMunicipio,
                MensagemImunizado = model.MensagemImunizado,
                MensagemIndeterminado = model.MensagemIndeterminado,
                MensagemNegativo = model.MensagemNegativo,
                MensagemPositivo = model.MensagemPositivo,
                Sid = model.Sid,
                Token = model.Token,
                NumeroSms = model.NumeroSms,
                NumeroWhatsapp = model.NumeroWhatsapp
            }).ToList();


        public ConfiguracaoNotificarModel GetById(int id)
         => _context.Configuracaonotificar
            .Where(m => m.IdConfiguracaoNotificar == id)
            .Select(model => new ConfiguracaoNotificarModel
            {
                IdConfiguracaoNotificar = model.IdConfiguracaoNotificar,
                HabilitadoSms = Convert.ToBoolean(model.HabilitadoSms),
                HabilitadoWhatsapp = Convert.ToBoolean(model.HabilitadoWhatsapp),
                IdEmpresaExame = model.IdEmpresaExame,
                IdEstado = model.IdEstado,
                IdMunicipio = model.IdMunicipio,
                MensagemImunizado = model.MensagemImunizado,
                MensagemIndeterminado = model.MensagemIndeterminado,
                MensagemNegativo = model.MensagemNegativo,
                MensagemPositivo = model.MensagemPositivo,
                Sid = model.Sid,
                Token = model.Token,
                NumeroSms = model.NumeroSms,
                NumeroWhatsapp = model.NumeroWhatsapp
            }).FirstOrDefault();

        public ConfiguracaoNotificarModel GetByIdEstado(int idEstado)
        => _context.Configuracaonotificar
            .Where(m => m.IdEstado == idEstado)
            .Select(model => new ConfiguracaoNotificarModel
            {
                IdConfiguracaoNotificar = model.IdConfiguracaoNotificar,
                HabilitadoSms = Convert.ToBoolean(model.HabilitadoSms),
                HabilitadoWhatsapp = Convert.ToBoolean(model.HabilitadoWhatsapp),
                IdEmpresaExame = model.IdEmpresaExame,
                IdEstado = model.IdEstado,
                IdMunicipio = model.IdMunicipio,
                MensagemImunizado = model.MensagemImunizado,
                MensagemIndeterminado = model.MensagemIndeterminado,
                MensagemNegativo = model.MensagemNegativo,
                MensagemPositivo = model.MensagemPositivo,
                Sid = model.Sid,
                Token = model.Tokenm
                NumeroSms = model.NumeroSms,
                NumeroWhatsapp = model.NumeroWhatsapp
            }).FirstOrDefault();

        public ConfiguracaoNotificarModel GetByIdIdEmpresaExame(int idEmpresaExame)
        => _context.Configuracaonotificar
            .Where(m => m.IdEmpresaExame == idEmpresaExame)
            .Select(model => new ConfiguracaoNotificarModel
            {
                IdConfiguracaoNotificar = model.IdConfiguracaoNotificar,
                HabilitadoSms = Convert.ToBoolean(model.HabilitadoSms),
                HabilitadoWhatsapp = Convert.ToBoolean(model.HabilitadoWhatsapp),
                IdEmpresaExame = model.IdEmpresaExame,
                IdEstado = model.IdEstado,
                IdMunicipio = model.IdMunicipio,
                MensagemImunizado = model.MensagemImunizado,
                MensagemIndeterminado = model.MensagemIndeterminado,
                MensagemNegativo = model.MensagemNegativo,
                MensagemPositivo = model.MensagemPositivo,
                Sid = model.Sid,
                Token = model.Token,
                NumeroSms = model.NumeroSms,
                NumeroWhatsapp = model.NumeroWhatsapp
            }).FirstOrDefault();

        public ConfiguracaoNotificarModel GetByIdMunicipio(int idMunicipio)
        => _context.Configuracaonotificar
            .Where(m => m.IdMunicipio == idMunicipio)
            .Select(model => new ConfiguracaoNotificarModel
            {
                IdConfiguracaoNotificar = model.IdConfiguracaoNotificar,
                HabilitadoSms = Convert.ToBoolean(model.HabilitadoSms),
                HabilitadoWhatsapp = Convert.ToBoolean(model.HabilitadoWhatsapp),
                IdEmpresaExame = model.IdEmpresaExame,
                IdEstado = model.IdEstado,
                IdMunicipio = model.IdMunicipio,
                MensagemImunizado = model.MensagemImunizado,
                MensagemIndeterminado = model.MensagemIndeterminado,
                MensagemNegativo = model.MensagemNegativo,
                MensagemPositivo = model.MensagemPositivo,
                Sid = model.Sid,
                Token = model.Token,
                NumeroSms = model.NumeroSms,
                NumeroWhatsapp = model.NumeroWhatsapp
            }).FirstOrDefault();

        public bool Insert(ConfiguracaoNotificarModel configuracaoNotificarModel)
        {
            try
            {
                _context.Configuracaonotificar.Add(ModelToEntity(configuracaoNotificarModel));
                return _context.SaveChanges() == 1 ? true : false;
            }
            catch(Exception ex){
                Console.WriteLine("An exception ({0}) occurred.",
                ex.GetType().Name);
                Console.WriteLine("   Message:\n{0}", ex.Message);
                Console.WriteLine("   Stack Trace:\n   {0}", ex.StackTrace);
                throw ex.InnerException;
            }
        }

        public bool Update(ConfiguracaoNotificarModel configuracaoNotificarModel)
        {
            try
            {
                _context.Configuracaonotificar.Update(ModelToEntity(configuracaoNotificarModel));
                return _context.SaveChanges() == 1 ? true : false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception ({0}) occurred.",
                ex.GetType().Name);
                Console.WriteLine("   Message:\n{0}", ex.Message);
                Console.WriteLine("   Stack Trace:\n   {0}", ex.StackTrace);
                throw ex.InnerException;
            }
        }

        private Configuracaonotificar ModelToEntity(ConfiguracaoNotificarModel configuracaoNotificarModel) =>
            new Configuracaonotificar
            {
                IdConfiguracaoNotificar = configuracaoNotificarModel.IdConfiguracaoNotificar,
                HabilitadoSms = Convert.ToByte(configuracaoNotificarModel.HabilitadoSms),
                HabilitadoWhatsapp = Convert.ToByte(configuracaoNotificarModel.HabilitadoWhatsapp),
                IdEmpresaExame = configuracaoNotificarModel.IdEmpresaExame,
                IdEstado = configuracaoNotificarModel.IdEstado,
                IdMunicipio = configuracaoNotificarModel.IdMunicipio,
                MensagemImunizado = configuracaoNotificarModel.MensagemImunizado,
                MensagemIndeterminado = configuracaoNotificarModel.MensagemIndeterminado,
                MensagemNegativo = configuracaoNotificarModel.MensagemNegativo,
                MensagemPositivo = configuracaoNotificarModel.MensagemPositivo,
                Sid = configuracaoNotificarModel.Sid,
                Token = configuracaoNotificarModel.Token,
                NumeroSms = configuracaoNotificarModel.NumeroSms,
                NumeroWhatsapp = configuracaoNotificarModel.NumeroWhatsapp
            };
    }
}
