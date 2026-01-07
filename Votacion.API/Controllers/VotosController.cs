using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Votacion.Modelos;
using Votacion.Modelos.DTOs;

namespace Votacion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VotosController : ControllerBase
    {
        private readonly VotacionAPIContext _context;

        public VotosController(VotacionAPIContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostVoto(VotoDTO dto)
        {
            var candidato = await _context.Candidatos
                .FirstOrDefaultAsync(c => c.CandidatoId == dto.CandidatoId);

            if (candidato == null)
                return BadRequest("El candidato no existe");

            var yaVoto = await _context.Votos.AnyAsync(v =>
                v.UsuarioId == dto.UsuarioId &&
                v.EleccionId == candidato.EleccionId);

            if (yaVoto)
                return BadRequest("El usuario ya votó en esta elección");

            var voto = new Voto
            {
                UsuarioId = dto.UsuarioId,
                CandidatoId = dto.CandidatoId,
                EleccionId = candidato.EleccionId,
                FechaRegistro = DateTime.UtcNow,
                HashVotante = Guid.NewGuid().ToString()
            };

            _context.Votos.Add(voto);
            await _context.SaveChangesAsync();

            return Ok(voto);
        }

    }
}
