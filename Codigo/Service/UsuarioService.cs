using Model;
using Persistence;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                    Email = model.Email
                }).ToList();


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
                    Email = model.Email
                }).FirstOrDefault();

        public UsuarioModel GetByLogin(string cpf, string senha)
         => _context
                .Usuario
                .Where(model => model.Cpf == cpf && model.Senha == senha)
                .Select(model => new UsuarioModel
                {
                    IdUsuario = model.IdUsuario,
                    Cpf = model.Cpf,
                    Senha = model.Senha,
                    TipoUsuario = Convert.ToByte(model.TipoUsuario),
                    Email = model.Email
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

        private Usuario ModelToEntity(UsuarioModel model, Usuario entity)
        {
            entity.IdUsuario = model.IdUsuario;
            entity.Cpf = model.Cpf;
            entity.Senha = model.Senha;
            entity.TipoUsuario = Convert.ToByte(model.TipoUsuario);
            entity.Email = model.Email;

            return entity;
        }
    }
}
