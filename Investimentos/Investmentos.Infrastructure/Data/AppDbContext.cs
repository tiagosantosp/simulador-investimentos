using Investimentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Investimentos.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PriceHistory> PriceHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Position>()
                .HasOne(p => p.Asset)
                .WithMany()
                .HasForeignKey(p => p.AssetSymbol)
                .HasPrincipalKey(a => a.Symbol); 
        }
    }
}
