using System;
using System.Collections.Generic;
using System.Text;
using Votacion.Modelos.Enums;

namespace Votacion.Modelos.DTOs
{
    public class EleccionDTO
    {
        public int EleccionId { get; set; }   
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public TipoEleccion Tipo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
