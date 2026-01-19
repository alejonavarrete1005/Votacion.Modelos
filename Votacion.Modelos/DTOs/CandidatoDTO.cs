using System;
using System.Collections.Generic;
using System.Text;

namespace Votacion.Modelos.DTOs
{
    public class CandidatoDTO
    {
        public int CandidatoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Propuesta { get; set; } = string.Empty;
        public int EleccionId { get; set; }
    }

}
