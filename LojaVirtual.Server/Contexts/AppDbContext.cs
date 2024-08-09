using LojaVirtual.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Server.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        // Registrandos os modelos
        public DbSet<Produto> Produtos { get; set; }
    }
}
