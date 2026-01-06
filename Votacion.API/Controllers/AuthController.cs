using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;     
using Votacion.Modelos;           
using Votacion.Modelos.DTOs;      
using Votacion.Modelos.Enums;     


namespace Votacion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly VotacionAPIContext _context;

        public AuthController(VotacionAPIContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioRegisterDTO dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("El correo ya existe");

            var usuario = new Usuario
            {
                NombreCompleto = dto.NombreCompleto,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = RolUsuario.Votante,
                Activo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(usuario);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDTO dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null)
                return Unauthorized("Usuario no encontrado");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
                return Unauthorized("Contraseña incorrecta");

            return Ok(usuario);
        }

    }
}
