using System;
using System.Collections.Generic;
using System.Text;

namespace Votacion.Modelos.DTOs
{
    public class ResultadoDTO
    {
        public int CandidatoId { get; set; }
        public string NombreCandidato { get; set; }
        public int TotalVotos { get; set; }
    }
}
