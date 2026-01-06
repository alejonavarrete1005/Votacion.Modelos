using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Votacion.Modelos.Enums;

namespace Votacion.Modelos
{
    public class Eleccion
    {
        [Key]
        public int EleccionId { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public TipoEleccion Tipo { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public bool Cerrada { get; set; } = false;

        // 🔗 Navegación
        public ICollection<Candidato> Candidatos { get; set; }
        public ICollection<Voto> Votos { get; set; }
    }
}
