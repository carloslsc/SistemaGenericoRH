using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGenericoRHData.Models
{
    public class UsuarioDTO
    {
        public long Id { get; set; }
        public string Correo { get; set; }
        public string Usuario { get; set; }
        public string UsuarioUpd { get; set; }
        public string Contrasenia { get; set; }
        public char Sexo { get; set; }
        public bool Estatus { get; set; }
        public bool IsSave { get; set; }
    }
}
