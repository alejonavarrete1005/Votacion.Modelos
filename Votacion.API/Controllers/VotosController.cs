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
        public async Task<IActionResult> PostVoto([FromBody] VotoDTO dto)
        {
            var eleccion = await _context.Elecciones.FindAsync(dto.EleccionId);
            if (eleccion == null || eleccion.Cerrada)
                return BadRequest("Elección inválida");

            var hash = Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()))
            );

            var voto = new Voto
            {
                EleccionId = dto.EleccionId,
                CandidatoId = dto.CandidatoId,
                HashVotante = hash
            };

            _context.Votos.Add(voto);
            await _context.SaveChangesAsync();

            return Ok(voto);
        }
    }
}
