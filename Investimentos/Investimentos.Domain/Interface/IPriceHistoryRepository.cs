using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Interface
{
    public interface IPriceHistoryRepository
    {
        Task<List<PriceHistory>> GetBySymbolAsync(string symbol);
        Task AddRangeAsync(List<PriceHistory> histories);
        Task<bool> AnyAsync();
    }
}
