using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SistemaGenericoRHData.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGenericoRHData.Repositorio
{
    public class UsuarioRepositorio
    {
        private readonly db_sistema_generico_rhContext _context;
        private readonly ILogger<UsuarioRepositorio> _logger;
        private readonly IPasswordHasher<Usuarios> _passwordHasher;

        public UsuarioRepositorio(db_sistema_generico_rhContext context, ILogger<UsuarioRepositorio> logger, IPasswordHasher<Usuarios> passwordHasher)
        {
            this._context = context;
            this._logger = logger;
            this._passwordHasher = passwordHasher;
        }

        #region CREATE
        public async Task<UsuarioDTO> GuardarUsuario(UsuarioDTO usuario, long idUsuario)
        {
            try
            {
                var pass = GetSHA256(usuario.Contrasenia);

                Usuarios userSave = new Usuarios
                {
                    Correo = usuario.Correo,
                    Usuario = usuario.Usuario,
                    Contrasena = pass,
                    Sexo = usuario.Sexo.ToString(),
                    FechaAlta = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    UsuarioAlta = idUsuario,
                    UsuarioModificacion = idUsuario,
                    Estatus = true
                };

                this._context.Usuarios.Add(userSave);
                await this._context.SaveChangesAsync();

                if (userSave.Id < 1)
                {
                    return null;
                }

                return new UsuarioDTO { 
                    Correo = userSave.Correo,
                    Usuario = userSave.Usuario,
                    Sexo = userSave.Sexo[0]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {nameof(GuardarUsuario)}: ${ex.Message}");
                return null;
            }
        }
        #endregion

        #region READ
        public async Task<UsuarioDTO> LeerUsuarioByCorreoOrUser(string strUserCorreo)
        {
            try
            {
                var userRead = await (from u in _context.Usuarios
                                      where (u.Correo == strUserCorreo || u.Usuario == strUserCorreo) && u.Estatus
                                      select new UsuarioDTO
                                      {
                                          Contrasenia = u.Contrasena,
                                          Correo = u.Correo,
                                          Estatus = u.Estatus,
                                          Sexo = u.Sexo[0],
                                          Usuario = u.Usuario
                                      }).FirstOrDefaultAsync();

                if (userRead == null)
                {
                    return null;
                }

                return userRead;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {nameof(LeerUsuarioByCorreoOrUser)}: ${ex.Message}");
                return null;
            }
        }

        public async Task<List<UsuarioDTO>> LeerListaUsuarios()
        {
            try
            {
                var darrUsers = await (from u in this._context.Usuarios
                                       where u.Estatus
                                       select new UsuarioDTO
                                       {
                                           Contrasenia = u.Contrasena,
                                           Correo = u.Correo,
                                           Estatus = u.Estatus,
                                           Sexo = u.Sexo[0],
                                           Usuario = u.Usuario
                                       }).ToListAsync();

                if (darrUsers == null)
                {
                    return null;
                }

                return darrUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {nameof(LeerListaUsuarios)}: ${ex.Message}");
                return null;
            }
        }
        #endregion

        #region Update
        public async Task<UsuarioDTO> ActualizarUsuario(UsuarioDTO usuario, long idUsuario)
        {
            try
            {
                var usuarioUpd = await (from u in this._context.Usuarios
                                   where (u.Correo == usuario.UsuarioUpd)
                                   select u).FirstOrDefaultAsync();

                if (usuarioUpd == null)
                {
                    return null;
                }

                usuarioUpd.Usuario = usuario.Usuario;
                usuarioUpd.Correo = usuario.Correo;
                usuarioUpd.Sexo = usuario.Sexo.ToString();
                usuarioUpd.UsuarioModificacion = usuarioUpd.Id;
                usuarioUpd.FechaModificacion = DateTime.Now;

                await this._context.SaveChangesAsync();

                return new UsuarioDTO
                {
                    Correo = usuarioUpd.Correo,
                    Usuario = usuarioUpd.Usuario,
                    Sexo = usuarioUpd.Sexo[0],
                    Estatus = usuarioUpd.Estatus
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {nameof(ActualizarUsuario)}: ${ex.Message}");
                return null;
            }
        }

        public async Task<int> ActualizarPasswordUsuario(UsuarioDTO usuario, int idUsuario)
        {
            try
            {
                var userUpdPass = await (from u in this._context.Usuarios
                                         where (u.Usuario == usuario.Usuario || u.Correo == usuario.Correo) && u.Estatus
                                         select u).FirstOrDefaultAsync();

                string passHashed = GetSHA256(usuario.Contrasenia);

                
                userUpdPass.Contrasena = passHashed;
                userUpdPass.UsuarioModificacion = idUsuario;
                userUpdPass.FechaModificacion = DateTime.Now;

                var respuesta = await this._context.SaveChangesAsync();
                
                if (respuesta > 0)
                {
                    return 1;
                }
                
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {nameof(ActualizarPasswordUsuario)}: ${ex.Message}");
                return -1;
            }
        }
        #endregion

        #region DELETE
        public async Task<int> EliminarUsuario(string strUserCorreo, long idUsuario)
        {
            try
            {
                Usuarios userDel = await (from u in this._context.Usuarios
                                          where (u.Correo == strUserCorreo || u.Usuario == strUserCorreo) && u.Estatus
                                          select u).FirstOrDefaultAsync();
                if (userDel != null)
                {
                    userDel.Estatus = false;
                    userDel.UsuarioModificacion = idUsuario;
                    userDel.FechaModificacion = DateTime.Now;

                    await this._context.SaveChangesAsync();

                    return 1;
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {nameof(EliminarUsuario)}: ${ex.Message}");
                return -1;
            }
        }
        #endregion

        #region Metodos privados
        private static string GetSHA256(string str)
        {
            //METODO DE ENCRIPTACION PARA LA CONTRASEÑA
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
        #endregion
    }
}
