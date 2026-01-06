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
    public class EleccionesController : ControllerBase
    {
        private readonly VotacionAPIContext _context;

        public EleccionesController(VotacionAPIContext context)
        {
            _context = context;
        }

        // GET: api/Elecciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Eleccion>>> GetElecciones()
        {
            return await _context.Elecciones
                .Include(e => e.Candidatos)
                .ToListAsync();
        }

        // GET: api/Elecciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Eleccion>> GetEleccion(int id)
        {
            var eleccion = await _context.Elecciones
                .Include(e => e.Candidatos)
                .FirstOrDefaultAsync(e => e.EleccionId == id);

            if (eleccion == null)
                return NotFound();

            return eleccion;
        }

        // POST: api/Elecciones
        [HttpPost]
        public async Task<ActionResult<Eleccion>> PostEleccion(EleccionDTO dto)
        {
            var eleccion = new Eleccion
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Tipo = dto.Tipo,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin
            };

            _context.Elecciones.Add(eleccion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEleccion),
                new { id = eleccion.EleccionId }, eleccion);
        }

        // PUT: api/Elecciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEleccion(int id, EleccionDTO dto)
        {
            var eleccion = await _context.Elecciones.FindAsync(id);
            if (eleccion == null)
                return NotFound();

            eleccion.Nombre = dto.Nombre;
            eleccion.Descripcion = dto.Descripcion;
            eleccion.Tipo = dto.Tipo;
            eleccion.FechaInicio = dto.FechaInicio;
            eleccion.FechaFin = dto.FechaFin;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Elecciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEleccion(int id)
        {
            var eleccion = await _context.Elecciones.FindAsync(id);
            if (eleccion == null)
                return NotFound();

            _context.Elecciones.Remove(eleccion);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
