using Votacion.Modelos.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votacion.Modelos
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required, MaxLength(100)]
        public string NombreCompleto { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public RolUsuario Rol { get; set; }

        public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<Voto> Votos { get; set; }
        public ICollection<Candidato> Candidaturas { get; set; }
    }
}
