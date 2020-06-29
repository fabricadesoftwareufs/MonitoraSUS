using Microsoft.EntityFrameworkCore;
using Model;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

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
					//_context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    var pessoaInserida = new Pessoa();
					if (atualizaSintomas)
						_context.Update(ModelToEntity(pessoaModel, pessoaInserida));
					else
						_context.Update(ModelToEntitySemSintomas(pessoaModel, pessoaInserida));

                    if (string.IsNullOrWhiteSpace(pessoaModel.Cpf))
                    {
                        pessoaModel.Cpf = "T" + DateTime.Now.Ticks.ToString().Substring(12);
                        _context.SaveChanges();
						//_context.Entry(pessoaModel).State = EntityState.Detached;
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
					//_context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
					var pessoaInserida = new Pessoa();
                    _context.Pessoa.Add(ModelToEntity(pessoaModel, pessoaInserida));

                    if (string.IsNullOrWhiteSpace(pessoaModel.Cpf))
                    {
                        pessoaModel.Cpf = "T" + DateTime.Now.Ticks.ToString().Substring(12);
                        _context.SaveChanges();
						//_context.Entry(pessoaModel).State = EntityState.Detached;
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

		public PessoaModel GetByCns(string cns)
	   => _context.Pessoa.AsNoTracking()
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
         => _context.Pessoa.AsNoTracking()
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


		private Pessoa ModelToEntitySemSintomas(PessoaModel model, Pessoa entity)
		{
			entity.Idpessoa = model.Idpessoa;
			entity.Nome = model.Nome;
			entity.Cpf = model.Cpf;
			entity.Sexo = model.Sexo == "Masculino" ? "M" : "F";
			entity.IdAreaAtuacao = model.IdAreaAtuacao;
			entity.Profissao = model.Profissao;
			entity.Cep = model.Cep;
			entity.Rua = model.Rua;
			entity.Bairro = model.Bairro;
			entity.Cidade = model.Cidade;
			entity.Estado = model.Estado;
			entity.Numero = model.Numero;
			entity.Complemento = model.Complemento;
			entity.Latitude = model.Latitude;
			entity.Longitude = model.Longitude;
			entity.FoneCelular = model.FoneCelular;
			entity.FoneFixo = model.FoneFixo;
			entity.Email = model.Email;
			entity.DataNascimento = model.DataNascimento;
			entity.Hipertenso = Convert.ToByte(model.Hipertenso);
			entity.Diabetes = Convert.ToByte(model.Diabetes);
			entity.Obeso = Convert.ToByte(model.Obeso);
			entity.Cardiopatia = Convert.ToByte(model.Cardiopatia);
			entity.Imunodeprimido = Convert.ToByte(model.Imunodeprimido);
			entity.Cancer = Convert.ToByte(model.Cancer);
			entity.DoencaRespiratoria = Convert.ToByte(model.DoencaRespiratoria);
			entity.DoencaRenal = Convert.ToByte(model.DoencaRenal);
			entity.Epilepsia = Convert.ToByte(model.Epilepsia);
			entity.OutrasComorbidades = model.OutrasComorbidades;
			entity.SituacaoSaude = model.SituacaoSaude;
			entity.Cns = model.Cns;
			return entity;
		}
		private Pessoa ModelToEntity(PessoaModel model, Pessoa entity)
        {
            entity.Idpessoa = model.Idpessoa;
            entity.Nome = model.Nome;
            entity.Cpf = model.Cpf;
            entity.Sexo = model.Sexo == "Masculino" ? "M" : "F";
			entity.IdAreaAtuacao = model.IdAreaAtuacao;
			entity.Profissao = model.Profissao;
			entity.Cep = model.Cep;
            entity.Rua = model.Rua;
            entity.Bairro = model.Bairro;
            entity.Cidade = model.Cidade;
            entity.Estado = model.Estado;
            entity.Numero = model.Numero;
            entity.Complemento = model.Complemento;
            entity.Latitude = model.Latitude;
            entity.Longitude = model.Longitude;
            entity.FoneCelular = model.FoneCelular;
            entity.FoneFixo = model.FoneFixo;
            entity.Email = model.Email;
            entity.DataNascimento = model.DataNascimento;
            entity.Hipertenso = Convert.ToByte(model.Hipertenso);
            entity.Diabetes = Convert.ToByte(model.Diabetes);
            entity.Obeso = Convert.ToByte(model.Obeso);
            entity.Cardiopatia = Convert.ToByte(model.Cardiopatia);
            entity.Imunodeprimido = Convert.ToByte(model.Imunodeprimido);
            entity.Cancer = Convert.ToByte(model.Cancer);
            entity.DoencaRespiratoria = Convert.ToByte(model.DoencaRespiratoria);
			entity.DoencaRenal = Convert.ToByte(model.DoencaRenal);
			entity.Epilepsia = Convert.ToByte(model.Epilepsia);
			entity.OutrasComorbidades = model.OutrasComorbidades;
			entity.SituacaoSaude = model.SituacaoSaude;
			entity.Coriza = Convert.ToByte(model.Coriza);
			entity.Nausea = Convert.ToByte(model.Nausea);
			entity.Tosse = Convert.ToByte(model.Tosse);
			entity.PerdaOlfatoPaladar = Convert.ToByte(model.PerdaOlfatoPaladar);
			entity.Diarreia = Convert.ToByte(model.Diarreia);
			entity.DificuldadeRespiratoria = Convert.ToByte(model.DificuldadeRespiratoria);
			entity.DorAbdominal = Convert.ToByte(model.DorAbdominal);
			entity.DorGarganta = Convert.ToByte(model.DorGarganta);
			entity.DorOuvido = Convert.ToByte(model.DorOuvido);
			entity.Febre = Convert.ToByte(model.Febre);
			entity.OutrosSintomas = model.OutrosSintomas;
			entity.DataObito = model.DataObito;
			entity.Cns = model.Cns;
			return entity;
        }
    }
}
