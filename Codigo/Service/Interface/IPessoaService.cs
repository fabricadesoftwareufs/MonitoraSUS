﻿using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IPessoaService
    {
        PessoaModel Insert(PessoaModel pessoaModel);
        PessoaModel Update(PessoaModel pessoaModel, bool atualizaSintomas);
        bool Delete(int id);
        List<PessoaModel> GetAll();
        PessoaModel GetById(int id);
        PessoaModel GetByCpf(string cpf);
        PessoaModel GetByCns(string cns);
        List<PessoaModel> GetByCidade(string cidade);
		List<PessoaModel> GetByEstado(string estado);
	}
}
