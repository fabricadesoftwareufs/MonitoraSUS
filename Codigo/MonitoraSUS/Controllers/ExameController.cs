using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Model;
using Service;
using Service.Interface;

namespace MonitoraSUS.Controllers
{
    public class ExameController : Controller
    {
        private readonly IVirusBacteriaService _virusBacteriaContext;
        private readonly IExameService _exameContext;
        private readonly IPessoaService _pessoaContext;

        public ExameController(IVirusBacteriaService virusBacteriaContext,
                               IExameService exameContext,
                               IPessoaService pessoaContext)
        {
            _virusBacteriaContext = virusBacteriaContext;
            _exameContext = exameContext;
            _pessoaContext = pessoaContext;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.VirusBacteria = new SelectList(_virusBacteriaContext.GetAll(), "IdVirusBacteria", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ExameViewModel exame)
        {

            ViewBag.VirusBacteria = new SelectList(_virusBacteriaContext.GetAll(), "IdVirusBacteria", "Nome");

            if (exame.PesquisarCpf == 1) //pesquisar usuario por cpf 
            {
                var cpf = exame.IdPaciente.Cpf.Replace(".", "").Replace(".", "").Replace("-", "");
                var pessoa = _pessoaContext.GetByCpf(cpf);

                if (pessoa != null)
                    exame.IdPaciente = pessoa;
                else
                    TempData["resultadoPesquisa"] = "Este paciente  não está cadastrado, preencha os campos para cadastra-lo agora mesmo!";

                return View(exame);
            }
            else
            {
                var pessoa = _pessoaContext.GetById(1); // id do claims 
                exame.IdAgenteSaude = pessoa.Idpessoa;
                
                

                if (ModelState.IsValid)
                {
                    if (_exameContext.Insert(ViewModelToModel(exame, new ExameModel())))
                    {
                        return View();
                    }
                }

                TempData["mensagemErro"] = "Houve um problema ao realizar notificação, tente novamente!";

            }

            TempData["mensagemSucesso"] = "Notificação realizada com SUCESSO!";


            return View(exame);
        }


        public string getStatusExame(ExameViewModel exame) {

            string resultado = "";

            if (exame.Pcr.Equals('S') || exame.IgM.Equals('S'))
            {
                resultado = "P";
            }
            else if (exame.IgG.Equals('S'))
            {
                resultado = "C";
            }
            else if (exame.Pcr.Equals('N') && exame.IgM.Equals('N'))
            {
                resultado = "N";
            }
            else if (exame.Pcr.Equals('I') && exame.IgM.Equals('I'))
            {
                resultado = "I";
            }
            else if ((exame.Pcr.Equals('N') && exame.IgM.Equals('I'))
                || exame.Pcr.Equals('I') && exame.IgM.Equals('N'))
            {
                resultado = "N";
            }

            return resultado;
        }

        public ExameModel ViewModelToModel(ExameViewModel viewModel, ExameModel exame)
        {
            exame.IdAgenteSaude = viewModel.IdAgenteSaude;
            exame.IdPaciente = viewModel.IdPaciente.Idpessoa;
            exame.IdVirusBacteria = viewModel.IdVirusBacteria;
            exame.IgG = viewModel.IgG;
            exame.IgM = viewModel.IgM;
            exame.Pcr = viewModel.Pcr;
            exame.EstadoRealizacao = viewModel.EstadoRealizacao;
            exame.MunicipioId = viewModel.MunicipioId;
            exame.DataInicioSintomas = viewModel.DataInicioSintomas;
            exame.DataExame = viewModel.DataExame;

            return exame;
        }
    }
}
