using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Interface
{
    public interface IPortfolioRepository
    {
        Task<List<Portfolio>> GetAllByUserIdAsync(string userId);
        Task<Portfolio?> GetByIdWithPositionsAsync(int id); // Importante: Trazer posições junto!
        Task AddAsync(Portfolio portfolio);
        Task UpdateAsync(Portfolio portfolio);
        Task DeletePositionAsync(int positionId);
        Task AddPositionAsync(Position position);
        Task<bool> AnyAsync();
    }
}
