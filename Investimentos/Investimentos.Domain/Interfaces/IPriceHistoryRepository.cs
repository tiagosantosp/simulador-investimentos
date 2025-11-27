using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Interfaces
{
    public interface IPriceHistoryRepository
    {
        Task<List<PriceHistory>> GetBySymbolAsync(string symbol);
        Task AddRangeAsync(List<PriceHistory> histories);
        Task<bool> AnyAsync();
    }
}
