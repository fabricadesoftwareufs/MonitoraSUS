using Model;
using Persistence;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository
{
    public class ExameRepository : IExameRepository
    {
        private readonly monitorasusContext _context;
        public ExameRepository(monitorasusContext context)
        {
            _context = context;
        }

        public bool Insert(ExameViewModel exameModel)
        {
            if (exameModel != null)
            {
                try
                {
                    _context.Exame.Add(ModelToEntity(exameModel));
                    return _context.SaveChanges() == 1;
                }
                catch (Exception e)
                {
                    throw e.InnerException;
                }
            }
            return false;
        }

        public monitorasusContext GetContext() => _context;

        public List<ExameModel> GetExamesRelizadosData(int idPaciente, int idVirusBacteria, DateTime dateExame, string metodoExame)
         => _context.Exame.Where(exameModel => exameModel.IdVirusBacteria == idVirusBacteria &&
                exameModel.IdPaciente == idPaciente && exameModel.MetodoExame.Equals(metodoExame) &&
                dateExame.ToString("dd/MM/yyyy").Equals(exameModel.DataExame.ToString("dd/MM/yyyy")))
                .Select(exame => new ExameModel
                {
                    IdExame = exame.IdExame,
                }).ToList();


        // METODOS PRIVADOS
        private Exame ModelToEntity(ExameViewModel exameModel)
        {
            exameModel.Exame.CodigoColeta = (exameModel.Exame.CodigoColeta == null) ? "" : exameModel.Exame.CodigoColeta;
            exameModel.Exame.IdNotificacao = (exameModel.Exame.IdNotificacao == null) ? "" : exameModel.Exame.IdNotificacao;
            var secretarioMunicipio = _context.Pessoatrabalhamunicipio.Where(p => p.IdPessoa == exameModel.Exame.IdAgenteSaude).FirstOrDefault();

            if (secretarioMunicipio != null)
            {
                exameModel.Exame.IdMunicipio = secretarioMunicipio.IdMunicipio;
                exameModel.Exame.IdEstado = Convert.ToInt32(secretarioMunicipio.IdMunicipioNavigation.Uf);
                exameModel.Exame.IdEmpresaSaude = 1; // empresa padrão do banco 
            }
            else
            {
                var secretarioEstado = _context.Pessoatrabalhaestado.Where(p => p.Idpessoa == exameModel.Exame.IdAgenteSaude).FirstOrDefault();
                exameModel.Exame.IdEstado = secretarioEstado.IdEstado;
                exameModel.Exame.IdEmpresaSaude = secretarioEstado.IdEmpresaExame;
                exameModel.Exame.IdMunicipio = null;
            }
            return new Exame
            {
                IdAreaAtuacao = exameModel.Exame.IdAreaAtuacao,
                IdExame = exameModel.Exame.IdExame,
                IdAgenteSaude = exameModel.Usuario.IdPessoa,
                IdPaciente = exameModel.Paciente.Idpessoa,
                IdVirusBacteria = exameModel.Exame.IdVirusBacteria,
                IgG = exameModel.Exame.IgG,
                IgM = exameModel.Exame.IgM,
                Pcr = exameModel.Exame.Pcr,
                IgMigG = exameModel.Exame.IgGIgM,
                MetodoExame = exameModel.Exame.MetodoExame,
                IdEstado = exameModel.Exame.IdEstado,
                IdMunicipio = exameModel.Exame.IdMunicipio,
                DataInicioSintomas = exameModel.Exame.DataInicioSintomas,
                DataExame = exameModel.Exame.DataExame,
                IdEmpresaSaude = exameModel.Exame.IdEmpresaSaude,
                CodigoColeta = exameModel.Exame.CodigoColeta,
                StatusNotificacao = exameModel.Exame.StatusNotificacao,
                IdNotificacao = exameModel.Exame.IdNotificacao,
                DataNotificacao = DateTime.Now,
                AguardandoResultado = Convert.ToByte(exameModel.Exame.AguardandoResultado),
                Coriza = Convert.ToByte(exameModel.Exame.Coriza),
                Nausea = Convert.ToByte(exameModel.Exame.Nausea),
                Tosse = Convert.ToByte(exameModel.Exame.Tosse),
                PerdaOlfatoPaladar = Convert.ToByte(exameModel.Exame.PerdaOlfatoPaladar),
                RelatouSintomas = Convert.ToByte(exameModel.Exame.RelatouSintomas),
                Diarreia = Convert.ToByte(exameModel.Exame.Diarreia),
                DificuldadeRespiratoria = Convert.ToByte(exameModel.Exame.DificuldadeRespiratoria),
                DorAbdominal = Convert.ToByte(exameModel.Exame.DorAbdominal),
                DorGarganta = Convert.ToByte(exameModel.Exame.DorGarganta),
                DorOuvido = Convert.ToByte(exameModel.Exame.DorOuvido),
                Febre = Convert.ToByte(exameModel.Exame.Febre),
                OutroSintomas = exameModel.Exame.OutrosSintomas,
            };
        }
    }
}
