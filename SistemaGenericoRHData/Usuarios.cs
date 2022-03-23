using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace SistemaGenericoRHData
{
    public partial class Usuarios
    {
        public long Id { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public string Correo { get; set; }
        public string Sexo { get; set; }
        public bool Estatus { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public long? UsuarioModificacion { get; set; }
    }
}
