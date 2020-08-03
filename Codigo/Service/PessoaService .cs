using Model;
using Util;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Model.ViewModel;

namespace Service
{
    public class PessoaService : IPessoaService
    {
        private readonly monitorasusContext _context;
        public PessoaService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Delete(int id)
        {
            var pessoa = _context.Pessoa.Find(id);
            _context.Pessoa.Remove(pessoa);
            return _context.SaveChanges() == 1 ? true : false;
        }

        public PessoaModel Update(PessoaModel pessoaModel, bool atualizaSintomas)
        {
            if (pessoaModel != null)
            {
                try
                {
                    var pessoaInserida = new Pessoa();
					
					if (atualizaSintomas)
						_context.Update(ModelToEntity(pessoaModel, pessoaInserida));
					else
						_context.Update(ModelToEntitySemSintomas(pessoaModel, pessoaInserida));

                    if (string.IsNullOrWhiteSpace(pessoaModel.Cpf))
                    {
                        pessoaModel.Cpf = "T" + DateTime.Now.Ticks.ToString().Substring(12);
                        _context.SaveChanges();
                        pessoaInserida.Cpf = "T" + Convert.ToString(pessoaInserida.Idpessoa).PadLeft(8, '0') + pessoaInserida.Estado;
                    }
                    _context.SaveChanges();
					_context.Entry(pessoaInserida).State = EntityState.Detached;

					// Returning the last inserted ID.
					pessoaModel.Cpf = pessoaInserida.Cpf;
                    pessoaModel.Idpessoa = pessoaInserida.Idpessoa;
                    return pessoaModel;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

            return null;
        }

        public PessoaModel Insert(PessoaModel pessoaModel)
        {
            if (pessoaModel != null)
            {
                try
                {
                    var pessoaInserida = new Pessoa();
					_context.Pessoa.Add(ModelToEntity(pessoaModel, pessoaInserida));

                    if (string.IsNullOrWhiteSpace(pessoaModel.Cpf))
                    {
                        pessoaModel.Cpf = "T" + DateTime.Now.Ticks.ToString().Substring(12);
                        _context.SaveChanges();
                        pessoaInserida.Cpf = "T" + Convert.ToString(pessoaInserida.Idpessoa).PadLeft(8, '0') + pessoaInserida.Estado;
                    }
                    _context.SaveChanges();
					_context.Entry(pessoaInserida).State = EntityState.Detached;

					// Returning the last inserted ID.
					pessoaModel.Cpf = pessoaInserida.Cpf;
                    pessoaModel.Idpessoa = pessoaInserida.Idpessoa;
                    return pessoaModel;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

            return null;
        }


		public PessoaModel InsertAgente(PessoaViewModel pessoaViewModel)
		{
			pessoaViewModel.Pessoa.Cpf = Methods.RemoveSpecialsCaracts(pessoaViewModel.Cpf);
			var pessoa = GetByCpf(pessoaViewModel.Pessoa.Cpf);
			if (pessoa != null)
				throw new ServiceException("Já possui um cadastro seu no sistema. Solicite a um Gestor de Saúde " + pessoaViewModel.AreaAtuacao +
					" para autorizar seu CPF para acessar o sistema.");
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{
					pessoa = Insert(pessoaViewModel.Pessoa);
					if (pessoaViewModel.AreaAtuacao.Equals("Municipal"))
					{
						PessoaTrabalhaMunicipioService _pessoaTrabalhaMunicipioService = new PessoaTrabalhaMunicipioService(_context);
						_pessoaTrabalhaMunicipioService
								.Insert(new PessoaTrabalhaMunicipioModel
								{
									IdPessoa = pessoa.Idpessoa,
									IdMunicipio = pessoaViewModel.IdMunicipio,
									SituacaoCadastro = EmpresaExameModel.SITUACAO_CADASTRO_SOLICITADA,
									EhResponsavel = false,
									EhSecretario = false
								});
					}
					else if (pessoaViewModel.AreaAtuacao.Equals("Estadual"))
					{
						PessoaTrabalhaEstadoService _pessoaTrabalhaEstadoService = new PessoaTrabalhaEstadoService(_context);
						_pessoaTrabalhaEstadoService
								.Insert(new PessoaTrabalhaEstadoModel
								{
									IdPessoa = pessoa.Idpessoa,
									IdEstado = Convert.ToInt32(pessoaViewModel.IdEstado),
									SituacaoCadastro = EmpresaExameModel.SITUACAO_CADASTRO_SOLICITADA,
									IdEmpresaExame = EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO,
									EhSecretario = false,
									EhResponsavel = false,
								});
					}
					transaction.Commit();
				} catch (Exception e)
                {
					transaction.Rollback();
					throw e;
				}
			}
			return pessoa;
		}


		public PessoaModel InsertGestor(PessoaViewModel pessoaViewModel)
		{
			pessoaViewModel.Pessoa.Cpf = Methods.RemoveSpecialsCaracts(pessoaViewModel.Cpf);
			var pessoa = GetByCpf(pessoaViewModel.Pessoa.Cpf);
			if (pessoa != null)
				throw new ServiceException("Já possui um cadastro seu no sistema. Solicite a um Gestor de Saúde " + pessoaViewModel.AreaAtuacao +
					" para autorizar seu CPF para acessar o sistema.");

			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{

					pessoa = Insert(pessoaViewModel.Pessoa);

					if (pessoaViewModel.AreaAtuacao.Equals("Municipal"))
					{
						PessoaTrabalhaMunicipioService _pessoaTrabalhaMunicipioService = new PessoaTrabalhaMunicipioService(_context);
						_pessoaTrabalhaMunicipioService
								.Insert(new PessoaTrabalhaMunicipioModel
								{

									IdPessoa = pessoa.Idpessoa,
									IdMunicipio = pessoaViewModel.IdMunicipio,
									SituacaoCadastro = EmpresaExameModel.SITUACAO_CADASTRO_SOLICITADA,
									EhSecretario = false,
									EhResponsavel = true
								});
					}
					else if (pessoaViewModel.AreaAtuacao.Equals("Estadual"))
					{
						PessoaTrabalhaEstadoService _pessoaTrabalhaEstadoService = new PessoaTrabalhaEstadoService(_context);
						_pessoaTrabalhaEstadoService
								.Insert(new PessoaTrabalhaEstadoModel
								{
									IdPessoa = pessoa.Idpessoa,
									IdEstado = pessoaViewModel.IdEstado,
									SituacaoCadastro = EmpresaExameModel.SITUACAO_CADASTRO_SOLICITADA,
									IdEmpresaExame = EmpresaExameModel.EMPRESA_ESTADO_MUNICIPIO,    //valor padrão
									EhSecretario = false,
									EhResponsavel = true
								});
					}
					transaction.Commit();
				}
				catch (Exception e)
				{
					transaction.Rollback();
					throw e;
				}
			}
			return pessoa;
		}

		public List<PessoaModel> GetAll()
         => _context.Pessoa
                .Select(pessoa => new PessoaModel
                {
                    Idpessoa = pessoa.Idpessoa,
                    Nome = pessoa.Nome,
                    Cpf = pessoa.Cpf,
                    Email = pessoa.Email,
                    FoneFixo = pessoa.FoneFixo,
                    FoneCelular = pessoa.FoneCelular,
                    DataNascimento = pessoa.DataNascimento,
                    Sexo = pessoa.Sexo,
                    Cep = pessoa.Cep,
                    Rua = pessoa.Rua,
                    Estado = pessoa.Estado,
                    Cidade = pessoa.Cidade,
                    Complemento = pessoa.Complemento,
                    Bairro = pessoa.Bairro,
                    Numero = pessoa.Numero,
                    Latitude = pessoa.Latitude,
                    Longitude = pessoa.Longitude,
                    Imunodeprimido = Convert.ToBoolean(pessoa.Imunodeprimido),
                    Cancer = Convert.ToBoolean(pessoa.Cancer),
                    Hipertenso = Convert.ToBoolean(pessoa.Hipertenso),
                    Obeso = Convert.ToBoolean(pessoa.Obeso),
                    Diabetes = Convert.ToBoolean(pessoa.Diabetes),
                    DoencaRespiratoria = Convert.ToBoolean(pessoa.DoencaRespiratoria),
                    Cardiopatia = Convert.ToBoolean(pessoa.Cardiopatia),
					DoencaRenal = Convert.ToBoolean(pessoa.DoencaRenal),
					Epilepsia = Convert.ToBoolean(pessoa.Epilepsia),
					OutrasComorbidades = pessoa.OutrasComorbidades,
					SituacaoSaude = pessoa.SituacaoSaude,
					Coriza = Convert.ToBoolean(pessoa.Coriza),
					Diarreia = Convert.ToBoolean(pessoa.Diarreia),
					DificuldadeRespiratoria = Convert.ToBoolean(pessoa.DificuldadeRespiratoria),
					DorAbdominal = Convert.ToBoolean(pessoa.DorAbdominal),
					DorGarganta = Convert.ToBoolean(pessoa.DorGarganta),
					DorOuvido = Convert.ToBoolean(pessoa.DorOuvido),
					Febre = Convert.ToBoolean(pessoa.Febre),
					Nausea = Convert.ToBoolean(pessoa.Nausea),
					PerdaOlfatoPaladar = Convert.ToBoolean(pessoa.PerdaOlfatoPaladar),
					Tosse = Convert.ToBoolean(pessoa.Tosse),
					IdAreaAtuacao = pessoa.IdAreaAtuacao,
					OutrosSintomas = pessoa.OutrosSintomas,
					Profissao = pessoa.Profissao,
					DataObito = pessoa.DataObito,
					Cns = pessoa.Cns
				}).ToList();

        public PessoaModel GetById(int id)
        => _context.Pessoa
                .Where(pessoaModel => pessoaModel.Idpessoa == id)
                .Select(pessoa => new PessoaModel
                {
					Idpessoa = pessoa.Idpessoa,
					Nome = pessoa.Nome,
					Cpf = pessoa.Cpf,
					Email = pessoa.Email,
					FoneFixo = pessoa.FoneFixo,
					FoneCelular = pessoa.FoneCelular,
					DataNascimento = pessoa.DataNascimento,
					Sexo = pessoa.Sexo,
					Cep = pessoa.Cep,
					Rua = pessoa.Rua,
					Estado = pessoa.Estado,
					Cidade = pessoa.Cidade,
					Complemento = pessoa.Complemento,
					Bairro = pessoa.Bairro,
					Numero = pessoa.Numero,
					Latitude = pessoa.Latitude,
					Longitude = pessoa.Longitude,
					Imunodeprimido = Convert.ToBoolean(pessoa.Imunodeprimido),
					Cancer = Convert.ToBoolean(pessoa.Cancer),
					Hipertenso = Convert.ToBoolean(pessoa.Hipertenso),
					Obeso = Convert.ToBoolean(pessoa.Obeso),
					Diabetes = Convert.ToBoolean(pessoa.Diabetes),
					DoencaRespiratoria = Convert.ToBoolean(pessoa.DoencaRespiratoria),
					Cardiopatia = Convert.ToBoolean(pessoa.Cardiopatia),
					DoencaRenal = Convert.ToBoolean(pessoa.DoencaRenal),
					Epilepsia = Convert.ToBoolean(pessoa.Epilepsia),
					OutrasComorbidades = pessoa.OutrasComorbidades,
					SituacaoSaude = pessoa.SituacaoSaude,
					Coriza = Convert.ToBoolean(pessoa.Coriza),
					Diarreia = Convert.ToBoolean(pessoa.Diarreia),
					DificuldadeRespiratoria = Convert.ToBoolean(pessoa.DificuldadeRespiratoria),
					DorAbdominal = Convert.ToBoolean(pessoa.DorAbdominal),
					DorGarganta = Convert.ToBoolean(pessoa.DorGarganta),
					DorOuvido = Convert.ToBoolean(pessoa.DorOuvido),
					Febre = Convert.ToBoolean(pessoa.Febre),
					Nausea = Convert.ToBoolean(pessoa.Nausea),
					PerdaOlfatoPaladar = Convert.ToBoolean(pessoa.PerdaOlfatoPaladar),
					Tosse = Convert.ToBoolean(pessoa.Tosse),
					IdAreaAtuacao = pessoa.IdAreaAtuacao,
					OutrosSintomas = pessoa.OutrosSintomas,
					Profissao = pessoa.Profissao,
					DataObito = pessoa.DataObito,
					Cns = pessoa.Cns
				}).FirstOrDefault();

        public PessoaModel GetByCpf(string cpf)
         => _context.Pessoa
                .Where(pessoaModel => pessoaModel.Cpf.Equals(cpf))
                .Select(pessoa => new PessoaModel
                {
					Idpessoa = pessoa.Idpessoa,
					Nome = pessoa.Nome,
					Cpf = pessoa.Cpf,
					Email = pessoa.Email,
					FoneFixo = pessoa.FoneFixo,
					FoneCelular = pessoa.FoneCelular,
					DataNascimento = pessoa.DataNascimento,
					Sexo = pessoa.Sexo,
					Cep = pessoa.Cep,
					Rua = pessoa.Rua,
					Estado = pessoa.Estado,
					Cidade = pessoa.Cidade,
					Complemento = pessoa.Complemento,
					Bairro = pessoa.Bairro,
					Numero = pessoa.Numero,
					Latitude = pessoa.Latitude,
					Longitude = pessoa.Longitude,
					Imunodeprimido = Convert.ToBoolean(pessoa.Imunodeprimido),
					Cancer = Convert.ToBoolean(pessoa.Cancer),
					Hipertenso = Convert.ToBoolean(pessoa.Hipertenso),
					Obeso = Convert.ToBoolean(pessoa.Obeso),
					Diabetes = Convert.ToBoolean(pessoa.Diabetes),
					DoencaRespiratoria = Convert.ToBoolean(pessoa.DoencaRespiratoria),
					Cardiopatia = Convert.ToBoolean(pessoa.Cardiopatia),
					DoencaRenal = Convert.ToBoolean(pessoa.DoencaRenal),
					Epilepsia = Convert.ToBoolean(pessoa.Epilepsia),
					OutrasComorbidades = pessoa.OutrasComorbidades,
					SituacaoSaude = pessoa.SituacaoSaude,
					Coriza = Convert.ToBoolean(pessoa.Coriza),
					Diarreia = Convert.ToBoolean(pessoa.Diarreia),
					DificuldadeRespiratoria = Convert.ToBoolean(pessoa.DificuldadeRespiratoria),
					DorAbdominal = Convert.ToBoolean(pessoa.DorAbdominal),
					DorGarganta = Convert.ToBoolean(pessoa.DorGarganta),
					DorOuvido = Convert.ToBoolean(pessoa.DorOuvido),
					Febre = Convert.ToBoolean(pessoa.Febre),
					Nausea = Convert.ToBoolean(pessoa.Nausea),
					PerdaOlfatoPaladar = Convert.ToBoolean(pessoa.PerdaOlfatoPaladar),
					Tosse = Convert.ToBoolean(pessoa.Tosse),
					IdAreaAtuacao = pessoa.IdAreaAtuacao,
					OutrosSintomas = pessoa.OutrosSintomas,
					Profissao = pessoa.Profissao,
					DataObito = pessoa.DataObito,
					Cns = pessoa.Cns
				}).FirstOrDefault();

		public PessoaModel GetByCns(string cns)
		 => _context.Pessoa
				.Where(pessoaModel => pessoaModel.Cns.Equals(cns))
				.Select(pessoa => new PessoaModel
				{
					Idpessoa = pessoa.Idpessoa,
					Nome = pessoa.Nome,
					Cpf = pessoa.Cpf,
					Email = pessoa.Email,
					FoneFixo = pessoa.FoneFixo,
					FoneCelular = pessoa.FoneCelular,
					DataNascimento = pessoa.DataNascimento,
					Sexo = pessoa.Sexo,
					Cep = pessoa.Cep,
					Rua = pessoa.Rua,
					Estado = pessoa.Estado,
					Cidade = pessoa.Cidade,
					Complemento = pessoa.Complemento,
					Bairro = pessoa.Bairro,
					Numero = pessoa.Numero,
					Latitude = pessoa.Latitude,
					Longitude = pessoa.Longitude,
					Imunodeprimido = Convert.ToBoolean(pessoa.Imunodeprimido),
					Cancer = Convert.ToBoolean(pessoa.Cancer),
					Hipertenso = Convert.ToBoolean(pessoa.Hipertenso),
					Obeso = Convert.ToBoolean(pessoa.Obeso),
					Diabetes = Convert.ToBoolean(pessoa.Diabetes),
					DoencaRespiratoria = Convert.ToBoolean(pessoa.DoencaRespiratoria),
					Cardiopatia = Convert.ToBoolean(pessoa.Cardiopatia),
					DoencaRenal = Convert.ToBoolean(pessoa.DoencaRenal),
					Epilepsia = Convert.ToBoolean(pessoa.Epilepsia),
					OutrasComorbidades = pessoa.OutrasComorbidades,
					SituacaoSaude = pessoa.SituacaoSaude,
					Coriza = Convert.ToBoolean(pessoa.Coriza),
					Diarreia = Convert.ToBoolean(pessoa.Diarreia),
					DificuldadeRespiratoria = Convert.ToBoolean(pessoa.DificuldadeRespiratoria),
					DorAbdominal = Convert.ToBoolean(pessoa.DorAbdominal),
					DorGarganta = Convert.ToBoolean(pessoa.DorGarganta),
					DorOuvido = Convert.ToBoolean(pessoa.DorOuvido),
					Febre = Convert.ToBoolean(pessoa.Febre),
					Nausea = Convert.ToBoolean(pessoa.Nausea),
					PerdaOlfatoPaladar = Convert.ToBoolean(pessoa.PerdaOlfatoPaladar),
					Tosse = Convert.ToBoolean(pessoa.Tosse),
					IdAreaAtuacao = pessoa.IdAreaAtuacao,
					OutrosSintomas = pessoa.OutrosSintomas,
					Profissao = pessoa.Profissao,
					DataObito = pessoa.DataObito,
					Cns = pessoa.Cns
				}).FirstOrDefault();

		public List<PessoaModel> GetByCidade(string cidade)
          => _context.Pessoa
                .Where(p => p.Cidade.ToUpper().Equals(cidade.ToUpper()))
                .Select(pessoa => new PessoaModel
                {
					Idpessoa = pessoa.Idpessoa,
					Nome = pessoa.Nome,
					Cpf = pessoa.Cpf,
					Email = pessoa.Email,
					FoneFixo = pessoa.FoneFixo,
					FoneCelular = pessoa.FoneCelular,
					DataNascimento = pessoa.DataNascimento,
					Sexo = pessoa.Sexo,
					Cep = pessoa.Cep,
					Rua = pessoa.Rua,
					Estado = pessoa.Estado,
					Cidade = pessoa.Cidade,
					Complemento = pessoa.Complemento,
					Bairro = pessoa.Bairro,
					Numero = pessoa.Numero,
					Latitude = pessoa.Latitude,
					Longitude = pessoa.Longitude,
					Imunodeprimido = Convert.ToBoolean(pessoa.Imunodeprimido),
					Cancer = Convert.ToBoolean(pessoa.Cancer),
					Hipertenso = Convert.ToBoolean(pessoa.Hipertenso),
					Obeso = Convert.ToBoolean(pessoa.Obeso),
					Diabetes = Convert.ToBoolean(pessoa.Diabetes),
					DoencaRespiratoria = Convert.ToBoolean(pessoa.DoencaRespiratoria),
					Cardiopatia = Convert.ToBoolean(pessoa.Cardiopatia),
					DoencaRenal = Convert.ToBoolean(pessoa.DoencaRenal),
					Epilepsia = Convert.ToBoolean(pessoa.Epilepsia),
					OutrasComorbidades = pessoa.OutrasComorbidades,
					SituacaoSaude = pessoa.SituacaoSaude,
					Coriza = Convert.ToBoolean(pessoa.Coriza),
					Diarreia = Convert.ToBoolean(pessoa.Diarreia),
					DificuldadeRespiratoria = Convert.ToBoolean(pessoa.DificuldadeRespiratoria),
					DorAbdominal = Convert.ToBoolean(pessoa.DorAbdominal),
					DorGarganta = Convert.ToBoolean(pessoa.DorGarganta),
					DorOuvido = Convert.ToBoolean(pessoa.DorOuvido),
					Febre = Convert.ToBoolean(pessoa.Febre),
					Nausea = Convert.ToBoolean(pessoa.Nausea),
					PerdaOlfatoPaladar = Convert.ToBoolean(pessoa.PerdaOlfatoPaladar),
					Tosse = Convert.ToBoolean(pessoa.Tosse),
					IdAreaAtuacao = pessoa.IdAreaAtuacao,
					OutrosSintomas = pessoa.OutrosSintomas,
					Profissao = pessoa.Profissao,
					DataObito = pessoa.DataObito,
					Cns = pessoa.Cns
				}).ToList();

		public List<PessoaModel> GetByEstado(string estado)
          => _context.Pessoa
                .Where(p => p.Estado.ToUpper().Equals(estado.ToUpper()))
                .Select(pessoa => new PessoaModel
                {
					Idpessoa = pessoa.Idpessoa,
					Nome = pessoa.Nome,
					Cpf = pessoa.Cpf,
					Email = pessoa.Email,
					FoneFixo = pessoa.FoneFixo,
					FoneCelular = pessoa.FoneCelular,
					DataNascimento = pessoa.DataNascimento,
					Sexo = pessoa.Sexo,
					Cep = pessoa.Cep,
					Rua = pessoa.Rua,
					Estado = pessoa.Estado,
					Cidade = pessoa.Cidade,
					Complemento = pessoa.Complemento,
					Bairro = pessoa.Bairro,
					Numero = pessoa.Numero,
					Latitude = pessoa.Latitude,
					Longitude = pessoa.Longitude,
					Imunodeprimido = Convert.ToBoolean(pessoa.Imunodeprimido),
					Cancer = Convert.ToBoolean(pessoa.Cancer),
					Hipertenso = Convert.ToBoolean(pessoa.Hipertenso),
					Obeso = Convert.ToBoolean(pessoa.Obeso),
					Diabetes = Convert.ToBoolean(pessoa.Diabetes),
					DoencaRespiratoria = Convert.ToBoolean(pessoa.DoencaRespiratoria),
					Cardiopatia = Convert.ToBoolean(pessoa.Cardiopatia),
					DoencaRenal = Convert.ToBoolean(pessoa.DoencaRenal),
					Epilepsia = Convert.ToBoolean(pessoa.Epilepsia),
					OutrasComorbidades = pessoa.OutrasComorbidades,
					SituacaoSaude = pessoa.SituacaoSaude,
					Coriza = Convert.ToBoolean(pessoa.Coriza),
					Diarreia = Convert.ToBoolean(pessoa.Diarreia),
					DificuldadeRespiratoria = Convert.ToBoolean(pessoa.DificuldadeRespiratoria),
					DorAbdominal = Convert.ToBoolean(pessoa.DorAbdominal),
					DorGarganta = Convert.ToBoolean(pessoa.DorGarganta),
					DorOuvido = Convert.ToBoolean(pessoa.DorOuvido),
					Febre = Convert.ToBoolean(pessoa.Febre),
					Nausea = Convert.ToBoolean(pessoa.Nausea),
					PerdaOlfatoPaladar = Convert.ToBoolean(pessoa.PerdaOlfatoPaladar),
					Tosse = Convert.ToBoolean(pessoa.Tosse),
					IdAreaAtuacao = pessoa.IdAreaAtuacao,
					OutrosSintomas = pessoa.OutrosSintomas,
					Profissao = pessoa.Profissao,
					DataObito = pessoa.DataObito,
					Cns = pessoa.Cns
				}).ToList();


		private Pessoa ModelToEntitySemSintomas(PessoaModel pessoaModel, Pessoa entity)
		{
			pessoaModel.Cpf = Methods.RemoveSpecialsCaracts(pessoaModel.Cpf);
			pessoaModel.Cep = Methods.RemoveSpecialsCaracts(pessoaModel.Cep);
			pessoaModel.FoneFixo = pessoaModel.FoneFixo != null ? Methods.RemoveSpecialsCaracts(pessoaModel.FoneFixo) : "";
			pessoaModel.FoneCelular = Methods.RemoveSpecialsCaracts(pessoaModel.FoneCelular);

			entity.Idpessoa = pessoaModel.Idpessoa;
			entity.Nome = pessoaModel.Nome.ToUpper();
			entity.Cpf = pessoaModel.Cpf;
			entity.Cns = pessoaModel.Cns;
			entity.Sexo = pessoaModel.Sexo;
			entity.IdAreaAtuacao = pessoaModel.IdAreaAtuacao;
			entity.Profissao = pessoaModel.Profissao;
			entity.Cep = pessoaModel.Cep;
			entity.Rua = pessoaModel.Rua.ToUpper();
			entity.Bairro = pessoaModel.Bairro.ToUpper();
			entity.Cidade = pessoaModel.Cidade.ToUpper();
			entity.Estado = pessoaModel.Estado.ToUpper();
			entity.Numero = pessoaModel.Numero;
			entity.Complemento = pessoaModel.Complemento != null? pessoaModel.Complemento.ToUpper() : pessoaModel.Complemento;
			entity.Latitude = pessoaModel.Latitude;
			entity.Longitude = pessoaModel.Longitude;
			entity.FoneCelular = pessoaModel.FoneCelular;
			entity.FoneFixo = pessoaModel.FoneFixo;
			entity.Email = pessoaModel.Email;
			entity.DataNascimento = pessoaModel.DataNascimento;
			entity.Hipertenso = Convert.ToByte(pessoaModel.Hipertenso);
			entity.Diabetes = Convert.ToByte(pessoaModel.Diabetes);
			entity.Obeso = Convert.ToByte(pessoaModel.Obeso);
			entity.Cardiopatia = Convert.ToByte(pessoaModel.Cardiopatia);
			entity.Imunodeprimido = Convert.ToByte(pessoaModel.Imunodeprimido);
			entity.Cancer = Convert.ToByte(pessoaModel.Cancer);
			entity.DoencaRespiratoria = Convert.ToByte(pessoaModel.DoencaRespiratoria);
			entity.DoencaRenal = Convert.ToByte(pessoaModel.DoencaRenal);
			entity.Epilepsia = Convert.ToByte(pessoaModel.Epilepsia);
			entity.OutrasComorbidades = pessoaModel.OutrasComorbidades;
			entity.SituacaoSaude = pessoaModel.SituacaoSaude;
			entity.DataObito = pessoaModel.DataObito;
			return entity;
		}
		private Pessoa ModelToEntity(PessoaModel pessoaModel, Pessoa entity)
        {
			entity = ModelToEntitySemSintomas(pessoaModel, entity);
			entity.Coriza = Convert.ToByte(pessoaModel.Coriza);
			entity.Nausea = Convert.ToByte(pessoaModel.Nausea);
			entity.Tosse = Convert.ToByte(pessoaModel.Tosse);
			entity.PerdaOlfatoPaladar = Convert.ToByte(pessoaModel.PerdaOlfatoPaladar);
			entity.Diarreia = Convert.ToByte(pessoaModel.Diarreia);
			entity.DificuldadeRespiratoria = Convert.ToByte(pessoaModel.DificuldadeRespiratoria);
			entity.DorAbdominal = Convert.ToByte(pessoaModel.DorAbdominal);
			entity.DorGarganta = Convert.ToByte(pessoaModel.DorGarganta);
			entity.DorOuvido = Convert.ToByte(pessoaModel.DorOuvido);
			entity.Febre = Convert.ToByte(pessoaModel.Febre);
			entity.OutrosSintomas = pessoaModel.OutrosSintomas;
			return entity;
        }
    }
}
