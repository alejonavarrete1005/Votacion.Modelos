using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Votacion.Modelos
{
    public class TokenSesion
    {
        [Key]
        public int TokenSesionId { get; set; }

        public string Token { get; set; }

        public DateTime Expira { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; }
    }
}
