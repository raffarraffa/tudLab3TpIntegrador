using Lab3Api.Repositories;
using Microsoft.AspNetCore.Mvc;

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

        // Ejemplo de una acción que devuelve todos los propietarios
        [HttpGet]
        public IActionResult GetAll()
        {
            var propietarios = _repositorio.GetAllAsync();
            return Ok(propietarios);
        }

        // Ejemplo de una acción que devuelve un propietario específico por ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var propietario = _repositorio.GetByIdAsync(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return Ok(propietario);
        }

    }
}
