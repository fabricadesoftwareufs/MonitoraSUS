using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model;
using Model.ViewModel;
using MonitoraSUS.Resources.Methods;
using QueroTransporteWeb.Resources.Methods;
using Service;

namespace MonitoraSUS.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly UsuarioService _usuarioService;
        public LoginController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;

        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var user = _usuarioService.GetByLogin(MethodsUtils.RemoverCaracteresEspeciais(login.Cpf), login.Senha); ///Criptografia.GerarHashSenha(login.Senha));

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.SerialNumber, user.IdUsuario.ToString()),
                        new Claim(ClaimTypes.UserData, user.Cpf),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.TipoUsuario.ToString())
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
                usuario.Cpf = MethodsUtils.RemoverCaracteresEspeciais(usuario.Cpf);
                usuario.Senha = Criptografia.GerarHashSenha(usuario.Senha);
                usuario.TipoUsuario = 0;       // usuario comum

                if (_usuarioService.Insert(usuario))
                    return RedirectToAction("LoginAsync", "Login");
            }
            return View(usuario);
        }

        [Authorize]
        public ActionResult AcessDenied()
        {
            return View();
        }
    }
}
