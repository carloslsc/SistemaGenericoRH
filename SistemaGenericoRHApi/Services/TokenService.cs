﻿using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SistemaGenericoRHData;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGenericoRHApi.Services
{
    public class TokenService
    {
        private IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerarToken(Usuarios usuario)
        {
            //Accedemos a la sección JwtSettings del archivo appsettings.json
            var jwtSettings = _configuration.GetSection("JwtSettings");
            //Obtenemos la clave secreta guardada en JwtSettings:SecretKey
            string secretKey = jwtSettings.GetValue<string>("SecretKey");
            //Obtenemos el tiempo de vida en minutos del Jwt guardada en JwtSettings:MinutesToExpiration
            int minutes = jwtSettings.GetValue<int>("MinutesToExpiration");
            //Obtenemos el valor del emisor del token en JwtSettings:Issuer
            string issuer = jwtSettings.GetValue<string>("Issuer");
            //Obtenemos el valor de la audiencia a la que está destinado el Jwt en JwtSettings:Audience
            string audience = jwtSettings.GetValue<string>("Audience");

            var key = Encoding.ASCII.GetBytes(secretKey);

            //Creamos nuestra lista de Claims, en este caso para el Username,
            //el Email y el Perfil del Usuario.
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Sid, usuario.Id.ToString()));

            // Creamos el objeto JwtSecurityToken
            var token = new JwtSecurityToken(
              issuer: issuer,
              audience: audience,
              claims: claims,
              notBefore: DateTime.UtcNow,
              expires: DateTime.UtcNow.AddMinutes(minutes),
              signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            // Creamos una representación en cadena del Token JWT (Json Web Token)
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
