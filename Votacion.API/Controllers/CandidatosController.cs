using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Votacion.Modelos;
using Votacion.Modelos.DTOs;

namespace Votacion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidatosController : ControllerBase
    {
        private readonly VotacionAPIContext _context;

        public CandidatosController(VotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet("eleccion/{eleccionId}")]
        public async Task<IActionResult> GetCandidatos(int eleccionId)
        {
            var candidatos = await _context.Candidatos
                .Include(c => c.Usuario) 
                .Where(c => c.EleccionId == eleccionId)
                .Select(c => new CandidatoDTO
                {
                    CandidatoId = c.CandidatoId,
                    Nombre = c.Usuario.NombreCompleto,
                    Propuesta = c.Propuesta,
                    EleccionId = c.EleccionId
                })
                .ToListAsync();

            return Ok(candidatos);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PostCandidato(CandidatoCreateDTO dto)
        {
            var usuarioId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                return BadRequest("Usuario no válido");

            var eleccion = await _context.Elecciones.FindAsync(dto.EleccionId);
            if (eleccion == null)
                return BadRequest("Elección no válida");

            var candidato = new Candidato
            {
                UsuarioId = usuarioId,     
                EleccionId = dto.EleccionId,
                Propuesta = dto.Propuesta
            };

            _context.Candidatos.Add(candidato);
            await _context.SaveChangesAsync();

            return Ok(new CandidatoDTO
            {
                CandidatoId = candidato.CandidatoId,
                Nombre = usuario.NombreCompleto,   
                Propuesta = candidato.Propuesta,
                EleccionId = candidato.EleccionId
            });
        }

    }
}
