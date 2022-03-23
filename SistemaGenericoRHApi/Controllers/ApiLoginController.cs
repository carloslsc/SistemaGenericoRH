using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaGenericoRHApi.Services;
using SistemaGenericoRHData;
using SistemaGenericoRHData.Models;
using SistemaGenericoRHData.Repositorio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SistemaGenericoRHApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiLoginController : ControllerBase
    {
        private SesionRepositorio _repositorio;
        private TokenService _tokenService;
        private readonly ILogger<ApiLoginController> _logger;

        public ApiLoginController(SesionRepositorio repositorio, TokenService tokenService, ILogger<ApiLoginController> logger)
        {
            _repositorio = repositorio;
            _tokenService = tokenService;
            _logger = logger;
        }

        // GET: api/<ApiLoginController>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TokensDTO>> PostLogin(UsuarioDTO usuarioLogin)
        {
            var resultadoValidacion = await _repositorio.ValidarDatosLogin(usuarioLogin);
            if (!resultadoValidacion.resultado)
            {
                return null;
            }

            var token = _tokenService.GenerarToken(resultadoValidacion.usuario);
            
            TokensDTO data = new TokensDTO
            {
                TokenAcceso = token,
                Usuario = resultadoValidacion.usuario.Usuario
            };

            return data;
        }
    }
}
