using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Votacion.Modelos;
using Votacion.Modelos.DTOs;


namespace Votacion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class VotosController : ControllerBase
    {
        private readonly VotacionAPIContext _context;

        public VotosController(VotacionAPIContext context)
        {
            _context = context;
        }

        // POST: api/Votos
        [HttpPost]
        public async Task<IActionResult> Votar([FromBody] VotoDTO dto)
        {
            // 1️ Obtener usuario desde el token
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (usuarioIdClaim == null)
                return Unauthorized("Token inválido");

            var usuarioId = int.Parse(usuarioIdClaim.Value);

            // 2️ Verificar que el usuario exista
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.UsuarioId == usuarioId);

            if (!usuarioExiste)
                return BadRequest("Usuario no válido");

            // 3️ Buscar candidato y su elección
            var candidato = await _context.Candidatos
                .Include(c => c.Eleccion)
                .FirstOrDefaultAsync(c => c.CandidatoId == dto.CandidatoId);

            if (candidato == null)
                return BadRequest("Candidato no válido");

            // 4️ Verificar elección activa
            var ahora = DateTime.UtcNow;

            if (ahora < candidato.Eleccion.FechaInicio ||
                ahora > candidato.Eleccion.FechaFin)
            {
                return BadRequest("La elección no está activa");
            }

            // 5️ BLOQUEO REAL DE DOBLE VOTO 
            var yaVoto = await _context.Votos.AnyAsync(v =>
                v.EleccionId == candidato.EleccionId &&
                v.UsuarioId == usuarioId
            );

            if (yaVoto)
                return BadRequest("El usuario ya votó en esta elección");

            // 6️ Registrar voto
            var voto = new Voto
            {
                UsuarioId = usuarioId,
                EleccionId = candidato.EleccionId,
                CandidatoId = candidato.CandidatoId,
                FechaRegistro = DateTime.UtcNow
            };

            _context.Votos.Add(voto);
            await _context.SaveChangesAsync();

            return Ok("Voto registrado correctamente");
        }

        // GET: api/Votos/ya-voto/{eleccionId}
        [HttpGet("ya-voto/{eleccionId}")]
        public async Task<IActionResult> YaVoto(int eleccionId)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (usuarioIdClaim == null)
                return Unauthorized();

            var usuarioId = int.Parse(usuarioIdClaim.Value);

            var yaVoto = await _context.Votos.AnyAsync(v =>
                v.EleccionId == eleccionId &&
                v.UsuarioId == usuarioId
            );

            return Ok(yaVoto);
        }
    }
}
