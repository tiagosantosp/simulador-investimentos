using Investimentos.Application.DTOs;

namespace Investimentos.Application.Interfaces
{
    public interface IRiskAnalysisService
    {
        Task<RiskAnalysisDto> AnalyzePortfolioRiskAsync(int portfolioId);
    }
}