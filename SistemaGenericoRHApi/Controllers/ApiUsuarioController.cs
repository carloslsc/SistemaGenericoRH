using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaGenericoRHData.Models;
using SistemaGenericoRHData.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaGenericoRHApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApiUsuarioController : ControllerBase
    {
        private readonly UsuarioRepositorio _repositorio;
        private readonly ILogger<ApiUsuarioController> _logger;

        public ApiUsuarioController(UsuarioRepositorio repositorio, ILogger<ApiUsuarioController> logger)
        {
            this._repositorio = repositorio;
            this._logger = logger;
        }

        [HttpGet("{cadenaBusqueda}")]
        [Route("DataUsuarioByEmail/{cadenaBusqueda}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UsuarioDTO>> GetDataUsuarioByEmail(string cadenaBusqueda)
        {
            try
            {
                var result = await this._repositorio.LeerUsuarioByCorreoOrUser(cadenaBusqueda);

                if (result == null)
                {
                    return NoContent();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ${nameof(GetDataUsuarioByEmail)}: ${ex.Message}");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("ListDataUsuarios")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetListDataUsuario()
        {
            try
            {
                var result = await this._repositorio.LeerListaUsuarios();

                if (result.Count > 0)
                {
                    return Ok(result);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ${nameof(GetListDataUsuario)}: ${ex.Message}");
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("saveUsuario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostSaveUsuario(UsuarioDTO usuario)
        {
            try
            {
                var claimId = HttpContext.User.Claims.Where(claim => claim.Type.Contains("sid")).FirstOrDefault();
                int intUserId = 0;
                Int32.TryParse(claimId.Value, out intUserId);

                var userToCheck = await _repositorio.LeerUsuarioByCorreoOrUser(usuario.UsuarioUpd);

                bool UserExist = (userToCheck != null) ? true : false;

                if (!UserExist && usuario.IsSave)
                {
                    var result = await this._repositorio.GuardarUsuario(usuario, intUserId);
                    if (result != null)
                    {
                        return Created(nameof(PostSaveUsuario), result);
                    }

                    return NoContent();
                }
                else if (UserExist && !usuario.IsSave)
                {
                    var result = await this._repositorio.ActualizarUsuario(usuario, intUserId);
                    if (result != null)
                    {
                        return Ok(result);
                    }

                    return NoContent();
                }
                else
                {
                    return Accepted();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ${nameof(PostSaveUsuario)}: ${ex.Message}");
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("deleteUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> PostDeleteUsuario(UsuarioDTO usuario)
        {
            try
            {
                var claimId = HttpContext.User.Claims.Where(claim => claim.Type.Contains("sid")).FirstOrDefault();
                int intUserId = 0;
                Int32.TryParse(claimId.Value, out intUserId);

                var result = await this._repositorio.EliminarUsuario(usuario.Correo, intUserId);

                if (result > 0)
                {
                    return Ok(result);
                }
                else
                {
                    return Accepted();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ${nameof(PostDeleteUsuario)}: ${ex.Message}");
                return BadRequest();
            }
        }

        [HttpPut("{Contrasenia}")]
        [Route("updatePasswordUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PutUpdatePasswordUsuario(UsuarioDTO usuario)
        {
            try
            {
                var claimId = HttpContext.User.Claims.Where(claim => claim.Type.Contains("sid")).FirstOrDefault();
                int intUserId = 0;
                Int32.TryParse(claimId.Value, out intUserId);

                var result = await this._repositorio.ActualizarPasswordUsuario(usuario, intUserId);

                if (result > 0)
                {
                    return Ok(result);
                }
                else
                {
                    return Accepted();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ${nameof(PutUpdatePasswordUsuario)}: ${ex.Message}");
                return BadRequest();
            }
        }
    }
}
