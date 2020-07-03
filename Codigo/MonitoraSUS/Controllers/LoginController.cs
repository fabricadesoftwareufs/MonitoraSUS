using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.ViewModel;
using Util;
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
        private readonly IPessoaTrabalhaEstadoService _pessoaTrabalhaEstado;
        private readonly IPessoaTrabalhaMunicipioService _pessoaTrabalhaMunicipio;
        private readonly IEstadoService _estadoService;
        private readonly IMunicipioService _municipioService;
        private readonly IEmpresaExameService _empresaExameService;
        private readonly IEmailService _emailService;
        private readonly IRecuperarSenhaService _recuperarSenhaService;
        public LoginController(IUsuarioService usuarioService, IPessoaService pessoaService,
            IPessoaTrabalhaEstadoService pessoaTrabalhaEstado, IPessoaTrabalhaMunicipioService pessoaTrabalhaMunicipio,
            IEstadoService estadoService, IMunicipioService municipioService, IEmpresaExameService empresaExameService,
            IEmailService emailService, IRecuperarSenhaService recuperarSenhaService)
        {
            _usuarioService = usuarioService;
            _pessoaService = pessoaService;
            _pessoaTrabalhaEstado = pessoaTrabalhaEstado;
            _pessoaTrabalhaMunicipio = pessoaTrabalhaMunicipio;
            _estadoService = estadoService;
            _municipioService = municipioService;
            _empresaExameService = empresaExameService;
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
                    var trabalha = "";
                    var empresa = "";

                    if (role != "ADM" || role != "USUARIO")
                    {
                        var trabalhaEstado = _pessoaTrabalhaEstado.GetByIdPessoa(person.Idpessoa);
                        if (trabalhaEstado != null)
                        {
                            trabalha = _estadoService.GetById(trabalhaEstado.IdEstado).Nome;
                            empresa = _empresaExameService.GetById(trabalhaEstado.IdEmpresaExame).Nome;
                        }
                        else
                        {
                            var trabalhaMunicipio = _pessoaTrabalhaMunicipio.GetByIdPessoa(person.Idpessoa);
                            trabalha = _municipioService.GetById(trabalhaMunicipio.IdMunicipio).Nome;
                        }
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.SerialNumber, user.IdUsuario.ToString()),
                        new Claim(ClaimTypes.Name, person.Nome),
                        new Claim(ClaimTypes.StateOrProvince, person.Estado),
                        new Claim(ClaimTypes.Locality, person.Cidade),
                        new Claim(ClaimTypes.UserData, user.Cpf),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.IdPessoa.ToString()),
                        new Claim(ClaimTypes.Role, role),
                        new Claim(ClaimTypes.Dns, trabalha),
                        new Claim(ClaimTypes.Sid, empresa)
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

        public async Task<ActionResult> EmitirToken(string cpf, int finalidade = 0)
        {
            (bool invalidCpf, bool invalidUser, bool failInsertOrUserHasToken) = await GenerateToken(cpf, finalidade);

            if (!invalidCpf && !invalidUser && !failInsertOrUserHasToken)
                return RedirectToActionPermanent("Index", "Login", new { msg = "invalidUser" });

            if (!invalidUser && !failInsertOrUserHasToken)
                return RedirectToActionPermanent("Index", "Login", new { msg = "hasToken" });

            if (!failInsertOrUserHasToken)
                return RedirectToActionPermanent("Index", "Login", new { msg = "insertFail" });

            // Se der tudo bem.
            return RedirectToActionPermanent("Index", "Login", new { msg = "successSend" });
        }

        public async Task<(bool, bool, bool)> GenerateToken(string cpf, int finalidade)
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
                                await _emailService.SendEmailAsync(user.Email, "MonitoraSUS - Acesso ao Sistema", _usuarioService.MessageEmail(recSenha, finalidade));
                                return (true, true, true);
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

