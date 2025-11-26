using Investimentos.Application.DTOs;

namespace Investimentos.Application.Interfaces
{
    public interface IAssetService
    {
        Task<List<AssetDto>> GetAllAsync();
        Task<AssetDto?> GetBySymbolAsync(string symbol);
        Task UpdatePriceAsync(string symbol, decimal newPrice);
    }
}