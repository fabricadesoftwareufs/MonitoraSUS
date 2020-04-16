using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.ViewModel;
using MonitoraSUS.Utils;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MonitoraSUS.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IPessoaService _pessoaService;
        public LoginController(IUsuarioService usuarioService, IPessoaService pessoaService)
        {
            _usuarioService = usuarioService;
            _pessoaService = pessoaService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Login/RetornaSenha/{senha}")]
        public string RetornaSenha(string senha) => Criptografia.GerarHashSenha(senha);

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var cpf = Methods.RemoveSpecialsCaracts(login.Cpf);
                var senha = Criptografia.GerarHashSenha(login.Senha);
                var user = _usuarioService.GetByLogin(cpf, senha);

                if (user != null)
                {
                    // informaçoes pessoais do usuario | adicionar as claims o dado que mais precisar
                    var person = _pessoaService.GetById(user.IdPessoa);
                    var role = ReturnRole(user.TipoUsuario);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.SerialNumber, user.IdUsuario.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, person.Nome),
                        new Claim(ClaimTypes.StateOrProvince, person.Estado),
                        new Claim(ClaimTypes.Locality, person.Cidade),
                        new Claim(ClaimTypes.UserData, user.Cpf),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, role)
                    };
                    // Adicionando uma identidade as claims.
                    var identidade = new ClaimsIdentity(claims, "login");

                    // Propriedades da autenticação.
                    var propriedadesClaim = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1) // Expira em 1 dia
                    };

                    // Logando efetivamente.
                    await HttpContext.SignInAsync(new ClaimsPrincipal(identidade), propriedadesClaim);

                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Index", "Login", new { msg = "error" });
        }

        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(UsuarioModel usuario)
        {
            if (ModelState.IsValid)
            {
                // Informações do objeto
                usuario.Cpf = Methods.RemoveSpecialsCaracts(usuario.Cpf);
                usuario.Senha = Criptografia.GerarHashSenha(usuario.Senha);

                if (_usuarioService.Insert(usuario))
                    return RedirectToAction("SignIn", "Login");
            }
            return View(usuario);
        }

        [Authorize]
        public ActionResult AcessDenied()
        {
            return View();
        }

        private string ReturnRole(int userType)
        {
            switch (userType)
            {
                case 0: return "USUARIO";
                case 1: return "AGENTE";
                case 2: return "COORDENADOR";
                case 3: return "SECRETARIO";
                case 4: return "ADM";
                default: return "UNDEFINED";
            }
        }
    }
}
