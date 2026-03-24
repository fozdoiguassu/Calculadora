
using CalculadoraDebitos.Models;
using Microsoft.EntityFrameworkCore;

namespace CalculadoraDebitos.Data
{

    //
    public class AppDbContext : DbContext
    {
        public DbSet<UffiAnual> UffiAnual { get; set; }

        public DbSet<SelicMensal> SelicMensal { get; set; }

        public DbSet<CalculoDebito> Calculos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SelicMensal>()
                .Property(s => s.PercentualMensal)
                .HasConversion(
                    v => v.ToString(System.Globalization.CultureInfo.InvariantCulture), // Salva com ponto
                    v => decimal.Parse(v.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture) // Lê tratando vírgula como ponto
                );
        }
    }
}
