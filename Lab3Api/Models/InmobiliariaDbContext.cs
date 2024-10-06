using Microsoft.EntityFrameworkCore;

namespace Lab3Api.Models
{
    public class InmobiliariaDbContext : DbContext
    {
        public InmobiliariaDbContext(DbContextOptions<InmobiliariaDbContext> options) : base(options) { }

        public async Task<bool> TestConeccion()
        {
            try
            {

                var result = await this.Database.ExecuteSqlRawAsync("SELECT 1 + 1");
                return result == 2;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
