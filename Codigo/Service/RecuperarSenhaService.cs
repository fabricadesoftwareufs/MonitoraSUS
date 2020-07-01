using Model;
using Persistence;
using Service.Interface;
using System;
using System.Linq;

namespace Service
{
    public class RecuperarSenhaService : IRecuperarSenhaService
    {
        private readonly monitorasusContext _context;
        public RecuperarSenhaService(monitorasusContext context)
        {
            _context = context;
        }

        public RecuperarSenhaModel GetByUser(int idUser)
            => _context
                .Recuperarsenha
                .Where(t => t.IdUsuario == idUser)
                .Select(t => new RecuperarSenhaModel
                {
                    Id = t.Id,
                    Token = t.Token,
                    InicioToken = t.InicioToken,
                    FimToken = t.FimToken,
                    EhValido = t.EhValido,
                    IdUsuario = t.IdUsuario
                })
                .FirstOrDefault();

        public bool Insert(RecuperarSenhaModel recuperarSenha)
        {
            if (recuperarSenha != null)
            {
                try
                {
                    var entity = ModelToEntity(recuperarSenha, new Recuperarsenha());
                    entity.FimToken = recuperarSenha.InicioToken.AddDays(1);
                    _context.Recuperarsenha.Add(entity);
                    return _context.SaveChanges() == 1 ? true : false;
                }
                catch (Exception e)
                {
                    throw e.InnerException;
                }
            }
            return false;
        }

        public bool IsTokenValid(string token)
            => _context
                .Recuperarsenha
                .Where(t => t.Token.Equals(token) && CheckValidateToken(t.InicioToken, t.FimToken, t.EhValido))
                .Select(t => true)
                .FirstOrDefault();

        public bool UserNotHasToken(int idUser) => _context.Recuperarsenha.Where(t => t.IdUsuario == idUser && CheckValidateToken(t.InicioToken, t.FimToken, t.EhValido)).Count() == 0 ? true : false;

        private Recuperarsenha ModelToEntity(RecuperarSenhaModel model, Recuperarsenha entity)
        {
            entity.Id = model.Id;
            entity.Token = model.Token;
            entity.InicioToken = model.InicioToken;
            entity.FimToken = model.FimToken;
            entity.EhValido = model.EhValido;
            entity.IdUsuario = model.IdUsuario;

            return entity;
        }

        private bool CheckValidateToken(DateTime inicio, DateTime fim, byte situacaoToken) => (DateTime.Now >= inicio && DateTime.Now < fim) && situacaoToken == 1;

        public RecuperarSenhaModel GetByToken(string token)
            => _context
                .Recuperarsenha
                .Where(t => t.Token.Equals(token) && CheckValidateToken(t.InicioToken, t.FimToken, t.EhValido))
                .Select(t => new RecuperarSenhaModel
                {
                    Id = t.Id,
                    Token = t.Token,
                    InicioToken = t.InicioToken,
                    FimToken = t.FimToken,
                    EhValido = t.EhValido,
                    IdUsuario = t.IdUsuario
                })
                .FirstOrDefault();

        public void SetTokenInvalid(int idUser)
        {
            var tokens = _context.Recuperarsenha.Where(t => t.IdUsuario == idUser).ToList();
            tokens.ForEach(token =>
            {
                token.EhValido = Convert.ToByte(false);
                _context.Recuperarsenha.Update(token);
            });
            _context.SaveChanges();
        }

        public void DeleteByUser(int idUser)
        {
            var tokens = _context.Recuperarsenha.Where(t => t.IdUsuario == idUser).ToList();
            tokens.ForEach(token =>
            {

                _context.Recuperarsenha.Remove(token);
            });
            _context.SaveChanges();
        }

		
	}
}
