using Microsoft.EntityFrameworkCore;
using opentelemetry_jaeger_api.Models;

namespace opentelemetry_jaeger_api.Data
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Carro>().HasKey(c => c.Id);
        }

        public DbSet<Carro> Carros { get; set; }
    }
}
