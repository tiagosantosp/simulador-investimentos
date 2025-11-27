using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Investimentos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Investimentos.Infrastructure.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly AppDbContext _context;

        public PortfolioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Portfolio>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Portfolios
                .Include(p => p.Positions) 
                .ThenInclude(pos => pos.Asset) 
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<Portfolio?> GetByIdWithPositionsAsync(int id)
        {
            return await _context.Portfolios
                .Include(p => p.Positions)
                .ThenInclude(pos => pos.Asset)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Portfolio portfolio)
        {
            await _context.Portfolios.AddAsync(portfolio);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Portfolio portfolio)
        {
            _context.Portfolios.Update(portfolio);
            await _context.SaveChangesAsync();
        }

        public async Task AddPositionAsync(Position position)
        {
            await _context.Positions.AddAsync(position);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePositionAsync(int positionId)
        {
            var position = await _context.Positions.FindAsync(positionId);
            if (position != null)
            {
                _context.Positions.Remove(position);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Position?> GetPositionByIdAsync(int positionId)
        {
            return await _context.Positions
                .Include(p => p.Asset)
                .FirstOrDefaultAsync(p => p.Id == positionId);
        }

        public async Task UpdatePositionAsync(Position position)
        {
            _context.Positions.Update(position);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AnyAsync() => await _context.Portfolios.AnyAsync();
    }
}
