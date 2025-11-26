using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Investimentos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Investimentos.Infrastructure.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly AppDbContext _context;

        public AssetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Asset>> GetAllAsync() => await _context.Assets.ToListAsync();

        public async Task<Asset?> GetByIdAsync(int id) => await _context.Assets.FindAsync(id);

        public async Task<Asset?> GetBySymbolAsync(string symbol) =>
            await _context.Assets.FirstOrDefaultAsync(a => a.Symbol == symbol);

        public async Task AddAsync(Asset asset)
        {
            await _context.Assets.AddAsync(asset);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Asset asset)
        {
            _context.Assets.Update(asset);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AnyAsync() => await _context.Assets.AnyAsync();
    }
}
