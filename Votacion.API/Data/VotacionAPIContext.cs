using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Votacion.Modelos;

    public class VotacionAPIContext : DbContext
    {
        public VotacionAPIContext (DbContextOptions<VotacionAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Votacion.Modelos.Eleccion> Elecciones { get; set; } = default!;

        public DbSet<Votacion.Modelos.Candidato> Candidatos { get; set; } = default!;

        public DbSet<Votacion.Modelos.Voto> Votos { get; set; } = default!;
        public DbSet<Votacion.Modelos.Usuario> Usuarios { get; set; } = default!;
        public DbSet<Votacion.Modelos.TokenSesion> TokenSesions { get; set; } = default!;
        
}
