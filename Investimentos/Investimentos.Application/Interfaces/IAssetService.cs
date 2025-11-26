using Investimentos.Application.DTOs;

namespace Investimentos.Application.Interfaces
{
    public interface IAssetService
    {
        Task<List<AssetDto>> GetAllAsync();
        Task<AssetDto?> GetBySymbolAsync(string symbol);
        Task UpdatePriceAsync(string symbol, decimal newPrice);
        Task<AssetDto?> GetByIdAsync(int id);
        Task<AssetDto> CreateAsync(CreateAssetDto dto);
    }
}