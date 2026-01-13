using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Votacion.Modelos.DTOs;

namespace Votacion.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ResultadosController : ControllerBase
    {
        private readonly VotacionAPIContext _context;

        public ResultadosController(VotacionAPIContext context)
        {
            _context = context;
        }

        [HttpGet("{eleccionId}")]
        public async Task<ActionResult<IEnumerable<ResultadoDTO>>> GetResultados(int eleccionId)
        {
            var resultados = await _context.Votos
                .Where(v => v.EleccionId == eleccionId)
                .Include(v => v.Candidato)
                    .ThenInclude(c => c.Usuario)
                .GroupBy(v => new
                {
                    v.CandidatoId,
                    v.Candidato.Usuario.NombreCompleto
                })
                .Select(g => new ResultadoDTO
                {
                    CandidatoId = g.Key.CandidatoId,
                    NombreCandidato = g.Key.NombreCompleto,
                    TotalVotos = g.Count()
                })
                .ToListAsync();

            return Ok(resultados);
        }
    }
}

