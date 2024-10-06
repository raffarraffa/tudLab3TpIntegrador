using Microsoft.EntityFrameworkCore;
using Lab3Api.Models;

namespace Lab3Api.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }

        public DbSet<Propietario> Propietarios { get; set; }


    }
}
