using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaGenericoRHData.Models;

namespace SistemaGenericoRHData.Repositorio
{
    public class SesionRepositorio
    {
        private readonly db_sistema_generico_rhContext _context;
        private readonly ILogger<SesionRepositorio> _logger;
        private readonly IPasswordHasher<Usuarios> _passwordHasher;

        public SesionRepositorio(db_sistema_generico_rhContext context, ILogger<SesionRepositorio> logger, IPasswordHasher<Usuarios> passwordHasher)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task<(bool resultado, Usuarios usuario)> ValidarDatosLogin(UsuarioDTO datosLoginUsuario)
        {
            try
            {

                string passwordIngresada = GetSHA256(datosLoginUsuario.Contrasenia);


                var user = await (from u in _context.Usuarios
                                   where (u.Usuario == datosLoginUsuario.Usuario || u.Correo == datosLoginUsuario.Usuario)
                                   select u
                                   ).FirstOrDefaultAsync();

                if (user != null)
                {
                    if (passwordIngresada == user.Contrasena)
                    {
                        return (true, user);
                    }
                }
                return (false, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en {nameof(ValidarDatosLogin)}: " + ex.Message);
                return (false, null);
            }
        }

        #region Metodos privados
        private string GetSHA256(string str)
        {
            try
            {
                SHA256 sha256 = SHA256Managed.Create();
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] stream = null;
                StringBuilder sb = new StringBuilder();
                stream = sha256.ComputeHash(encoding.GetBytes(str));
                for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en {nameof(GetSHA256)}: " + ex.Message);
                return null;
            }

        }
        #endregion
    }
}
