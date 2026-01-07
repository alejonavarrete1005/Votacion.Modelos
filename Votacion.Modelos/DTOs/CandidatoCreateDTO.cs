using System;
using System.Collections.Generic;
using System.Text;

namespace Votacion.Modelos.DTOs
{
    public class CandidatoCreateDTO
    {
        public int UsuarioId { get; set; }
        public int EleccionId { get; set; }
        public string Propuesta { get; set; } = string.Empty;
    }

}
