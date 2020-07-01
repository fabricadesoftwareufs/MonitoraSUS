using Model;
using Model.ViewModel;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Service
{
    public class UsuarioService : IUsuarioService
    {
        private readonly monitorasusContext _context;

        public UsuarioService(monitorasusContext context)
        {
            _context = context;
        }

        public bool Delete(int id)
        {
            var model = _context.Usuario.Where(m => m.IdUsuario == id).FirstOrDefault();
            if (model != null)
            {
                _context.Remove(model);
                return _context.SaveChanges() == 1 ? true : false;
            }
            return false;
        }

        public List<UsuarioModel> GetAll()
            => _context
                .Usuario
                .Select(model => new UsuarioModel
                {
                    IdUsuario = model.IdUsuario,
                    Cpf = model.Cpf,
                    Senha = model.Senha,
                    TipoUsuario = Convert.ToByte(model.TipoUsuario),
                    Email = model.Email,
                    IdPessoa = model.IdPessoa
                }).ToList();

        public UsuarioModel GetByCpf(string cpf)
            => _context
                .Usuario
                .Where(r => r.Cpf.Equals(cpf))
                .Select(model => new UsuarioModel
                {
                    IdUsuario = model.IdUsuario,
                    Cpf = model.Cpf,
                    Senha = model.Senha,
                    TipoUsuario = Convert.ToByte(model.TipoUsuario),
                    Email = model.Email
                }).FirstOrDefault();

        public UsuarioModel GetById(int id)
            => _context
                .Usuario
                .Where(r => r.IdUsuario == id)
                .Select(model => new UsuarioModel
                {
                    IdUsuario = model.IdUsuario,
                    Cpf = model.Cpf,
                    Senha = model.Senha,
                    TipoUsuario = Convert.ToByte(model.TipoUsuario),
                    Email = model.Email,
                    IdPessoa = model.IdPessoa
                }).FirstOrDefault();

        public UsuarioModel GetByIdPessoa(int idPessoa)
           => _context
               .Usuario
               .Where(r => r.IdPessoa == idPessoa)
               .Select(model => new UsuarioModel
               {
                   IdUsuario = model.IdUsuario,
                   Cpf = model.Cpf,
                   Senha = model.Senha,
                   TipoUsuario = Convert.ToByte(model.TipoUsuario),
                   Email = model.Email,
                   IdPessoa = model.IdPessoa
               }).FirstOrDefault();

        public UsuarioModel GetByLogin(string cpf, string senha)
            => _context
                .Usuario
                .Where(model => model.Cpf.Equals(cpf) && model.Senha.Equals(senha))
                .Select(model => new UsuarioModel
                {
                    IdUsuario = model.IdUsuario,
                    Cpf = model.Cpf,
                    Senha = model.Senha,
                    TipoUsuario = Convert.ToByte(model.TipoUsuario),
                    Email = model.Email,
                    IdPessoa = model.IdPessoa
                }).FirstOrDefault();


        public bool Insert(UsuarioModel usuarioModel)
        {
            if (usuarioModel != null)
            {
                _context.Add(ModelToEntity(usuarioModel, new Usuario()));
                return _context.SaveChanges() == 1 ? true : false;
            }
            return false;
        }

        public bool Update(UsuarioModel usuarioModel)
        {
            if (usuarioModel != null)
            {
                var oldUser = _context.Usuario.Where(model => model.IdUsuario == usuarioModel.IdUsuario).FirstOrDefault();
                if (oldUser != null)
                {
                    _context.Update(ModelToEntity(usuarioModel, oldUser));
                    return _context.SaveChanges() == 1 ? true : false;
                }
                return false;
            }
            return false;
        }

		/// <summary>
		/// Recebe o Usuario da sessão em questão e retorna os dados do mesmo em um objeto usuario.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public UsuarioViewModel RetornLoggedUser(ClaimsIdentity claimsIdentity)
		{
			var usuario = new UsuarioModel
			{
				IdUsuario = int.Parse(claimsIdentity.Claims.Where(s => s.Type == ClaimTypes.SerialNumber).Select(s => s.Value).FirstOrDefault()),
				Cpf = claimsIdentity.Claims.Where(s => s.Type == ClaimTypes.UserData).Select(s => s.Value).FirstOrDefault(),
				Email = claimsIdentity.Claims.Where(s => s.Type == ClaimTypes.Email).Select(s => s.Value).FirstOrDefault(),
				IdPessoa = Convert.ToInt32(claimsIdentity.Claims.Where(s => s.Type == ClaimTypes.NameIdentifier).Select(s => s.Value).FirstOrDefault()),

			};

			var usuarioViewModel = new UsuarioViewModel
			{
				UsuarioModel = usuario,
				RoleUsuario = claimsIdentity.Claims.Where(s => s.Type == ClaimTypes.Role).Select(s => s.Value).FirstOrDefault()
			};

			return usuarioViewModel;
		}


		private Usuario ModelToEntity(UsuarioModel model, Usuario entity)
        {
            entity.IdUsuario = model.IdUsuario;
            entity.Cpf = model.Cpf;
            entity.Senha = model.Senha;
            entity.TipoUsuario = Convert.ToByte(model.TipoUsuario);
            entity.Email = model.Email;
            entity.IdPessoa = model.IdPessoa;
            return entity;
        }

		public string MessageEmail(RecuperarSenhaModel senhaModel, int finalidadeEmail)
		{
			var uri = new Uri("https://www.monitorasus.ufs.br/");
			var site = "<a href='" + uri.Scheme + "://" + uri.Host + ":" + uri.Port;
			var link = site + "/Login/RecuperarSenha/";

			switch (finalidadeEmail)
			{
				case 0:
					return "<html><body>" +
						"Foi solicitado a recuperação de sua senha para acesso ao MonitoraSUS.<br>" +
						"Você possui 24 horas para fazer a alteração da sua senha de acesso.<br>" +
						link + senhaModel.Token + "'>Clique aqui mudar a senha</a>" +
						RodapeEmail();


				case 1:
					return "<html><body>" +
						"Parabéns! Seu cadastro foi aprovado para acesso ao MonitoraSUS.<br>" +
						"Você possui 24 horas para criar sua senha de acesso ao sistema.<br>" +
						link + senhaModel.Token + "'>Clique aqui para criar sua senha.</a>" +
						RodapeEmail();

				case 2:
					return "<html><body>" +
						"Parabéns! Seu cadastro foi ativado para acesso ao MonitoraSUS. <br>" +
						"Acesse o sistema " + site + "'>aqui</a>." +
						"<br>Caso não lembre da sua senha de acesso, você possui 24 horas para criar uma nova senha.<br>" +
						link + senhaModel.Token + "'>Clique aqui para criar uma nova senha.</a>" +
						RodapeEmail();
				case 4:
					return "<html><body>" +
						"Obrigado por solicitar o cadastro no MonitoraSUS! <br/>" +
						"O sistema permite gerenciar os testes virais realizados pela gestão e fazer o monitoramento dos pacientes <br/>" +
						"residentes no município que foram positivados e notificados pelo MonitoraSUS. <br/>" +
						"Seu cadastro foi aprovado com perfil de ADMINISTRADOR do Município ou Estado solicitado.<br/>" +
						"Acesse o sistema através da url www.monitorasus.ufs.br e consulte o manual do sistema." +
						"Entraremos em contato para agendarmos uma apresentação das funcionalidades." +
						"Você possui 24 horas para criar sua senha de acesso ao sistema.<br>" +
						link + senhaModel.Token + "'>Clique aqui para criar sua senha.</a>" +
						RodapeEmail();

				default: return null;
			}
		}

		private string RodapeEmail()
		{
			return "<br>" +
					"Qualquer dúvida ou sugestão entre em contato com o nosso time." +
						"<br><br>" +
						"<br>Equipe MonitoraSUS" +
						"<br>KNUTH-Fábrica de Software da Universidade Federal de Sergipe" +
						"<br>fabricadesoftware@ufs.br";
		}
	}
}
