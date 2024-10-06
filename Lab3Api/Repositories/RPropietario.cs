using Lab3Api.Data;
using Lab3Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab3Api.Repositories
{
    // Repositorio de Propietarios
    public class RPropietario : IRepository<Propietario>
    {
        private readonly ApiDbContext _context;

        public RPropietario(ApiDbContext context)
        {
            _context = context;
        }


        /// Obtiene todos los propietarios en la base de datos.
        /// return  lista de propietarios.        

        public async Task<IEnumerable<Propietario>> GetAllAsync()
        {
            return await _context.Propietarios.ToListAsync();
        }


        /// Obtiene un propietario por su id.
        /// param name="id" Id del propietario
        /// return  propietario si existe o null si no existe.

        public async Task<Propietario?> GetByIdAsync(int id)
        {
            return await _context.Propietarios.FindAsync(id);
        }

        /// Agrega un nuevo propietario en la base de datos.
        /// param name="entidad" Entidad del propietario a agregar.
        public async Task AddAsync(Propietario entidad)
        {
            await _context.Propietarios.AddAsync(entidad);
            await SaveAsync(); // Guardar cambios
        }

        /// Actualiza un propietario en la base de datos.
        /// param name="entidad" Entidad del propietario a actualizar
        public async Task UpdateAsync(Propietario entidad)
        {
            _context.Propietarios.Update(entidad);
            await SaveAsync(); // Guardar cambios
        }


        /// Elimina un propietario en la base de datos por su id.        
        /// param name="id" Id del propietario a eliminar
        /// Si el propietario no existe, no se hace nada.

        public async Task DeleteAsync(int id)
        {
            var propietario = await GetByIdAsync(id);
            if (propietario != null)
            {
                _context.Propietarios.Remove(propietario);
                await SaveAsync(); // Guardar cambios
            }
        }

        /// Guarda los cambios en la base de datos.        
        /// Llama a DbContext.SaveChangesAsync        
        ///  Guarda los cambios en la base de datos.        

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync(); // Guardar cambios
        }
    }
}
