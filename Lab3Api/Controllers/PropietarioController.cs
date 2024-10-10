using Lab3Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCrypt.Net;
using Lab3Api.Models;

namespace Lab3Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropietarioController : ControllerBase
    {
        private readonly RPropietario _repositorio;

        public PropietarioController(RPropietario repositorio)
        {
            _repositorio = repositorio;
        }

        // reurn todos los propietarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Propietario>>> GetAll()
        {
            var propietarios = await _repositorio.GetAllAsync();
            return Ok(propietarios);
        }

        // return un propietario específico por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Propietario>> GetById(int id)
        {
            var propietario = await _repositorio.GetByIdAsync(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return Ok(propietario);
        }

        [HttpPost]
        public async Task<ActionResult<Propietario>> AddAsync(Propietario propietario)
        {
            try
            {
                Console.WriteLine(propietario.ToString());
                if (propietario == null)
                {
                    return BadRequest("Propietario es null");
                }
                propietario.Password = BCrypt.Net.BCrypt.HashPassword(propietario.Password);
                await _repositorio.AddAsync(propietario);
                return Ok(propietario);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<Propietario>> UpdateAsync(Propietario propietario)
        {
            try
            {
                if (propietario == null)
                {
                    return BadRequest("Propietario es null");
                }
                await _repositorio.UpdateAsync(propietario);
                return Ok(propietario);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }


        /// Elimina un propietario por id.   
        /// param name="id"
        /// return Propietario
        /// response code="404" No se encontró el propietario solicitado.
        /// response code="400" Error al eliminar el propietario.

        [HttpDelete("{id}")]
        public async Task<ActionResult<Propietario>> DeleteAsync(int id)
        {
            try
            {

                var propietario = await _repositorio.GetByIdAsync(id);
                if (propietario == null)
                {
                    return NotFound();
                }

                propietario.Borrado = true;
                await _repositorio.PatchAsync(propietario);
                return Ok(propietario);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
        [HttpPatch]
        public async Task<ActionResult<Propietario>> PatchAsync(Propietario propietario)
        {
            try
            {
                if (propietario == null)
                {
                    return BadRequest("Propietario es null");
                }
                if (!string.IsNullOrEmpty(propietario.Password))
                {
                    propietario.Password = BCrypt.Net.BCrypt.HashPassword(propietario.Password);

                    Console.WriteLine(propietario.Password);
                }
                await _repositorio.PatchAsync(propietario);
                return Ok(propietario);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<Propietario>> Login(string email, string password)
        {
            try
            {
                // Busca el propietario por el email
                var propietario = await _repositorio.FindByEmailAsync(email);

                // Verifica si el propietario existe
                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado.");
                }

                // Verifica la contraseña
                if (BCrypt.Net.BCrypt.Verify(password, propietario.Password))
                {
                    // Aquí podrías agregar la lógica para crear un token JWT o manejar la sesión
                    return Ok(propietario); // Retorna el propietario o el token de sesión
                }
                else
                {
                    return BadRequest("Contraseña incorrecta.");
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                Console.WriteLine($"Error en el inicio de sesión: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}