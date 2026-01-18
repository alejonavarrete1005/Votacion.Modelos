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

        // Claves foráneas
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        [ForeignKey("Eleccion")]
        public int EleccionId { get; set; }

        [ForeignKey("Candidato")]
        public int CandidatoId { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Hash para anonimato
        [Required]
        public string HashVotante { get; set; } = string.Empty;

        // Navegaciones
        public Usuario Usuario { get; set; }
        public Eleccion Eleccion { get; set; }
        public Candidato Candidato { get; set; }
    }

}
