using System;
using System.Collections.Generic;
using System.Text;
using Votacion.Modelos.Enums;

namespace Votacion.Modelos.DTOs
{
    public class UsuarioRegisterDTO
    {
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public RolUsuario Rol { get; set; }
    }
}
