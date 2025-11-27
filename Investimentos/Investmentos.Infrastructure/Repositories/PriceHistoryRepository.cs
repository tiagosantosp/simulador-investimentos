using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Investimentos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Investimentos.Infrastructure.Repositories
{
    public class PriceHistoryRepository : IPriceHistoryRepository
    {
        private readonly AppDbContext _context;

        public PriceHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PriceHistory>> GetBySymbolAsync(string symbol)
        {
            return await _context.PriceHistories
                .Where(p => p.AssetSymbol == symbol)
                .OrderBy(p => p.Date)
                .ToListAsync();
        }

        public async Task AddRangeAsync(List<PriceHistory> histories)
        {
            await _context.PriceHistories.AddRangeAsync(histories);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AnyAsync() => await _context.PriceHistories.AnyAsync();
    }
}
