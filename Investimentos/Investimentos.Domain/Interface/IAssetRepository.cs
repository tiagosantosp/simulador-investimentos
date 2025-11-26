using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Interfaces
{
    public interface IAssetRepository
    {
        Task<List<Asset>> GetAllAsync();
        Task<Asset?> GetByIdAsync(int id);
        Task<Asset?> GetBySymbolAsync(string symbol);
        Task AddAsync(Asset asset);
        Task UpdateAsync(Asset asset);
        // Útil para verificar se o banco está vazio na hora do Seed
        Task<bool> AnyAsync();
    }
}