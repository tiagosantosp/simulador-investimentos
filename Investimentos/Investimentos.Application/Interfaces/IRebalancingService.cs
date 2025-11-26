using Investimentos.Application.DTOs;

namespace Investimentos.Application.Interfaces
{
    public interface IRebalancingService
    {
        Task<RebalancingResultDto> GetRebalancingSuggestionsAsync(int portfolioId);
    }
}