using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace MonitoraSUS.Controllers
{
    public class AgenteSecretarioController : Controller
    {
        private readonly IMunicipioService _municipioService;
        private readonly IEstadoService _estadoService;
        public AgenteSecretarioController(IMunicipioService municipioService, IEstadoService estadoService)
        {
            _municipioService = municipioService;
            _estadoService = estadoService;
        }
        // GET: AgenteSecretario
        public ActionResult Index()
        {
            return View();
        }

        // GET: AgenteSecretario/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AgenteSecretario/Create
        public ActionResult Create(int userType)
        {
            ViewBag.userType = userType;
            return View();
        }

        // POST: AgenteSecretario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAgent(IFormCollection collection)
        {
            try
            {
                var cpf = collection["Cpf"];
                var nome = collection["Nome"];
                var dataNascimento = collection["DataNascimento"];
                var sexo = collection["sexo"];
                var cep = collection["Cep"];
                var rua = collection["Rua"];
                var numero = collection["Numero"];
                var bairro = collection["Bairro"];
                var cidade = collection["Cidade"];
                var estado = collection["Estado"];
                var complemento = collection["Complemento"];
                var cell = collection["FoneCelular"];
                var fixo = collection["FoneFixo"];
                var email = collection["Email"];

                // Selects 
                var estadoSelected = collection["select-Estado"];
                var cidadeSelected = collection["select-Cidade"];

                // Doenças
                var hipertenso = collection["Hipertenso"];
                var diabetes = collection["Diabetes"];
                var obeso = collection["Obeso"];
                var cardiopata = collection["Cardiopatia"];
                var imunoDepri = collection["Imunodeprimido"];
                var cancer = collection["Cancer"];
                var doencaResp = collection["DoencaRespiratoria"];

                // Local atuação
                var atuacao = collection["areaAtuacao"];

                return RedirectToAction(nameof(Create));
            }
            catch
            {
                return View();
            }
        }

        // POST: AgenteSecretario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSec(IFormCollection collection)
        {
            try
            {
                var cpf = collection["Cpf"];
                var nome = collection["Nome"];
                var dataNascimento = collection["DataNascimento"];
                var sexo = collection["sexo"];
                var cep = collection["Cep"];
                var rua = collection["Rua"];
                var numero = collection["Numero"];
                var bairro = collection["Bairro"];
                var cidade = collection["Cidade"];
                var estado = collection["Estado"];
                var complemento = collection["Complemento"];
                var cell = collection["FoneCelular"];
                var fixo = collection["FoneFixo"];
                var email = collection["Email"];
                // Selects 
                var estadoSelected = collection["select-Estado"];
                var cidadeSelected = collection["select-Cidade"];
                // Doenças
                var hipertenso = collection["Hipertenso"];
                var diabetes = collection["Diabetes"];
                var obeso = collection["Obeso"];
                var cardiopata = collection["Cardiopatia"];
                var imunoDepri = collection["Imunodeprimido"];
                var cancer = collection["Cancer"];
                var doencaResp = collection["DoencaRespiratoria"];

                var atuacao = collection["areaAtuacao"];

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult ReturnCities(string UF)
        {
            var listOfCities = _municipioService.GetByUFCode(UF);
            if (listOfCities.Count != 0)
                return Ok(listOfCities);
            else
                return NoContent();
        }

        public ActionResult ReturnStates() => Ok(_estadoService.GetAll());

        // GET: AgenteSecretario/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AgenteSecretario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AgenteSecretario/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AgenteSecretario/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}