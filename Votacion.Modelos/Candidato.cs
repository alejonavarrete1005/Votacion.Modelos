using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Votacion.Modelos
{
    public class Candidato
    {
        [Key]
        public int CandidatoId { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        [ForeignKey("Eleccion")]
        public int EleccionId { get; set; }

        public string Propuesta { get; set; }

        // 🔗 Navegación
        public Usuario Usuario { get; set; }
        public Eleccion Eleccion { get; set; }

        public ICollection<Voto> Votos { get; set; }
    }
}
