using Investimentos.Application.DTOs;

namespace Investimentos.Application.Interfaces
{
    public interface IPortfolioService
    {
        Task<PortfolioDto> CreateAsync(CreatePortfolioDto dto);
        Task<PortfolioDto?> GetByIdAsync(int id);
        Task<List<PortfolioDto>> GetAllByUserIdAsync(string userId);
        Task AddPositionAsync(int portfolioId, string symbol, int quantity, decimal price);
    }
}