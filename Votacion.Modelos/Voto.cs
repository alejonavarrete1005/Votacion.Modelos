using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Votacion.Modelos
{
    public class Voto
    {
        [Key]
        public int VotoId { get; set; }

        [ForeignKey("Eleccion")]
        public int EleccionId { get; set; }

        [ForeignKey("Candidato")]
        public int CandidatoId { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Hash para evitar doble voto (no identifica al usuario)
        [Required]
        public string HashVotante { get; set; }

        // 🔗 Navegación
        public Eleccion Eleccion { get; set; }
        public Candidato Candidato { get; set; }
    }
}
