using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly IEmailService _emailService;
        private readonly IRecuperarSenhaService _recuperarSenhaService;
        public LoginController(IUsuarioService usuarioService, IPessoaService pessoaService, IEmailService emailService, IRecuperarSenhaService recuperarSenhaService)
        {
            _usuarioService = usuarioService;
            _pessoaService = pessoaService;
            _emailService = emailService;
            _recuperarSenhaService = recuperarSenhaService;
        }
        public IActionResult Index()
        {
            return View();
        }

        /*
        [HttpGet("Login/RetornaSenha/{senha}")]
        public string RetornaSenha(string senha) => Criptography.GenerateHashPasswd(senha);
        */

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {

                var cpf = Methods.ValidarCpf(login.Cpf) ? Methods.RemoveSpecialsCaracts(login.Cpf) : throw new Exception("CPF Invalido!!");
                var senha = Criptography.GenerateHashString(login.Senha);

                var user = _usuarioService.GetByLogin(cpf, senha);

                if (user != null)
                {
                    // informaçoes pessoais do usuario | adicionar as claims o dado que mais precisar
                    var person = _pessoaService.GetById(user.IdPessoa);
                    var role = Methods.ReturnRole(user.TipoUsuario);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.SerialNumber, user.IdUsuario.ToString()),
                        new Claim(ClaimTypes.Name, person.Nome),
                        new Claim(ClaimTypes.StateOrProvince, person.Estado),
                        new Claim(ClaimTypes.Locality, person.Cidade),
                        new Claim(ClaimTypes.UserData, user.Cpf),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.IdPessoa.ToString()),
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
                usuario.Cpf = Methods.ValidarCpf(usuario.Cpf) ? Methods.RemoveSpecialsCaracts(usuario.Cpf) : throw new Exception("CPF Invalido!!");
                usuario.Senha = Criptography.GenerateHashString(usuario.Senha);

                if (_usuarioService.Insert(usuario))
                    return RedirectToAction("SignIn", "Login");
            }
            return View(usuario);
        }

        public bool ValidaCpf(string cpf) => Methods.ValidarCpf(cpf);

        [Authorize]
        public ActionResult AcessDenied()
        {
            return View();
        }

        public async Task<ActionResult> EmitirToken(string cpf)
        {
            (bool invalidCpf, bool invalidUser, bool failInsertOrUserHasToken) = await GenerateToken(cpf);

            if (!invalidCpf && !invalidUser && !failInsertOrUserHasToken)
                return RedirectToActionPermanent("Index", "Login", new { msg = "invalidUser" });

            if (!invalidUser && !failInsertOrUserHasToken)
                return RedirectToActionPermanent("Index", "Login", new { msg = "hasToken" });

            if (!failInsertOrUserHasToken)
                return RedirectToActionPermanent("Index", "Login", new { msg = "insertFail" });

            // Se der tudo bem.
            return RedirectToActionPermanent("Index", "Login", new { msg = "successSend" });
        }

        public async Task<(bool, bool, bool)> GenerateToken(string cpf)
        {
            if (Methods.ValidarCpf(cpf))
            {
                var user = _usuarioService.GetByCpf(Methods.RemoveSpecialsCaracts(cpf));
                if (user != null)
                {
                    if (_recuperarSenhaService.UserNotHasToken(user.IdUsuario))
                    {
                        // Objeto será criado e inserido apenas se o usuario não possuir Tokens validos cadastrados.
                        var recSenha = new RecuperarSenhaModel
                        {
                            Token = Methods.GenerateToken(),
                            InicioToken = DateTime.Now,
                            FimToken = DateTime.Now.AddDays(1),
                            EhValido = Convert.ToByte(true),
                            IdUsuario = user.IdUsuario
                        };

                        if (_recuperarSenhaService.Insert(recSenha))
                        {
                            try
                            {
                                // Email só será disparado caso a inserção seja feita com sucesso.
                                await _emailService.SendEmailAsync(user.Email, "MonitoraSUS - Recuperacao de senha", Methods.MessageEmail(recSenha));
                                return (true, true, true);
                            }
                            catch (Exception e)
                            {
                                throw e.InnerException;
                            }
                        }
                        return (true, true, false); // Falha na inserção do recuperarSenha
                    }
                    // User válido porém tem token.
                    return (true, false, false);
                }
            }
            return (false, false, false);
        }

        [HttpGet("Login/RecuperarSenha/{token}")]
        public ActionResult RecuperarSenha(string token)
        {
            if (_recuperarSenhaService.IsTokenValid(token))
                return View(_recuperarSenhaService.GetByToken(token));

            return RedirectToActionPermanent("Index", "Login", new { msg = "invalidToken" });
        }

        public ActionResult ChangePass(IFormCollection collection)
        {
            var user = _usuarioService.GetById(Convert.ToInt32(collection["IdUsuario"]));
            if (user != null)
            {
                user.Senha = Criptography.GenerateHashString(collection["senha"]);
                if (_usuarioService.Update(user))
                {
                    _recuperarSenhaService.SetTokenInvalid(user.IdUsuario);
                    return RedirectToActionPermanent("Index", "Login", new { msg = "sucessChange" });
                }
            }

            return RedirectToActionPermanent("Index", "Login", new { msg = "errorChange" });
        }
    }
}

