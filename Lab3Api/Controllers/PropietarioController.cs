using Lab3Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lab3Api.Models;

namespace Lab3Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropietarioController : ControllerBase
    {
        private readonly IRepository<Propietario> _repositorio;

        public PropietarioController(IRepository<Propietario> repositorio)
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
                await _repositorio.DeleteAsync(id);
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
                await _repositorio.PatchAsync(propietario);
                return Ok(propietario);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
    }
}