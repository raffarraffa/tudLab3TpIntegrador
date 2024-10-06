using Lab3Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab3Api.Repositories
{
    public interface IRepository<T>
    {
        // ABM todas las entidades deben implementar
        Task<IEnumerable<T>> GetAllAsync(); // trae todos los registros
        Task<T?> GetByIdAsync(int id); // trae un registro por id
        Task AddAsync(T entidad); // agrega un registro
        Task UpdateAsync(T entidad);  // actualiza un registro
        Task DeleteAsync(int id); // elimina un registro
        Task SaveAsync(); // guarda los cambios en base
        Task PatchAsync(T entidad);
    }
}