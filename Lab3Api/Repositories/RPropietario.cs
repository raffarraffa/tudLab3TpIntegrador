using BCrypt.Net;
using Lab3Api.Data;
using Lab3Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
            var sql = _context.Propietario.ToQueryString();
            Console.WriteLine($"Consulta SQL ejecutada: {sql}");

            return await _context.Propietario.ToListAsync();
        }


        /// Obtiene un propietario por su id.
        /// param name="id" Id del propietario
        /// return  propietario si existe o null si no existe.

        public async Task<Propietario?> GetByIdAsync(int id)
        {
            return await _context.Propietario.FindAsync(id);
        }

        /// Agrega un nuevo propietario en la base de datos.
        /// param name="entidad" Entidad del propietario a agregar.
        public async Task AddAsync(Propietario entidad)
        {
            await _context.Propietario.AddAsync(entidad);
            await SaveAsync(); // Guardar cambios
        }

        /// Actualiza un propietario en la base de datos.
        /// param name="entidad" Entidad del propietario a actualizar
        public async Task UpdateAsync(Propietario entidad)
        {
            var sql = _context.Propietario.ToQueryString();
            Console.WriteLine(GetType().Name + $" Consulta SQL ejecutada: {sql}");
            _context.Propietario.Update(entidad);
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
                _context.Propietario.Remove(propietario);
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


        public async Task PatchAsync(Propietario propietario)
        {
            //aca debo implentar la captura del usuario segun el JWT, en lugar de confiar en los datos recibidos
            //de moemnto pra debugear sirve
            var propietarioDb = await _context.Propietario.FindAsync(propietario.Id);
            if (propietarioDb == null)
            {
                throw new Exception("El propietario no existe en la base de datos.");
            }
            if (propietario.Password.IsNullOrEmpty())
            {
                propietario.Password = propietarioDb.Password;
                Console.WriteLine(propietarioDb.Password);
                Console.WriteLine(propietario.Password);
            }
            else
            {
                propietario.Password = BCrypt.Net.BCrypt.HashPassword(propietario.Password);
                Console.WriteLine(propietarioDb.Password);
                Console.WriteLine(propietario.Password);
            }
            var sql = _context.Propietario.ToQueryString();
            Console.WriteLine($"Consulta SQL ejecutada: {sql}");
            _context.Entry(propietarioDb).CurrentValues.SetValues(propietario);
            await _context.SaveChangesAsync();
        }

    }
}
