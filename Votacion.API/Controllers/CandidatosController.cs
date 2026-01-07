using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                .ToListAsync();

            return Ok(candidatos);
        }

        [HttpPost]
        public async Task<IActionResult> PostCandidato(CandidatoCreateDTO dto)
        {
            var candidato = new Candidato
            {
                UsuarioId = dto.UsuarioId,
                EleccionId = dto.EleccionId,
                Propuesta = dto.Propuesta
            };

            _context.Candidatos.Add(candidato);
            await _context.SaveChangesAsync();

            return Ok(candidato);
        }

    }
}
